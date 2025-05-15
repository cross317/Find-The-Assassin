using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody rb;
    public GameObject player;
    [SerializeField] GameObject panelPlayerUseTask;

    public bool isCollidingWithTask = false;
    Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }



    private void FixedUpdate() 
    {
        if (GameManager.Instance.task1.hasPlayed)
        {
            return;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        direction = new Vector3(-moveHorizontal, 0,-moveVertical);
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
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("task1"))
        {
            isCollidingWithTask = false;
        }
    }

    public void Update()
    {
        Vector3 pPosition = player.transform.position;
        pPosition.y = 0.1f;
        player.transform.position = pPosition;
        
        if (isCollidingWithTask)
        {
            panelPlayerUseTask.SetActive(true);
        }
        else if(!isCollidingWithTask)
        {
            panelPlayerUseTask.SetActive(false);
        }
        
    }
}

