using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Image = Microsoft.Unity.VisualStudio.Editor.Image;
using Photon.Pun;

public class Player_Controller : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody rb;
    public GameObject player;
    [SerializeField] GameObject panelPlayerUseTask;
    [Serialize] public GameObject panelForInventory1;
    [SerializeField] public GameObject panelForNotHavingGasCan;
    [SerializeField] public GameObject map;
    [SerializeField] public GameObject missionsPanel;
    [SerializeField] Image loadingImage;

    public bool isCollidingWithTask = false;
    public bool isCollidingWithTask2 = false;
    public bool isCollidingWithCan = false;
    public bool isCollidingWithTask3 = false;
    public bool isCollidingWithPlayer = false;
    public bool isAssassin = false;
    public bool isDead = false;
    public bool canDoTasks = true;

    Vector3 direction;

    GameManager gameManager;

    public GameObject[] players;
    GameObject giocatorePiuVicino;

    public Script1Task task1;
    public Script2Task task2;
    public Task3_Script task3;
    public bool canPlay;

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
        
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().enabled = false;
        }
        player = gameObject;

        Player_Controller playerScript = player.GetComponent<Player_Controller>();
        if (playerScript != null)
        {
            playerScript.task1 = FindObjectOfType<Script1Task>();
            playerScript.task2 = FindObjectOfType<Script2Task>();
            playerScript.canPlay = true;
        }
        else
        {
            Debug.LogError("PlayerScript non trovato sul prefab instanziato!");
        }

        rb = GetComponent<Rigidbody>();
        task3Script = FindObjectOfType<Task3_Script>();
        gameManager = FindObjectOfType<GameManager>();
    
    //if (PhotonNetwork.IsMasterClient)
    //{
    //    isAssassin = true;
    //}
    // else
    //{
    //    isAssassin = false;
    //}  
        isAssassin = false;
            print(isAssassin + ": isAssassin");

        rb = GetComponent<Rigidbody>();
       
        if (isAssassin == false)
        {
            player.tag = "Player";
            canDoTasks = true;
            Debug.Log("Ruolo attuale: " + (isAssassin ? "Assassin" : "Player"));
        }
        if (isAssassin == true)
        {
            player.tag = "Assassin";
            canDoTasks = false;
            Debug.Log("Ruolo attuale: " + (isAssassin ? "Assassin" : "Player"));
        }
        players = GameObject.FindGameObjectsWithTag("Player");
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

        if (gameManager.isTask1Complete == true && gameManager.isTask2Complete == true && gameManager.isTask3Complete == true)
        {
            missionsPanel.SetActive(false);
        }

        if (isAssassin == true)
        {   
            if (Input.GetMouseButtonDown(0) && isCollidingWithPlayer == true)
            {
                Attack();
                Debug.Log("isDead =" + isDead);
                Debug.Log("Ruolo attuale: " + (isAssassin ? "Assassin" : "Player"));
            }
        }

        if (players.Length <= 0)
        {
            SceneManager.LoadScene("AssassinWon");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.task1.hasPlayed && GameManager.Instance.canPlay == false || GameManager.Instance.task2.canDisable == true || task3Script.isPanel1Active == true)
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
            isCollidingWithCan = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidingWithPlayer = false;
        }
    }

    [PunRPC]
    public void Attack()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        GameObject giocatorePiuVicino = null;
        float distanzaMinima = 35f;

        foreach (GameObject player in players)
        {

            float distanza = Vector3.Distance(transform.position, player.transform.position);
            if (distanza < distanzaMinima)
            {
                distanzaMinima = distanza;
                giocatorePiuVicino = player;
            }

        }
        if (giocatorePiuVicino != null)
        {
            Camera cameraGiocatore = giocatorePiuVicino.GetComponentInChildren<Camera>();
            if (cameraGiocatore != null)
            {
                cameraGiocatore.transform.SetParent(null);
            }
            PhotonNetwork.Destroy(giocatorePiuVicino);
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        else
        {
            Debug.Log("Nessun giocatore valido trovato!");
        }
    }
}
