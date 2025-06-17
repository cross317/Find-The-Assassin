using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceMaager : MonoBehaviourPunCallbacks
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

    [SerializeField] public GameObject task1Prefab;
    Script1Task task1Script;
    Task3_Script task3Script;
    GameManager gameManager;

    public void Start()
    {
        task3Script = FindObjectOfType<Task3_Script>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void CreateTask1ForPlayer(Player_Controller player)
    {
        GameObject task1 = Instantiate(task1Prefab);
        Script1Task script = task1.GetComponent<Script1Task>();
        script.AssignPlayer(player);
        Debug.Log("Task1 creata e assegnata al player");
    }


}
