using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text timerText;
    [SerializeField] GameObject primaryPanel;
    [SerializeField] GameObject secondaryPanel;
    [SerializeField] TMP_Text playerName;
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] TMP_InputField input_Field;

    public float timer = 5f;
    public bool canStartTimer = false;
    private bool isConnectedToMaster = false;
    public static string savedName = "";
    public static bool isNameChanged = false;

    public void Start()
    {   
        primaryPanel.SetActive(true);
        secondaryPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        if (isNameChanged == false)
        {
            PhotonNetwork.NickName = "Player" + Random.Range(1, 9999);
            savedName = PhotonNetwork.NickName;
            playerName.text = "Your name is:" + savedName;
        }

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            isConnectedToMaster = true;
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
        if (input_Field.text != "")
        {
            ChangeName();
        }
    }

    public void CreateRoom()
    {
        if (!isConnectedToMaster)
        {
            Debug.LogWarning("Non ancora connesso");
            return;
        }
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props["isGameStarted"] = false;

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = true;
        options.IsOpen = true;
        options.CustomRoomProperties = props;
        options.CustomRoomPropertiesForLobby = new string[] { "isGameStarted" };

        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Sei entrato nella lobby. Ora puoi unirti a una stanza.");
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count == 0)
        {
            Debug.Log("Nessuna stanza disponibile.");
        }
        else
        {
            Debug.Log("Stanze disponibili:");
            foreach (RoomInfo room in roomList)
            {
                if (room.CustomProperties.TryGetValue("isGameStarted", out object started) && (bool)started)
                {
                    Debug.Log("Stanza già in corso, ignorata: " + room.Name);
                    continue;
                }

                Debug.Log("Stanza valida: " + room.Name);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Sei entrato nella stanza");

        if (PhotonNetwork.MasterClient == null || PhotonNetwork.PlayerList.Length == 0)
        {
            Debug.Log("Stanza vuota o senza master, la chiudo.");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LeaveRoom();
            return;
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("isGameStarted", out object started) && (bool)started)
        {
            Debug.Log("Partita già in corso, non posso entrare.");
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

    public void ChangeName()
    {
        PhotonNetwork.NickName = input_Field.text;
        savedName = PhotonNetwork.NickName;
        if (playerName != null)
            playerName.text = "Your name is:\n" + PhotonNetwork.NickName;
    }

    public void ActiveInfoPanel()
    {
        infoPanel.SetActive(true);
    }
    
    public void DeactivateInfoPanel()
    {
        infoPanel.SetActive(false);
    }

    public void ActiveSettingsPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void DeactivateSettingsPanel()
    {
        settingsPanel.SetActive(false);   
    }

}
