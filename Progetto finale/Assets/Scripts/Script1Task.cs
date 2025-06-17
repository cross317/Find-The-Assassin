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

    public bool hasPlayed = false;
    public float currentTimeTask;

    private void Update()
    {
        if (playerController == null) return;

        if (playerController.isCollidingWithTask && playerController.canDoTasks)
        {
            if (Input.GetKeyDown(KeyCode.E) && !hasPlayed)
            {
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
                Debug.Log("Task 1 completata");

                currentTimeTask = 0f;
                playerController.CompleteTask1();
            }
        }
    }

    private IEnumerator Start()
    {
        while (playerController == null)
        {
            playerController = FindObjectOfType<Player_Controller>();
            yield return null;
        }

        Debug.Log("playerController locale trovato: " + (playerController != null));

        gameManager = FindObjectOfType<GameManager>();
        hasPlayed = false;
    }

    public void AssignPlayer(Player_Controller controller)
    {
        playerController = controller;
    }
}