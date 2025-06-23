using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
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
    [SerializeField] GameObject AssassinWon;
    private bool hasShownWinScreen = false;
    private PhotonView photonView;
    int randSpawn;
    private int alivePlayers = 0;
    private bool gameEnded = false;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "FinalUnityProject")
        {
            Debug.Log("[GameManager] Scena attuale non è FinalUnityProject. Distruggo questo oggetto.");
            Destroy(gameObject);
            return;
        }
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
        if (SceneManager.GetActiveScene().name != "FinalUnityProject")
        {
            return;
        }

        totalTasks = 0;
        gameEnded = false;
        hasShownWinScreen = false;
        StartCoroutine(WaitForPhotonAndSpawn());
    }

    public void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null || gameEnded == true) return;

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;   

        switch (playerCount)
        {
            case 1:
                howManyTasksToWin = 3;
                break;
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
            gameEnded = true;
            photonView.RPC("ShowInnocentsWon", RpcTarget.All);
        }

    }

    private IEnumerator WaitForPhotonAndSpawn()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return null;
        }

        Debug.Log("In stanza: istanzio i player da Coroutine");
        hasShownWinScreen = false;
        alivePlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        SpawnPlayers();
        StartCoroutine(AssignAssassinDelayed());

        yield return new WaitForSeconds(1f);

        task1 = FindObjectOfType<Script1Task>();
        task2 = FindObjectOfType<Script2Task>();

        if (PhotonNetwork.IsMasterClient)
        {
            ResetAllPlayers();
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
            gameEnded = true;
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
        AssassinWon.SetActive(true);
    }

    public void OnReturnToMenuButtonClick()
    {
        ReturnToMenu();
    }

    public void ReturnToMenu()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Menu");
    }

    [PunRPC]
    public void AssignAssassin(int actorNumber)
    {
        foreach (var player in FindObjectsOfType<Player_Controller>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                player.isAssassin = true;
                player.tag = "Assassin";
                player.canDoTasks = false;
                Debug.Log($"[GameManager] Questo player è l’ASSASSINO: {actorNumber}");
            }
            else
            {
                player.isAssassin = false;
                player.tag = "Player";
                player.canDoTasks = true;
                Debug.Log($"[GameManager] Questo player è INNOCENTE: {player.photonView.Owner.ActorNumber}");
            }
        }
    }
    private IEnumerator AssignAssassinDelayed()
    {
        yield return new WaitUntil(() => FindObjectsOfType<Player_Controller>().Length == PhotonNetwork.CurrentRoom.PlayerCount);
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.IsMasterClient)
        {
            int masterActorNumber = PhotonNetwork.MasterClient.ActorNumber;
            photonView.RPC("AssignAssassin", RpcTarget.All, masterActorNumber);
        }
    }

    private int GetRandomPlayerActorNumber()
    {
        List<Photon.Realtime.Player> players = new List<Photon.Realtime.Player>(PhotonNetwork.PlayerList);
        int index = Random.Range(0, players.Count);
        return players[index].ActorNumber;
    }

    public void ResetAllPlayers()
    {
        foreach (Player_Controller p in FindObjectsOfType<Player_Controller>())
        {
            p.photonView.RPC("ResetPlayer", p.photonView.Owner);
        }
    }
}
