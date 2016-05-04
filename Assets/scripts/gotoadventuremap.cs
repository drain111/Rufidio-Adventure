using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class gotoadventuremap : MonoBehaviour {

    public static monsterdata savedGame;

    // Use this for initialization
    void Start () {
   
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public void GoToAdventure()
    {
        savedGame = monsterdata.current;
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        print(Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
        bf.Serialize(file, savedGame);
        file.Close();
        SceneManager.LoadScene("adventure map");
        
    }
}
