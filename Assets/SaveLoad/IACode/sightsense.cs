using UnityEngine;
using System.Collections;

public class sightsense : MonoBehaviour {
    public GameObject sight;
    public Transform monster;
    public GameObject sphere;
    public int i;
    public int j;
    public int[,] thingsseen;
    Rigidbody rb;
    public float maxSpeed = 5f;
    public float moveForce = 365f;
    public int rightmost = 0;

    bool isonland = false;
    public bool jump = false;
    public float jumpForce = 1000f;
    public Transform groundCheck;



    // Use this for initialization
    void Awake () {
        //get monster to move it

        rb = monster.GetComponent<Rigidbody>();

        //creation of the array that contains the objects that the monster sees
        Bounds size = sight.GetComponent<MeshFilter>().sharedMesh.bounds;
        Vector3 actualsize = Matrix4x4.Scale(sight.GetComponent<Transform>().localScale) * size.size;
        Bounds monstersize = sphere.GetComponent<MeshFilter>().sharedMesh.bounds;
        Vector3 monsteractualsize = Matrix4x4.Scale(sphere.GetComponent<Transform>().localScale) * monstersize.size;
        i = (int)Mathf.Ceil(actualsize.x / monsteractualsize.x);
        j = (int)Mathf.Ceil(actualsize.y / monsteractualsize.y);

        thingsseen = new int[j, i];


    }
    public int getRightMost()
    {
        return rightmost;
    }
    public void setRightMost(int RightMost)
    {
        rightmost = RightMost;
    } 
    public bool checkDistance()
    {
        if ((int)Mathf.Floor(monster.position.x) > rightmost)
        {
            rightmost = (int) Mathf.Floor(monster.position.x);
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update () {
        Bounds monstersize = sphere.GetComponent<MeshFilter>().sharedMesh.bounds;
        Vector3 monsteractualsize = Matrix4x4.Scale(sphere.GetComponent<Transform>().localScale) * monstersize.size;
        Vector3 corner = new Vector3(sight.GetComponent<Collider>().bounds.center.x + sight.GetComponent<Collider>().bounds.extents.x,
                         sight.GetComponent<Collider>().bounds.center.y + sight.GetComponent<Collider>().bounds.extents.y,
                         sight.GetComponent<Collider>().bounds.center.z);
        for (int l = 0; l < j; l++)
        {
            for (int m = 0; m < i; m++)
            {
                
                RaycastHit hit;
                Debug.DrawRay(monster.position, new Vector3(corner.x - monsteractualsize.x / 2 - monsteractualsize.x * m, corner.y - monsteractualsize.y / 2 - monsteractualsize.y * l, 0), Color.red);
                if (Physics.Raycast(monster.position, new Vector3(corner.x - monsteractualsize.x / 2 - monsteractualsize.x * m, corner.y - monsteractualsize.y / 2 - monsteractualsize.y * l, 0), out hit))
                {


                    switch (hit.transform.tag)
                    {
                        case "Ground":
                            thingsseen[l,m] = 1;
                            break;
                        default:
                            thingsseen[l,m] = 0;
                            break;

                    }
                }
                else thingsseen[l,m] = 0;
            }
        }





        ////////////////////Movement

        if (isonland)
        {
            jump = true;
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8) isonland = true;
        else isonland = false;
    }
    public void movemonster(Hashtable hashTable)
    {
        
        if ((bool) hashTable["Left"] == true)
        {
            float h = -0.5f;
            if (h * rb.velocity.x < maxSpeed)
                rb.AddForce(Vector2.left * h * moveForce);

            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        else if ((bool)hashTable["Right"] == true)
        {
            float h = 0.5f;
            if (h * rb.velocity.x < maxSpeed)
                rb.AddForce(Vector2.right * h * moveForce);

            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        if ((bool)hashTable["Jump"] == true)
        {
            if (jump && isonland)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                jump = false;
                isonland = false;
            }
        }
    }
}
