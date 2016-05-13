using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//[System.Serializable]
public class MonsterAbilities : MonoBehaviour {
    public static MonsterAbilities current;
    public int learningjump = 0;
    public bool jumplearned = false;
    public Text jumptext;
    public int money = 0;
    monsterdata monsterdata;

	// Use this for initialization
	void Start () {
        monsterdata.current = new monsterdata();

        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            monsterdata = (monsterdata)bf.Deserialize(file);
            file.Close();
            jumplearned = monsterdata.jumplearned;
            learningjump = monsterdata.learningjump;
            money = monsterdata.money;
            jumptext.text = "Jump learned : " + learningjump + " of 100\nMoney earned: " + money;

        }


    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Spring") && learningjump < 100)
        {
            learningjump++;
            
            jumptext.text = "Jump learned : " + learningjump + " of 100\nMoney earned: " + money;
            if (learningjump == 100)
            {
                jumplearned = true;
            }

        }

        monsterdata.current.jumplearned = jumplearned;
        monsterdata.current.learningjump = learningjump;
        monsterdata.current.money = money;
    }
}
