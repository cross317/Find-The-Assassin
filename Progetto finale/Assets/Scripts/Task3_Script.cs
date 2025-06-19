using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Task3_Script : MonoBehaviourPunCallbacks
{
    Player_Controller playerController;
    [SerializeField] GameObject gasCan;
    [SerializeField] Camera thirdCamera;
    [SerializeField] Animator animator;
    GameManager gameManager;

    public bool canDoTask3 = false;
    public float canStop1 = 0f;
    public bool isMainCameraActive = true;
    public float timeToFinishTask3 = 0f;
    public bool canDisablePanelInventory = false;
    private bool isRunningTask3 = false;
    public bool canChangeCamera = true;
    public bool isTask3Completed = false;

    private IEnumerator Start()
    {
        while (FindObjectOfType<Player_Controller>() == null)
            yield return null;

        foreach (var pc in FindObjectsOfType<Player_Controller>())
        {
            if (pc.photonView.IsMine)
            {
                playerController = pc;
                break;
            }
        }
        gameManager = FindObjectOfType<GameManager>();

        if (playerController == null)
        {
            Debug.LogError("playerController ancora NULL dopo attesa!");
            yield break;
        }

        if (playerController.mainCamera == null)
        {
            Debug.LogError("mainCamera nel playerController è NULL!");
            yield break;
        }

        playerController.mainCamera.enabled = true;
        thirdCamera.enabled = false;
        isTask3Completed = false;

        Debug.Log("Task3 inizializzato correttamente.");
    }
    private void Update()
    {
        if (playerController == null || playerController.mainCamera == null) return;

        if (playerController.isCollidingWithCan == true && playerController.canDoTasks == true)
        {
            canDoTask3 = true;
            gasCan.SetActive(false);
            playerController.panelForInventory1.SetActive(true);
        }
        if (canDoTask3 == false && playerController.isCollidingWithTask3 == true && Input.GetKeyDown(KeyCode.E) && playerController.canDoTasks == true)
        {
            playerController.panelForNotHavingGasCan.SetActive(true);
            playerController.isPanel1Active = true;
        }
        if (canDoTask3 == false && playerController.isCollidingWithTask3 == true && playerController.isPanel1Active == true && playerController.canDoTasks == true)
        {
            canStop1 += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.E) && canStop1 >= 1.5f)
            {
                playerController.panelForNotHavingGasCan.SetActive(false);
                canStop1 = 0f;
                playerController.isPanel1Active = false;
            }
        }
        if (canDoTask3 == true && playerController.isCollidingWithTask3 == true && Input.GetKeyDown(KeyCode.E) && playerController.canDoTasks == true)
        {
            if (!playerController.isMainCameraLocked && !isTask3Completed)
            {
                isMainCameraActive = false;
                isRunningTask3 = true; 
                playerController.isMainCameraLocked = true; 
            }
        }
        if (isRunningTask3 && isMainCameraActive == false)
        {
            timeToFinishTask3 += Time.deltaTime;

            if (playerController.photonView.IsMine)
            {
                thirdCamera.enabled = true;
                playerController.mainCamera.enabled = false;
            }

            animator.SetTrigger("Play Once");
        }
        if (isRunningTask3 && timeToFinishTask3 >= 2.5f)
        {
            isMainCameraActive = true;

            if (playerController.photonView.IsMine)
            {
                thirdCamera.enabled = false;
                if (canChangeCamera == true)
                {
                    playerController.mainCamera.enabled = true;
                    canChangeCamera = false;
                    canDoTask3 = false;
                }
                if (canChangeCamera == false)
                {
                    Debug.Log("All good");
                }
            }

            playerController.isMainCameraLocked = false;
            canDisablePanelInventory = true;
            //canDoTask3 = false;
            playerController.CompleteTask3();
            isTask3Completed = true;
            animator.SetTrigger("Stop");
        }
        if (canDisablePanelInventory == true)
        {
            playerController.panelForInventory1.SetActive(false);
        }
    }
}
