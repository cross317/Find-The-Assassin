using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceMaager : MonoBehaviour
{
    public static ReferenceMaager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool isCollidingWithTask = false;
    public bool isCollidingWithTask2 = false;
    public bool isCollidingWithCan = false;
    public bool isCollidingWithTask3 = false;
    public bool isCollidingWithPlayer = false;
    public bool isAssassin = false;
    public bool isDead = false;
    public bool canDoTasks = true;
    public bool isPanel1Active = false;

    Task3_Script task3Script;
    GameManager gameManager;

    public void Start()
    {
        task3Script = FindObjectOfType<Task3_Script>();
        gameManager = FindObjectOfType<GameManager>();
    }

  
}
