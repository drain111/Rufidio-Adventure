using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class startGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GoToScene ()
    {
        SceneManager.LoadScene("house scene");
    }
}
