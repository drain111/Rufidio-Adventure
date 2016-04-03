using UnityEngine;
using System.Collections;

public class rufidiocontroller : MonoBehaviour {
    bool isonland = false;
    public bool jump = false;
    public float moveForce = 365f;
    public float maxSpeed = 5f;
    public float jumpForce = 1000f;
    public Transform groundCheck;


    private bool grounded = false;
    private Rigidbody rb;
    // Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");


        if (h * rb.velocity.x < maxSpeed)
            rb.AddForce(Vector2.right * h * moveForce);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);


        if (jump)
        {
            rb.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump") && isonland)
        {
            jump = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8) isonland = true;
        else isonland = false;
    }
    }
