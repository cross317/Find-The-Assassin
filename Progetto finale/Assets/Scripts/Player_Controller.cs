using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody rb;
    public GameObject player;
    [SerializeField] GameObject panelPlayerUseTask;
    [Serialize] public GameObject panelForInventory1;
    [SerializeField] public GameObject panelForNotHavingGasCan;
    [SerializeField] public GameObject map;
    [SerializeField] public GameObject missionsPanel;

    public bool isCollidingWithTask = false;
    public bool isCollidingWithTask2 = false;
    public bool isCollidingWithCan = false;
    public bool isCollidingWithTask3 = false;

    Vector3 direction;

    GameManager gameManager;
    Task3_Script task3Script;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        task3Script = FindObjectOfType<Task3_Script>();
        gameManager = FindObjectOfType<GameManager>();
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
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.task1.hasPlayed && GameManager.Instance.canPlay == false || GameManager.Instance.task2.canDisable == true || task3Script.isPanel1Active == true)
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

    }
}