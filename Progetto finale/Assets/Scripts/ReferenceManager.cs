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

    Task3_Script task3Script;
    GameManager gameManager;

    public void Start()
    {
        task3Script = FindObjectOfType<Task3_Script>();
        gameManager = FindObjectOfType<GameManager>();
    }

  
}
