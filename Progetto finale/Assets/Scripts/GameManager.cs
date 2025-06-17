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
    private PhotonView photonView;
    int randSpawn;
    private int alivePlayers = 0;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        totalTasks = 0;

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("In stanza: istanzio i player da Start()");
            hasShownWinScreen = false;
            alivePlayers = PhotonNetwork.CurrentRoom.PlayerCount;
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
            case 2:
                howManyTasksToWin = 3;
                break;
            case 3:
                howManyTasksToWin = 6;
                break;
            case 4:
                howManyTasksToWin = 9;
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

    public void PlayerDied()
    {
        alivePlayers--;
        Debug.Log("Player morto. Rimasti: " + alivePlayers);

        if (!hasShownWinScreen && alivePlayers <= 1)
        {
            hasShownWinScreen = true;
            photonView.RPC("ShowAssassinWon", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ShowInnocentsWon()
    {
        InnocentsWon.SetActive(true);
    }
    [PunRPC]
    public void ShowAssassinWon()
    {
        PhotonNetwork.LoadLevel("AssassinWon");
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
