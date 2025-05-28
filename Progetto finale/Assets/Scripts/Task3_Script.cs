using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task3_Script : MonoBehaviour
{
    Player_Controller playerController;
    [SerializeField] GameObject gasCan;
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera thirdCamera;
    [SerializeField] Animator animator;

    public bool canDoTask3 = false;
    public float canStop1 = 0f;
    public bool isPanel1Active = false;
    public bool isMainCameraActive = true;
    public float timeToFinishTask3 = 0f;
    public bool canDisablePanelInventory = false;

    public void Start()
    {
        playerController = FindObjectOfType<Player_Controller>();
        animator = FindObjectOfType<Animator>();
        mainCamera.enabled = true;
        thirdCamera.enabled = false;
    }

    private void Update()
    {
        if (playerController.isCollidingWithCan == true)
        {
            canDoTask3 = true;
            gasCan.SetActive(false);
            playerController.panelForInventory1.SetActive(true);
        }
        if (canDoTask3 == false && playerController.isCollidingWithTask3 == true && Input.GetKeyDown(KeyCode.E))
        {
            playerController.panelForNotHavingGasCan.SetActive(true);
            isPanel1Active = true;
        }
        if (canDoTask3 == false && playerController.isCollidingWithTask3 == true && isPanel1Active == true)
        {
            canStop1 += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.E) && canStop1 >= 1.5f)
            {
                playerController.panelForNotHavingGasCan.SetActive(false);
                canStop1 = 0f;
                isPanel1Active = false;
            }
        }
        if (canDoTask3 == true && playerController.isCollidingWithTask3 == true && Input.GetKeyDown(KeyCode.E))
        {
            isMainCameraActive = false;
        }
        if (isMainCameraActive == false)
        {
            timeToFinishTask3 += Time.deltaTime;
            thirdCamera.enabled = true;
            mainCamera.enabled = false;
            animator.SetTrigger("Play Once");
        }
        if (timeToFinishTask3 >= 2.5f && playerController.isCollidingWithTask3 == true)
        {
            isMainCameraActive = true;
            thirdCamera.enabled = false;
            mainCamera.enabled = true;
            canDisablePanelInventory = true;
        }
        if (canDisablePanelInventory == true)
        {
            playerController.panelForInventory1.SetActive(false);
        }
    }
}
