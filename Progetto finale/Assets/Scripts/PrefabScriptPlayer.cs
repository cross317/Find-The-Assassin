using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PrefabScriptPlayer : MonoBehaviourPunCallbacks
{
    public PhotonView childPhotonView;

    private void Awake()
    {
        childPhotonView = GetComponentInChildren<PhotonView>();
    }
}
