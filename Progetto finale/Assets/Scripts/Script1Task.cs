using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Script1Task : MonoBehaviourPunCallbacks
{
    public Animator animator;
    GameManager gameManager;
    Player_Controller playerController;

    [SerializeField] float timeTask;
    [SerializeField] GameObject maskForTask1;

    public bool hasPlayed = false;
    public float currentTimeTask;

    private void Update()
    {
        if (playerController == null) return;

        if (playerController.isCollidingWithTask && playerController.canDoTasks)
        {
            if (Input.GetKeyDown(KeyCode.E) && !hasPlayed)
            {
                Debug.Log("Tasto E premuto in " + gameObject.name);
                animator.SetTrigger("PlayOnce");
                hasPlayed = true;
                GameManager.Instance.canPlay = false;
            }
        }

        if (hasPlayed)
        {
            currentTimeTask += Time.deltaTime;

            if (currentTimeTask >= timeTask)
            {
                animator.SetTrigger("Stop");
                GameManager.Instance.canPlay = true;
                photonView.RPC("AttivaMaskPerTutti", RpcTarget.AllBuffered);
                Debug.Log("Task 1 completata");

                currentTimeTask = 0f;
                playerController.isTask1Complete = true;
            }
        }
    }

    private IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        Player_Controller[] players;
        do
        {
            players = FindObjectsOfType<Player_Controller>();
            yield return null;
        } while (players.Length == 0);

        foreach (var p in players)
        {
            if (p.photonView.IsMine)
            {
                playerController = p;
                break;
            }
        }

        Debug.Log("playerController locale trovato: " + (playerController != null));
        gameManager = FindObjectOfType<GameManager>();
    }

    [PunRPC]
    public void AttivaMaskPerTutti()
    {
        if (maskForTask1 != null)
        {
            maskForTask1.SetActive(true);
        }
    }
}