using System;
using Microsoft.Unity.VisualStudio.Editor;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;

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

    public GameObject[] players;
    GameObject giocatorePiuVicino;

    public bool canPlay;
    public bool isMainCameraLocked = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ReferenceMaager.Instance.isAssassin);
            stream.SendNext(ReferenceMaager.Instance.isDead);
        }
        else
        {
            ReferenceMaager.Instance.isAssassin = (bool)stream.ReceiveNext();
            ReferenceMaager.Instance.isDead = (bool)stream.ReceiveNext();
        }
    }

    private void Start()
    {

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
        }
        player = gameObject;

        Player_Controller playerScript = player.GetComponent<Player_Controller>();
        mainCamera = GetComponentInChildren<Camera>();
        mainCamera.enabled = true;
        if (mainCamera == null)
        {
            Debug.LogError("❌ mainCamera non trovata sul body!");
        }
        else
        {
            Debug.Log("✅ mainCamera trovata correttamente!");
            mainCamera.enabled = photonView.IsMine;
        }

        if (playerScript != null)
        {
            playerScript.canPlay = true;
        }
        else
        {
            Debug.LogError("PlayerScript non trovato sul prefab instanziato!");
        }

        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    isAssassin = true;
        //}
        // else
        //{
        //    isAssassin = false;
        //}  
        ReferenceMaager.Instance.isAssassin = false;
        print(ReferenceMaager.Instance.isAssassin + ": isAssassin");

        if (ReferenceMaager.Instance.isAssassin == false)
        {
            player.tag = "Player";
            ReferenceMaager.Instance.canDoTasks = true;
            Debug.Log("Ruolo attuale: " + (ReferenceMaager.Instance.isAssassin ? "Assassin" : "Player"));
        }
        if (ReferenceMaager.Instance.isAssassin == true)
        {
            player.tag = "Assassin";
            ReferenceMaager.Instance.canDoTasks = false;
            Debug.Log("Ruolo attuale: " + (ReferenceMaager.Instance.isAssassin ? "Assassin" : "Player"));
        }
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void Update()
    {
        Vector3 pPosition = player.transform.position;
        pPosition.y = 0.1f;
        player.transform.position = pPosition;

        if (ReferenceMaager.Instance.isCollidingWithTask || ReferenceMaager.Instance.isCollidingWithTask2 == true || ReferenceMaager.Instance.isCollidingWithTask3 == true)
        {
            panelPlayerUseTask.SetActive(true);

        }
        else if (!ReferenceMaager.Instance.isCollidingWithTask || ReferenceMaager.Instance.isCollidingWithTask2 == false || ReferenceMaager.Instance.isCollidingWithTask3 == false)
        {
            panelPlayerUseTask.SetActive(false);
        }
        if (ReferenceMaager.Instance.isCollidingWithCan == false && ReferenceMaager.Instance.isCollidingWithTask == false && ReferenceMaager.Instance.isCollidingWithTask2 == false && ReferenceMaager.Instance.isCollidingWithTask3 == false)
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

        if (gameManager.isTask1Complete == true && gameManager.isTask2Complete == true && gameManager.isTask3Complete == true)
        {
            missionsPanel.SetActive(false);
        }

        if (ReferenceMaager.Instance.isAssassin == true)
        {
            if (Input.GetMouseButtonDown(0) && ReferenceMaager.Instance.isCollidingWithPlayer == true)
            {
                Attack();
                Debug.Log("isDead =" + ReferenceMaager.Instance.isDead);
                Debug.Log("Ruolo attuale: " + (ReferenceMaager.Instance.isAssassin ? "Assassin" : "Player"));
            }
        }

        if (players.Length <= 0)
        {
            PhotonNetwork.LoadLevel("AssassinWon");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.task1.hasPlayed && GameManager.Instance.canPlay == false || GameManager.Instance.task2.canDisable == true || ReferenceMaager.Instance.isPanel1Active == true)
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
            ReferenceMaager.Instance.isCollidingWithTask = true;
            Debug.Log("Tutto ok");
        }
        if (collision.gameObject.CompareTag("task2"))
        {
            ReferenceMaager.Instance.isCollidingWithTask2 = true;
        }
        if (collision.gameObject.CompareTag("task3"))
        {
            ReferenceMaager.Instance.isCollidingWithTask3 = true;
        }
        if (collision.gameObject.CompareTag("GasCan"))
        {
            ReferenceMaager.Instance.isCollidingWithCan = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            ReferenceMaager.Instance.isCollidingWithPlayer = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("task1"))
        {
            ReferenceMaager.Instance.isCollidingWithTask = false;
        }
        if (collision.gameObject.CompareTag("task2"))
        {
            ReferenceMaager.Instance.isCollidingWithTask2 = false;
        }
        if (collision.gameObject.CompareTag("task3"))
        {
            ReferenceMaager.Instance.isCollidingWithTask3 = false;
        }
        if (collision.gameObject.CompareTag("GasCan"))
        {
            ReferenceMaager.Instance.isCollidingWithCan = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            ReferenceMaager.Instance.isCollidingWithPlayer = false;
        }
    }

    public void Attack()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        GameObject target = null;
        float distanzaMinima = 35f;

        foreach (GameObject p in players)
        {
            if (p == this.gameObject) continue;

            float distanza = Vector3.Distance(transform.position, p.transform.position);
            if (distanza < distanzaMinima)
            {
                distanzaMinima = distanza;
                target = p;
            }
        }
        if (target != null)
        {
            PhotonView targetPV = target.GetComponent<PhotonView>();

            if (targetPV != null)
            {
                targetPV.RPC("OnKilled", targetPV.Owner);
                Debug.Log("🎯 Ucciso: " + target.name);
            }
        }
    }
    [PunRPC]
    public void OnKilled()
    {
        if (mainCamera != null)
        {
            mainCamera.enabled = false;
        }
        Debug.Log("Sei stato ucciso!");
        PhotonNetwork.Destroy(gameObject);
    }

}