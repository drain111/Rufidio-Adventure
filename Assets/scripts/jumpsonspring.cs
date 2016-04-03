using UnityEngine;
using System.Collections;

public class jumpsonspring : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Equals("monster")) {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 80000, 0));

        }
       

    }
}
