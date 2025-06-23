using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text playersText;
    [SerializeField] GameObject playButton;

    public void Start()
    {
        RefreshPlayers();
        if (!PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(false);
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("FinalUnityProject");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "Left the room");
        RefreshPlayers();
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            playButton.SetActive(true);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "Entered the room");
        RefreshPlayers();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && PhotonNetwork.IsMasterClient == false)
        {
            PhotonNetwork.SetMasterClient(newPlayer);
            Debug.Log("Nuovo MasterClient assegnato: " + newPlayer.NickName);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Nuovo MasterClient assegnato: " + newMasterClient.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        if (PhotonNetwork.IsConnectedAndReady) PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    [PunRPC]
    public void ShowPlayers()
    {
        playersText.text = "Players: ";

        foreach (Photon.Realtime.Player otherPlayer in PhotonNetwork.PlayerList)
        {
            playersText.text += "\n";
            playersText.text += otherPlayer.NickName;
        }
    }

    public void RefreshPlayers()
    {
        if (photonView == null)
        {
            Debug.LogError("photonView è nullo! Assicurati che LobbyManager abbia un PhotonView.");
            return;
        }

        photonView.RPC("ShowPlayers", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ShowPlayers", RpcTarget.All);
        }
    }

}
