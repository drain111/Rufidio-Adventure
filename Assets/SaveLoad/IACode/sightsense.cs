using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public int money = 0;
    bool isonland = false;
    public bool jump = false;
    public float jumpForce = 1000f;
    public Transform groundCheck;
    float actualsizex;
    float actualsizey;
    float cornerx;
    float cornery;
    public int sleep = 100;
    public int hungry = 100;
    public Text moneytext;
    public int coinlevel = 0;


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
        cornerx = sight.GetComponent<Collider>().bounds.center.x + sight.GetComponent<Collider>().bounds.extents.x;
        cornery = sight.GetComponent<Collider>().bounds.center.y + sight.GetComponent<Collider>().bounds.extents.y;
        actualsizex = monsteractualsize.x;
        actualsizey = monsteractualsize.y;


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
    public void changeMoneyText()
    {
        moneytext.text = "Money earned: " + money;

    }
    // Update is called once per frame
    void FixedUpdate () {
        Bounds monstersize = sphere.GetComponent<MeshFilter>().sharedMesh.bounds;
        Vector3 monsteractualsize = Matrix4x4.Scale(sphere.GetComponent<Transform>().localScale) * monstersize.size;
       
        for (int l = 0; l < j; l++)
        {
            for (int m = 0; m < i; m++)
            {
                //- monsteractualsize.x / 2 
                //- monsteractualsize.y / 2 
                RaycastHit hit;
                Debug.DrawRay(monster.position, new Vector3(cornerx / 2 + actualsizex * m, cornery - actualsizey * l, 0), Color.red);
                if (Physics.Raycast(monster.position, new Vector3(cornerx / 2 + actualsizex * m, cornery - actualsizey * l, 0), out hit))
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

        if (Time.time % 12 == 0 && sleep > 0)
        {
            sleep--;
        }
        if (Time.time % 4 == 0 && hungry > 0)
        {
            hungry--;
        }

        moneytext.text = "Money earned: " + money + "\nAwake: " + sleep + "\nHungry: " + hungry;




    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8) isonland = true;
     
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 8) isonland = false;

    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == 9)

        {
            
            money = money + (int) Mathf.Pow(2, coinlevel);
                   
            



            monsterdata.current.money = money;
            Destroy(collision.gameObject);
            moneytext.text = "Money earned: " + money + "\nAwake: " + sleep + "\nHungry: " + hungry;
        }
    }
    public void movemonster(Hashtable hashTable)
    {
        
        if ((bool) hashTable["Left"] == true)
        {
            float h = -1f;
            if (h * rb.velocity.x < maxSpeed)
                rb.AddForce(Vector2.left * h * moveForce);

            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        else if ((bool)hashTable["Right"] == true)
        {
            float h = 1f;
            if (h * rb.velocity.x < maxSpeed)
                rb.AddForce(Vector2.left * h * moveForce);

            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        if ((bool)hashTable["Jump"] == true)
        {
            if (isonland)
            {
                rb.AddForce(new Vector2(0f, jumpForce));
                isonland = false;
            }
        }
    }
}
