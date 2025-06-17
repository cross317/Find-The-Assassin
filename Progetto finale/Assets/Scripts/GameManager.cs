using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks

{
    public Script1Task task1;
    public Script2Task task2;
    public bool canPlay;
    [SerializeField] List<Transform> spawns = new List<Transform>();
    public int howManyTasksToWin;
    public int totalTasks = 0;
    [SerializeField] GameObject InnocentsWon;
    private bool hasShownWinScreen = false;

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
        totalTasks = 0;
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("In stanza: istanzio i player da Start()");
            hasShownWinScreen = false;
            SpawnPlayers();
        }
        else
        {
            Debug.LogError("Non sei in una stanza! Impossibile istanziare i player.");
        }
    }

    public void Update()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;   

        switch (playerCount)
        {
            case 1:
                howManyTasksToWin = 3;
                break;
            case 2:
                howManyTasksToWin = 6;
                break;
            case 3:
                howManyTasksToWin = 9;
                break;
            case 4:
                howManyTasksToWin = 12;
                break;
        }
        if (!hasShownWinScreen && totalTasks >= howManyTasksToWin)
        {
            hasShownWinScreen = true;
            photonView.RPC("ShowInnocentsWon", RpcTarget.All);
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

    [PunRPC]
    public void ShowInnocentsWon()
    {
        InnocentsWon.SetActive(true);
    }

    public void ReturnToMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        StartCoroutine(LoadMenuAfterLeaving());
    }

    private IEnumerator LoadMenuAfterLeaving()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        SceneManager.LoadScene("Menu");
    }
}
