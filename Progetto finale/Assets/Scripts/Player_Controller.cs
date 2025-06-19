using System;
using System.Collections;
using ExitGames.Client.Photon;
using Mono.Cecil.Cil;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player_Controller : MonoBehaviourPunCallbacks, IPunObservable
{
    Vector3 direction;
    public float speed = 100f;
    [SerializeField] public GameObject player;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject panelPlayerUseTask;
    [SerializeField] GameObject map;
    [SerializeField] GameObject missionsPanel;
    [Serialize] public GameObject panelForInventory1;
    [SerializeField] public GameObject panelForNotHavingGasCan;
    GameManager gameManager;
    [SerializeField] public Camera mainCamera;
    [SerializeField] float distanzaMinima = 4f;

    public bool isCollidingWithTask = false;
    public bool isCollidingWithTask2 = false;
    public bool isCollidingWithCan = false;
    public bool isCollidingWithTask3 = false;
    public bool isCollidingWithPlayer = false;
    public bool isAssassin = false;
    public bool isDead = false;
    public bool canDoTasks = true;
    public bool isPanel1Active = false;
    public bool hasCountedTasks = false;
    public bool isTask1Complete = false;
    public bool isTask2Complete = false;
    public bool isTask3Complete = false;

    public GameObject[] players;
    GameObject giocatorePiuVicino;

    public bool canPlay;
    public bool isMainCameraLocked = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isAssassin);
            stream.SendNext(isDead);
        }
        else
        {
            isAssassin = (bool)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
        }
    }

    private void Start()
    {
        player = gameObject;
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();

        isTask1Complete = false;
        isTask2Complete = false;
        isTask3Complete = false;
        hasCountedTasks = false;

        if (!photonView.IsMine)
        {
            if (mainCamera != null)
                mainCamera.enabled = false;

            if (panelPlayerUseTask != null)
                panelPlayerUseTask.SetActive(false);

            if (map != null)
                map.SetActive(false);

            if (missionsPanel != null)
                missionsPanel.SetActive(false);

            return;
        }

        StartCoroutine(InitCamera());

        if (missionsPanel != null)
            missionsPanel.SetActive(false);

        if (ReferenceManager.Instance != null)
        {
            ReferenceManager.Instance.CreateTask1ForPlayer(this);
        }
        else
        {
            Debug.LogWarning("ReferenceManager non trovato!");
        }

        if (isAssassin)
        {
            player.tag = "Assassin";
            canDoTasks = false;
            missionsPanel.SetActive(false);
            Debug.Log("Ruolo attuale: Assassin");
        }
        else
        {
            player.tag = "Player";
            canDoTasks = true;
            missionsPanel.SetActive(true);
            Debug.Log("Ruolo attuale: Player");
        }

        players = GameObject.FindGameObjectsWithTag("Player");
        canPlay = true;
    }

    public void Update()
    {
        Vector3 pPosition = player.transform.position;
        pPosition.y = 0.1f;
        player.transform.position = pPosition;

        if (isCollidingWithTask || isCollidingWithTask2 == true || isCollidingWithTask3 == true)
        {
            panelPlayerUseTask.SetActive(true);

        }
        else if (!isCollidingWithTask || isCollidingWithTask2 == false || isCollidingWithTask3 == false)
        {
            panelPlayerUseTask.SetActive(false);
        }
        if (isCollidingWithCan == false && isCollidingWithTask == false && isCollidingWithTask2 == false && isCollidingWithTask3 == false)
        {
            if (Input.GetKey(KeyCode.M))
            {
                map.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.M))
            {
                map.SetActive(false);
            }
        }

        if (isTask1Complete && isTask2Complete && isTask3Complete && !hasCountedTasks)
        {
            if (photonView.IsMine)
            {
                missionsPanel.SetActive(false);
                GameManager.Instance.totalTasks += 3;
                hasCountedTasks = true;
                Debug.Log(GameManager.Instance.totalTasks + ": total tasks");
            }
        }

        if (isAssassin == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Attack();
                Debug.Log("isDead =" + isDead);
                Debug.Log("Ruolo attuale: " + (isAssassin ? "Assassin" : "Player"));
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.task1.hasPlayed && GameManager.Instance.canPlay == false || GameManager.Instance.task2.canDisable == true || isPanel1Active == true)
        {
            return;
        }
        if (!photonView.IsMine) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(moveHorizontal * speed, rb.velocity.y, moveVertical * speed);

        Debug.Log("Horizontal: " + direction.x);
        Debug.Log("Vertical: " + direction.z);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision has been made");
        if (collision.gameObject.CompareTag("Wall"))
        {
            rb.velocity = Vector3.zero;
        }
        if (collision.gameObject.CompareTag("task1"))
        {
            isCollidingWithTask = true;
            Debug.Log("Tutto ok");
        }
        if (collision.gameObject.CompareTag("task2"))
        {
            isCollidingWithTask2 = true;
        }
        if (collision.gameObject.CompareTag("task3"))
        {
            isCollidingWithTask3 = true;
        }
        if (collision.gameObject.CompareTag("GasCan"))
        {
            isCollidingWithCan = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("task1"))
        {
            isCollidingWithTask = false;
        }
        if (collision.gameObject.CompareTag("task2"))
        {
            isCollidingWithTask2 = false;
        }
        if (collision.gameObject.CompareTag("task3"))
        {
            isCollidingWithTask3 = false;
        }
        if (collision.gameObject.CompareTag("GasCan"))
        {
            isCollidingWithCan = false;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    public void Attack()
    {
        if (!photonView.IsMine || !isAssassin) return;

        Player_Controller target = null;
        float minDistance = distanzaMinima;

        foreach (Player_Controller other in FindObjectsOfType<Player_Controller>())
        {
            if (other == this || other.isDead || other.isAssassin) continue;

            float distanza = Vector3.Distance(transform.position, other.transform.position);
            if (distanza < minDistance)
            {
                minDistance = distanza;
                target = other;
            }
        }

        if (target != null)
        {
            PhotonView targetPV = target.GetComponent<PhotonView>();
            if (targetPV != null)
            {
                if (!target.isDead)
                {
                    targetPV.RPC("OnKilled", targetPV.Owner);
                    Debug.Log($"[Attack] Ucciso: {targetPV.Owner.ActorNumber}");
                }
            }
        }
        else
        {
            Debug.Log("[Attack] Nessun bersaglio valido nel raggio");
        }
    }

    [PunRPC]
    public void OnKilled()
    {
        isDead = true;

        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }

        Debug.Log("Sei stato ucciso!");

        if (photonView != null && photonView.IsMine)
        {
            GameManager.Instance.PlayerDied();
            photonView.RPC("DestroyPlayerRPC", RpcTarget.All, photonView.ViewID);
        }
    }

    public void CompleteTask1()
    {
        isTask1Complete = true;
    }

    public void CompleteTask2()
    {
        isTask2Complete = true;
    }

    public void CompleteTask3()
    {
        isTask3Complete = true;
    }

    [PunRPC]
    public void DestroyPlayerRPC(int viewID)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
        }
        else
        {
            Debug.LogWarning($"[DestroyPlayerRPC] PhotonView {viewID} non trovato");
        }
    }

    private IEnumerator InitCamera()
    {
        yield return new WaitUntil(() => GetComponentInChildren<Camera>() != null);

        mainCamera = GetComponentInChildren<Camera>();

        if (mainCamera != null)
        {
            mainCamera.enabled = photonView.IsMine;
            Debug.Log($"mainCamera trovata e {(photonView.IsMine ? "attivata" : "disattivata")} correttamente!");
        }
        else
        {
            Debug.LogError("mainCamera ancora non trovata!");
        }
    }
}