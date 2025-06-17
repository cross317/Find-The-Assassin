using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssassinWonManager : MonoBehaviourPunCallbacks
{
    public float secondToWait = 0f;

    private void Update()
    {
        secondToWait += Time.deltaTime;
    }

    public void ReturnToMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        if (secondToWait >= 2f)
        {
            StartCoroutine(LoadMenuAfterLeaving());
        }
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
