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

    public float timer = 5f;
    public bool canStartTimer = false;

    public void Start()
    {   

        primaryPanel.SetActive(true);
        secondaryPanel.SetActive(false);
        PhotonNetwork.NickName = "Player" + Random.Range(1, 9999);
        Debug.Log("Player Name:" + PhotonNetwork.NickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Update()
    {
        if (timer > 0 && canStartTimer == true)
        {
            timer -= Time.deltaTime;
            AggiornaTimerUi();
        }
        if (timer <= 0)
        {
            CreateRoom();
            timer = 5f;
        }
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 4 });
    }

    public void JoinRoom()
    {       
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Sei entrato nella stanza!");
        SceneManager.LoadScene("Lobby");
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To The Server");
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
