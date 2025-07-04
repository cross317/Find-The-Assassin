using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

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
    [SerializeField] GameObject panelForAssassinKill;
    [SerializeField] TMP_Text textToKill;

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
    public bool canAttack = true;
    public float timeToAttack = 30f;

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
        gameManager = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        isTask1Complete = false;
        isTask2Complete = false;
        isTask3Complete = false;
        hasCountedTasks = false;
        canAttack = true;

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
            textToKill.text = "You can kill";
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
                hasCountedTasks = true;
                GameManager.Instance.photonView.RPC("ReportTasksCompleted", RpcTarget.MasterClient);
            }
        }

        if (!canAttack && isAssassin == true)
        {
            timeToAttack -= Time.deltaTime;
            textToKill.text = timeToAttack.ToString("F0") + ": To kill";
            Debug.Log("Sto decrementando il timer" + timeToAttack);
        }

        if (timeToAttack <= 0f && isAssassin == true)
        {
            canAttack = true;
            timeToAttack = 30f;
            textToKill.text = "You can kill";
        }

        if (isAssassin == true)
        {
            panelForAssassinKill.SetActive(true);
            if (Input.GetMouseButtonDown(0) && canAttack == true)
            {
                Attack();
                Debug.Log("isDead =" + isDead);
                Debug.Log("Ruolo attuale: " + (isAssassin ? "Assassin" : "Player"));
                Debug.Log("Hai cliccato, inizio a decrementare il timer");
            }
            
            if (canAttack == true)
            {
                textToKill.text = "You can kill";
            }
        }
    }

 
    private void FixedUpdate()
    {
        if (!photonView.IsMine || rb == null) return;

        if (GameManager.Instance == null || GameManager.Instance.task1 == null || GameManager.Instance.task2 == null)
            return;

        if ((GameManager.Instance.task1.hasPlayed && !GameManager.Instance.canPlay) ||
            GameManager.Instance.task2.canDisable ||
            isPanel1Active)
        {
            return;
        }

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

        canAttack = false;

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
        if (isDead) return;
        isDead = true;
        Debug.Log("Sei stato ucciso!");
        GameManager.Instance.PlayerDied();
        if (photonView.IsMine)
        {
            photonView.RPC("HidePlayerBody", RpcTarget.AllBuffered);

            mainCamera.transform.SetParent(null);
            mainCamera.gameObject.SetActive(true);
            rb.velocity = Vector3.zero;
            if (missionsPanel != null) missionsPanel.SetActive(false);
            if (map != null) map.SetActive(false);
            if (panelPlayerUseTask != null) panelPlayerUseTask.SetActive(false);
            if (panelForInventory1 != null) panelForInventory1.SetActive(false);
            if (panelForNotHavingGasCan != null) panelForNotHavingGasCan.SetActive(false);

            foreach (MonoBehaviour comp in GetComponents<MonoBehaviour>())
            {
                if (comp != this) comp.enabled = false;
            }

            if (!isAssassin && hasCountedTasks)
            {
                GameManager.Instance.photonView.RPC("SubtractTasksForDeadPlayer", RpcTarget.MasterClient);
            }

            StartCoroutine(LeaveRoomAfterDelay(1.5f));
        }
    }

    private IEnumerator LeaveRoomAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void HidePlayerBody()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = false;
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
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Solo il MasterClient può distruggere gli oggetti Photon.");
            return;
        }

        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            PhotonNetwork.Destroy(targetView.gameObject);
            Debug.Log($"[DestroyPlayerRPC] Oggetto distrutto dal MasterClient (ViewID: {viewID})");
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

    [PunRPC]
    public void ResetPlayer()
    {
        isDead = false;
        canDoTasks = !isAssassin;
        hasCountedTasks = false;
        isTask1Complete = false;
        isTask2Complete = false;
        isTask3Complete = false;

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = true;

        foreach (Collider c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        gameObject.tag = isAssassin ? "Assassin" : "Player";

        if (photonView.IsMine)
        {
            if (missionsPanel != null) missionsPanel.SetActive(!isAssassin);
            if (map != null) map.SetActive(false);
            if (panelPlayerUseTask != null) panelPlayerUseTask.SetActive(false);
            if (panelForInventory1 != null) panelForInventory1.SetActive(false);
            if (panelForNotHavingGasCan != null) panelForNotHavingGasCan.SetActive(false);

            StartCoroutine(InitCamera());
        }

        Debug.Log("Player reset completato");
    }
}