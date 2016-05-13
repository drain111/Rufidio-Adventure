using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[System.Serializable]

public class monsterdata{
    public static monsterdata current;
    public int learningjump;
    public bool jumplearned;
    public int money;
    // Use this for initialization
    public monsterdata()
    {
       learningjump = 0;
       jumplearned = false;
       money = 0;
    }
}
