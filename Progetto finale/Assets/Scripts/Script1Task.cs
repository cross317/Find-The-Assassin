using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script1Task : MonoBehaviour
{
    public Animator animator;
    GameObject player;
    Player_Controller playerController;
    [SerializeField] float timeTask;
    [SerializeField] GameObject maskForTask1;

    public bool hasPlayed = false;
    public float currentTimeTask;

    private void Update()
    {
        if (playerController.isCollidingWithTask)
        {
            if (Input.GetKeyUp(KeyCode.E)) 
            {
                if (hasPlayed == false)
                {
                    Debug.Log("Tasto E premuto in " + gameObject.name);
                    animator.SetTrigger("PlayOnce");
                    hasPlayed = true;
                    GameManager.Instance.canPlay = false;
                }
            }
        }
        if (hasPlayed == true)
        {
            currentTimeTask += Time.deltaTime;
            if (currentTimeTask >= timeTask)
            {
                animator.SetTrigger("Stop");     
                GameManager.Instance.canPlay = true;
                maskForTask1.SetActive(true);
                Debug.Log(hasPlayed);
                currentTimeTask = 3f;
            }
        }
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<Player_Controller>();
        Debug.Log("playerController trovato: " + (playerController != null));
    }
}
