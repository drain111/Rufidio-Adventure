using UnityEngine;
using System.Collections;

public class movemonster : MonoBehaviour {
    public Transform Monster;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

       
	}
    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(Monster.position);

        offset = Monster.position - Camera.main.ScreenToWorldPoint(
     new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

    }

    /*void OnMouseOver()
    {
        Vector3 aux = new Vector3(Input.mousePosition.x, Monster.position.y, Monster.position.z);
        Monster.position = aux;
        Debug.Log("Hello");
    }*/
}
