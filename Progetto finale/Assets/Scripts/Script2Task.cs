using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script2Task : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Camera secondCamera;
    Player_Controller playerController;
    [SerializeField] GameObject lightBlock;
    [SerializeField] Material newMaterial;
    [SerializeField] Material newMaterial2;

    public bool canDisable = false;
    public float timeForCanDisable = 0f;

    private void Update()
    {
        if (playerController.isCollidingWithTask2)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                mainCamera.enabled = false;
                secondCamera.enabled = true;
                canDisable = true;
                lightBlock.GetComponent<MeshRenderer>().material = newMaterial;
                Debug.Log("Tasto E premuto!");
            }
        }
        if (canDisable == true && timeForCanDisable >= 3f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("canDisable: " + canDisable);
                mainCamera.enabled = true;
                secondCamera.enabled = false;
                canDisable = false;
                lightBlock.GetComponent<MeshRenderer>().material = newMaterial2;
                timeForCanDisable = 0f;

            }
        }
        if (canDisable == true)
        {
            timeForCanDisable += Time.deltaTime;
        }
    }

    private void Start()
    {
        playerController = FindObjectOfType<Player_Controller>();
        if (playerController == null)
        {
            Debug.LogError("Player_Controller non trovato! Assicurati che esista nella scena.");
        }
    }
}
