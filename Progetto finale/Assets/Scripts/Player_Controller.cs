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
    [Serialize] GameObject panelForInventory1;
    [SerializeField] GameObject panelForNotHavingGasCan;
    [SerializeField] GameObject gasCan;

    public bool isCollidingWithTask = false;
    public bool isCollidingWithTask2 = false;
    public bool isCollidingWithCan = false;
    public bool isCollidingWithTask3 = false;
    public bool canDoTask3 = false;

    Vector3 direction;

    GameManager gameManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.task1.hasPlayed && GameManager.Instance.canPlay == false || GameManager.Instance.task2.canDisable == true)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        direction = new Vector3(-moveHorizontal, 0, -moveVertical);
        direction = transform.TransformDirection(direction);
        rb.velocity = direction * speed * 5.5f;
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
        }
        if (collision.gameObject.CompareTag("task2"))
        {
            isCollidingWithTask2 = true;
        }
        if (collision.gameObject.CompareTag("GasCan"))
        {
            isCollidingWithCan = true;
        }
        if (collision.gameObject.CompareTag("task3"))
        {
            isCollidingWithTask3 = true;
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
    }

    public void Update()
    {
        Vector3 pPosition = player.transform.position;
        pPosition.y = 0.1f;
        player.transform.position = pPosition;

        if (isCollidingWithTask || isCollidingWithTask2 == true || isCollidingWithCan == true || isCollidingWithTask3 == true)
        {
            panelPlayerUseTask.SetActive(true);

        }
        else if (!isCollidingWithTask || isCollidingWithTask2 == false || isCollidingWithCan == false || isCollidingWithTask3 == false)
        {
            panelPlayerUseTask.SetActive(false);
        }
        if (isCollidingWithCan == true && Input.GetKeyDown(KeyCode.E))
        {
            panelForInventory1.SetActive(true);
            canDoTask3 = true;
        }
        if (isCollidingWithTask3 == true && Input.GetKeyDown(KeyCode.E) && canDoTask3 == false)
        {
            panelForNotHavingGasCan.SetActive(true);
        }
        if (isCollidingWithTask3 == false)
        {
            panelForNotHavingGasCan.SetActive(false);
        }
    }
}