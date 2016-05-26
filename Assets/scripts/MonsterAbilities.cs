using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
//[System.Serializable]
public class MonsterAbilities : MonoBehaviour {
    public static MonsterAbilities current;
    int learningjump = 0;
    bool jumplearned = false;
    public Text jumptext;
    int sleep = 100;
    int hungry = 100;
    int money = 0;
    public int bedcost = 100;
    public int foodcost = 100;
    public GameObject bed;
    public int bedlevel = 0;
    public int foodlevel = 0;
    public GameObject food;
    public int moneycost = 100;
    public int coinlevel = 0;

    monsterdata monsterdata;
    

	// Use this for initialization
	void Awake () {
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
            sleep = monsterdata.sleep;
            hungry = monsterdata.hungry;
            bedlevel = monsterdata.bedlevel;
            foodlevel = monsterdata.foodlevel;
            bedcost = monsterdata.bedprice;
            foodcost = monsterdata.foodprice;
            moneycost = monsterdata.moneycost;
            coinlevel = monsterdata.coinlevel;
            jumptext.text = "Jump learned : " + learningjump + " of 100\nMoney earned: " + money;


        }


    }
	public int getMoney()
    {
        return money;
    }
    public void setMoney(int MoMoney)
    {
        money = MoMoney;
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

    }
    void OnCollisionStay(Collision collision)
    {
        
        if (collision.gameObject.tag.Equals("Food") && hungry < 100)
        {
            switch (foodlevel)
            {
                case 0:
                    if (Time.time % 10 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 1:
                    if (Time.time % 9 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 2:
                    if (Time.time % 8 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 3:
                    if (Time.time % 7 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 4:
                    if (Time.time % 6 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 5:
                    if (Time.time % 5 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 6:
                    if (Time.time % 4 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 7:
                    if (Time.time % 3 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 8:
                    if (Time.time % 2 == 0)
                    {
                        hungry++;
                    }
                    break;
                case 9:
                    if (Time.time % 1 == 0)
                    {
                        hungry++;
                    }
                    break;
                default:
                    break;
            }



        }
        else
       if (collision.gameObject.tag.Equals("Bed") && sleep < 100)
        {
            switch (bedlevel)
            {
                case 0:
                    if (Time.time % 20 == 0)
                    {
                        sleep++;
                        
                    }
                    break;
                case 1:
                    if (Time.time % 19 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 2:
                    if (Time.time % 18 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 3:
                    if (Time.time % 17 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 4:
                    if (Time.time % 16 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 5:
                    if (Time.time % 15 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 6:
                    if (Time.time % 14 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 7:
                    if (Time.time % 13 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 8:
                    if (Time.time % 12 == 0)
                    {
                        sleep++;
                    }
                    break;
                case 9:
                    if (Time.time % 11 == 0)
                    {
                        sleep++;
                    }
                    break;
                default:
                    break;
            }



        }
        jumptext.text = "Jump learned : " + learningjump + " of 100\nMoney earned: " + money + "\nAwake: " + sleep + "\nHungry: " + hungry;

        monsterdata.current.sleep = sleep;
        monsterdata.current.hungry = hungry;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Spring") && learningjump < 100)
        {
            learningjump++;
            
            if (learningjump == 100)
            {
                jumplearned = true;
            }
            jumptext.text = "Jump learned : " + learningjump + " of 100\nMoney earned: " + money + "\nAwake: " + sleep + "\nHungry: " + hungry;

        }



        monsterdata.current.jumplearned = jumplearned;
        monsterdata.current.learningjump = learningjump;
        monsterdata.current.money = money;
        

    }
}
