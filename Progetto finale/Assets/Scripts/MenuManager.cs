using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject primaryPanel;
    [SerializeField] GameObject secondaryPanel;
    [SerializeField] TMP_Text playerName;

    public float timer = 5f;
    public bool canStartTimer = false;
    private bool isConnectedToMaster = false;

    public void Start()
    {   
        primaryPanel.SetActive(true);
        secondaryPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Update()
    {
        if (!isConnectedToMaster) return;

        if (timer > 0 && canStartTimer == true)
        {
            timer -= Time.deltaTime;
            AggiornaTimerUi();
        }
        if (timer <= 0 && isConnectedToMaster && !PhotonNetwork.InRoom)
        {
            CreateRoom();
            timer = 5f;
        }
    }

    public void CreateRoom()
    {
        if (!isConnectedToMaster)
        {
            Debug.LogWarning("Non ancora connesso");
            return;
        }
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Sei entrato nella lobby. Ora puoi unirti a una stanza.");
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby(); 
        }
        else
        {
            Debug.LogWarning("Aspetta la connessione al Master Server prima di unirti a una stanza!");
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Sei entrato nella stanza");
        Debug.Log("Sei entrato nella lobby, Ora puoi unirti a una stanza");
        if (PhotonNetwork.MasterClient == null || PhotonNetwork.PlayerList.Length == 0)
        {
            Debug.LogWarning("Stanza vuota, esco...");
            PhotonNetwork.LeaveRoom();
            return;
        }
        SceneManager.LoadScene("Lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnesso da Photon: " + cause);
        isConnectedToMaster = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To The Server");
        isConnectedToMaster = true;
        Debug.Log("Player Name:" + PhotonNetwork.NickName);

        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            PhotonNetwork.NickName = "Player" + Random.Range(1, 9999);

        if (playerName != null)
            playerName.text = "Your name is:\n" + PhotonNetwork.NickName;
    }

    public void AggiornaTimerUi()
    {
        int secondi = Mathf.FloorToInt(timer);
        timerText.text = secondi.ToString("00");
    }

    public void ChangeSet()
    {
        primaryPanel.SetActive(false);
        secondaryPanel.SetActive(true);
        canStartTimer = true;
    }
}
