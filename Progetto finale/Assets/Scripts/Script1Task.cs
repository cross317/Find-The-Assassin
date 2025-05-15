using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script1Task : MonoBehaviour
{
    public Animator animator;
    public bool hasPlayed = false;
    GameObject player;
    Player_Controller playerController;
    
    [SerializeField] float timeTask;
    private float currentTimeTask;

    private void Update()
    {
        if (playerController.isCollidingWithTask)
        {
            if (Input.GetKeyUp(KeyCode.E)) 
            {
                if (hasPlayed == false)
                {
                    animator.SetTrigger("PlayOnce");
                    hasPlayed = true;
                }
            }
        }      
        if (hasPlayed)
        {
            currentTimeTask += Time.deltaTime;
            if (currentTimeTask >= timeTask)
            {
                animator.SetTrigger("Stop");
                hasPlayed = false;
                currentTimeTask = 0;
                
            }

            
        }
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<Player_Controller>();
    }
}
