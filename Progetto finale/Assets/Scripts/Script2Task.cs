using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Script2Task : MonoBehaviourPunCallbacks
{
    [SerializeField] Camera secondCamera;
    Player_Controller playerController;
    [SerializeField] GameObject lightBlock;
    [SerializeField] Material newMaterial;
    [SerializeField] Material newMaterial2;
    GameManager gameManager;

    public bool canDisable = false;
    public float timeForCanDisable = 0f;

    private void Update()
    {
        if (playerController.isCollidingWithTask2)
        {
            if (Input.GetKeyDown(KeyCode.E) && playerController.canDoTasks == true)
            {
                if (!playerController.isMainCameraLocked)
                {
                    playerController.isMainCameraLocked = true; 
                    playerController.mainCamera.enabled = false;
                    secondCamera.enabled = true;
                    canDisable = true;
                    photonView.RPC("CambiaColore", RpcTarget.All);
                
                    Debug.Log("Tasto E premuto in Task2!");
                }
            }
        }
        if (canDisable == true && timeForCanDisable >= 3f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("canDisable: " + canDisable);
                playerController.isMainCameraLocked = false;
                playerController.mainCamera.enabled = true;
                secondCamera.enabled = false;
                canDisable = false;
                photonView.RPC("CambiaColore2", RpcTarget.All);
                playerController.CompleteTask2();
                timeForCanDisable = 0f;

            }
        }
        if (canDisable == true)
        {
            timeForCanDisable += Time.deltaTime;
        }
    }

    private IEnumerator Start()
    {
        while (FindObjectOfType<Player_Controller>() == null)
        {
            yield return null; 
        }

        playerController = FindObjectOfType<Player_Controller>();
        gameManager = FindObjectOfType<GameManager>();

        if (playerController.mainCamera == null)
        {
            Debug.LogError("mainCamera non trovata nel Player_Controller!");
            yield break;
        }

        playerController.mainCamera.enabled = true;
        secondCamera.enabled = false;

        Debug.Log("Script2Task inizializzato correttamente.");
    }

    [PunRPC]
    public void CambiaColore2()
    {
        lightBlock.GetComponent<MeshRenderer>().material = newMaterial2;
    }

    [PunRPC]
    public void CambiaColore()
    {
        lightBlock.GetComponent<MeshRenderer>().material = newMaterial;
    }
}