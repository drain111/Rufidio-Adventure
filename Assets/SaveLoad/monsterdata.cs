using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[System.Serializable]

public class monsterdata{
    public static monsterdata current;
    public int learningjump;
    public bool jumplearned;
    public int money;
    public int sleep;
    public int hungry;
    public int bedlevel;
    public int foodlevel;
    public int foodprice;
    public int bedprice;
    public int coinlevel;
    public int moneycost;
    // Use this for initialization
    public monsterdata()
    {
        learningjump = 0;
        jumplearned = false;
        money = 0;
        sleep = 100;
        hungry = 100;
        bedlevel = 0;
        foodlevel = 0;
        foodprice = 100;
        bedprice = 1000;
        coinlevel = 0;
        moneycost = 100;
    }
}
