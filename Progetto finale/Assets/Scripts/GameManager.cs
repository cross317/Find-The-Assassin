using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks

{
    public Script1Task task1;
    public Script2Task task2;
    public bool canPlay;
    public bool isTask1Complete = false;
    public bool isTask2Complete = false;
    public bool isTask3Complete = false;
    [SerializeField] List<Transform> spawns = new List<Transform>();
    public int howManyTasksToWin;
    public int totalTasks;
    public int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

    int randSpawn;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {   
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("In stanza: istanzio i player da Start()");
            SpawnPlayers();
        }
        else
        {
            Debug.LogError("Non sei in una stanza! Impossibile istanziare i player.");
        }
    }

    public void Update()
    {   
        switch (playerCount)
        {
            case 1:
                howManyTasksToWin = 3;
                break;
            case 3:
                howManyTasksToWin = 6;
                break;
            case 4:
                howManyTasksToWin = 9;
                break;
        }
    }
        
    public void SpawnPlayers()
    {
        Debug.Log("SpawnPlayers() chiamato");

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            if (spawns.Count > 0)
            {
                randSpawn = Random.Range(0, spawns.Count);
                PhotonNetwork.Instantiate("BasicSkinViewedInGame", spawns[randSpawn].position, spawns[randSpawn].rotation);
            }
            else
            {
                Debug.LogError("Lista degli spawn è vuota!");
            }
        }
    }
}
