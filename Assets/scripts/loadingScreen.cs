using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class loadingScreen : MonoBehaviour {
    public GameObject portalBeginning;
    public GameObject portalEnd;
    public GameObject coin;
    public static MonsterAbilities current;
    int learningjump = 0;
    bool jumplearned = false;
    int sleep = 100;
    int hungry = 100;
    long money = 0;
    public int bedcost = 100;
    public int foodcost = 100;
    public int bedlevel = 0;
    public int foodlevel = 0;
    public int moneycost = 100;
    public int coinlevel = 0;
    public bool withcisthere = false;
    public int witchlevel = 0;
    public int tutorial;

    monsterdata monsterdata;
    public GameObject camera;
    // Use this for initialization
    void Start () {
        monsterdata.current = new monsterdata();
        camera.GetComponent<Transform>().position = new Vector3(portalBeginning.GetComponent<Transform>().position.x, camera.GetComponent<Transform>().position.y, camera.GetComponent<Transform>().position.z);
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            monsterdata = (monsterdata)bf.Deserialize(file);
            file.Close();
            jumplearned = monsterdata.jumplearned;
            learningjump = monsterdata.learningjump;
            money = monsterdata.money;
            sleep = monsterdata.sleep;
            hungry = monsterdata.hungry;
            bedlevel = monsterdata.bedlevel;
            foodlevel = monsterdata.foodlevel;
            bedcost = monsterdata.bedprice;
            foodcost = monsterdata.foodprice;
            moneycost = monsterdata.moneycost;
            coinlevel = monsterdata.coinlevel;
            withcisthere = monsterdata.withcisthere;
            witchlevel = monsterdata.witchlevel;
            tutorial = monsterdata.tutorial;

        }
    }
	
	// Update is called once per frame
	void Update () {
        monsterdata.current.jumplearned = jumplearned;
        monsterdata.current.money = money;
        monsterdata.current.learningjump = learningjump;
        monsterdata.current.hungry = hungry;
        monsterdata.current.sleep = sleep;
        monsterdata.current.bedlevel = bedlevel;
        monsterdata.current.bedprice = bedcost;
        monsterdata.current.foodlevel = foodlevel;
        monsterdata.current.foodprice = foodcost;
        monsterdata.current.coinlevel = coinlevel;
        monsterdata.current.moneycost = moneycost;
        monsterdata.current.withcisthere = withcisthere;
        monsterdata.current.witchlevel = witchlevel;
        monsterdata.current.tutorial = tutorial;

        if (Input.GetButtonDown("Z"))
        {
            camera.GetComponent<Transform>().Translate(1 , 0, 0);

        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == coin)
        {
            other.gameObject.SetActive(false);
            money += (long) Mathf.Pow(2, coinlevel);
        }
        else if (other.gameObject == portalEnd)
        {
            coin.SetActive(true);
            camera.GetComponent<Transform>().position = new Vector3(portalBeginning.GetComponent<Transform>().position.x, camera.GetComponent<Transform>().position.y, camera.GetComponent<Transform>().position.z);
            monsterdata.current.money = money;

            monsterdata = monsterdata.current;

            BinaryFormatter bf = new BinaryFormatter();
            //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
            FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
            bf.Serialize(file, monsterdata);
            file.Close();
        }
    }
}
