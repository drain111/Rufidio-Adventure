using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Threading;

public class gotoadventuremap : MonoBehaviour {

    public static monsterdata savedGame;
    bool loaded = false;
    public Rect windowRect = new Rect(150, 150, 300, 300);
    public Rect ShoppingRect = new Rect(150, 150, 300, 300);
    pleaseWork desiredobject;
    bool shopping = false;
    int[,] sightsense;
    bool hidewindows = true;
    MonsterAbilities shopdata;
    Thread pleasework;
    bool threadcreated = false;
    // Use this for initialization
    void Start () {
        windowRect = new Rect(150, 150, 300, 150);
        shopdata = GameObject.FindGameObjectWithTag("Player").GetComponent<MonsterAbilities>();
        if (Screen.fullScreen)
        {
            ShoppingRect = new Rect(Screen.currentResolution.width / 100, Screen.currentResolution.height / 100, Screen.currentResolution.width * 98 / 100, Screen.currentResolution.height * 98 / 100);

        }
        else
        {
            ShoppingRect = new Rect(Screen.currentResolution.width / 100, Screen.currentResolution.height / 100, Screen.currentResolution.width * 86 / 100, Screen.currentResolution.height * 95 / 100);

        }
        sightsense = new int[5, 10];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                sightsense[i, j] = 0;
            }
        }
        desiredobject = new pleaseWork(sightsense);

        if (!File.Exists("DataFiles/IA/monsters.xml"))
        {
            threadcreated = true;
            //StartCoroutine(loadGenomes(pool));
            pleasework = new Thread(new ThreadStart(desiredobject.ThreadRun));
            try
            {
                pleasework.Start();
            }
            catch (ThreadStateException e)
            {
                Console.WriteLine(e);  // Display text of exception
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e);  // This exception means that the thread
                                       // was interrupted during a Wait
            }

        }
        else
        {
            pleasework = null;
            
            loaded = true;
            hidewindows = false;

        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnGUI()
    {
        if (!hidewindows || !desiredobject.hidewindows)
        {
            windowRect = GUI.Window(0, windowRect, DoMyWindow, "My Window");

        }
        if (!loaded && !desiredobject.loaded)
        {
            windowRect = GUI.Window(1, windowRect, DoPreparedWindow, "My Window");
        }
        if (shopping)
        {
            ShoppingRect = GUI.Window(2, ShoppingRect, Shop, "What to buy?");
        }
    }
    public void startShopping()
    {
        shopping = true;
    }
    void Shop(int windowID)
    {
        if (shopdata.bedlevel < 9)
        {
            if (GUI.Button(new Rect(ShoppingRect.width / 8, ShoppingRect.height / 8, ShoppingRect.width / 8, ShoppingRect.height / 8), "Upgrade the bed, \nin order to make\n the monster rest faster\n for " + shopdata.bedcost))
            {
                if (shopdata.getMoney() >= shopdata.bedcost)
                {
                    shopdata.setMoney(shopdata.getMoney() - shopdata.bedcost);
                    shopdata.bedcost = (int)(shopdata.bedcost * 2.71f);
                    shopdata.bedlevel++;
                    monsterdata.current.bedlevel = shopdata.bedlevel;
                    monsterdata.current.bedprice = shopdata.bedcost;
                    monsterdata.current.money = shopdata.getMoney();

                }

            }
        }
        else
        {
            GUI.Button(new Rect(ShoppingRect.width / 8, ShoppingRect.height / 8, ShoppingRect.width / 8, ShoppingRect.height / 8), "Bed is upgraded \n to the maximum level");
        }
        if (shopdata.foodlevel < 9)
        {
            if (GUI.Button(new Rect(ShoppingRect.width * 3/ 8, ShoppingRect.height / 8, ShoppingRect.width  / 8, ShoppingRect.height / 8), "Upgrade the food, \nin order to make\n the monster eat faster\n for " + shopdata.foodcost))
            {
                if (shopdata.getMoney() >= shopdata.foodcost)
                {
                    shopdata.setMoney(shopdata.getMoney() - shopdata.foodcost);
                    shopdata.foodcost = (int)(shopdata.foodcost * 2.71f);
                    shopdata.foodlevel++;
                    monsterdata.current.foodlevel = shopdata.foodlevel;
                    monsterdata.current.foodprice = shopdata.foodcost;
                    monsterdata.current.money = shopdata.getMoney();

                }

            }
        }
        else
        {
            GUI.Button(new Rect(ShoppingRect.width / 8, ShoppingRect.height / 8, ShoppingRect.width / 8, ShoppingRect.height / 8), "Food is upgraded \n to the maximum level");
        }
        if (GUI.Button(new Rect(ShoppingRect.width * 6 / 8, ShoppingRect.height / 8, ShoppingRect.width / 8, ShoppingRect.height / 8), "Exit the menu"))
            shopping = false;
        if (shopdata.coinlevel < 6)
        {
            if (GUI.Button(new Rect(ShoppingRect.width / 8, ShoppingRect.height * 2/ 8, ShoppingRect.width / 8, ShoppingRect.height / 8), "Upgrade the money \n a coin gives you:  " + shopdata.moneycost))
            {
                if (shopdata.getMoney() >= shopdata.moneycost)
                {
                    shopdata.setMoney(shopdata.getMoney() - shopdata.moneycost);
                    shopdata.moneycost = (int)(shopdata.moneycost * 10);
                    shopdata.coinlevel++;
                    monsterdata.current.coinlevel = shopdata.coinlevel;
                    monsterdata.current.moneycost = shopdata.moneycost;
                    monsterdata.current.money = shopdata.getMoney();

                }

            }
        }
        else
        {
            GUI.Button(new Rect(ShoppingRect.width / 8, ShoppingRect.height / 8, ShoppingRect.width / 8, ShoppingRect.height / 8), "Coin are upgraded \n to the maximum level");

        }

    }
    void DoPreparedWindow(int windowID)
    {
        if (GUI.Button(new Rect(windowRect.x / 2, windowRect.y / 2, 150, 25), "Rufidios isn't ready yet"))
            print("Not ready yet");
            

    }
    void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(windowRect.x / 2, windowRect.y / 2, 150, 25), "Rufidio is ready to go"))
            hidewindows = true;

    }
    Genome newGenome(Pool pool)
    {

        Genome genome = new Genome(0, new List<Neuron>(), 0, 0, 0.25f, 2.0f, 0.4f, 0.5f, 0.2f, 0.4f, 0.1f, new List<Genes>());
        genome.setMaxNeuron(51);

        genome.mutate(pool, sightsense);

        return genome;
    }
    private void loadGenomes()
    {
        Pool pool = new Pool(0, 0, 0, 0, 0, 3, new List<Species>());

        int i = 0;
        while (i < 300)
        {
            if (Time.frameCount % 60 == 0)
            {
                Genome genome = newGenome(pool);
                pool.addToSpecies(genome);
                i++;
            }

        }
        saveNetwork(pool);
        loaded = true;
        hidewindows = false;


    }
    void saveNetwork(Pool pool)
    {
        /*1st method, too slow
        Pool.current.setGeneration(pool.getGeneration());
        Pool.current.setCurrentSpecies(pool.getcurrentspecies());
        Pool.current.setCurrentGenome(pool.getcurrentgenome());
        Pool.current.setMaxFitness(pool.getMaxFitness());
        Pool.current.setInnovation(pool.getInnovation());
        Pool.current.setSpecies(pool.getSpecies());


        //they should be equal already but just in case
        pool = Pool.current;
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        FileStream file = File.Create(Application.persistentDataPath + "/MonsterPool.gd"); //you can call it anything you want
        bf.Serialize(file, pool);
        file.Close();*/

        if (!Directory.Exists("DataFiles"))
        {
            DirectoryInfo DataFilesfolder = Directory.CreateDirectory("DataFiles"); // returns a DirectoryInfo object

        }
        if (!Directory.Exists("DataFiles/IA"))
        {
            DirectoryInfo DataFilesfolder = Directory.CreateDirectory("DataFiles/IA"); // returns a DirectoryInfo object

        }
        if (File.Exists("DataFiles/IA/monsters.xml"))
        {
            var file = File.Create("DataFiles/IA/monsters.xml"); // returns a FileInfo object
            file.Close();
        }


        var serializer = new XmlSerializer(typeof(Pool));
        var stream = new FileStream("DataFiles/IA/monsters.xml", FileMode.Create);
        serializer.Serialize(stream, pool);
        stream.Close();
    }
    public void GoToAdventure()
    {
        if ((loaded || desiredobject.loaded) && !shopping)
        {
            if (threadcreated)
            {
                pleasework.Join();

            }

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
}

public class pleaseWork 
{
    public bool loaded = false;
    public bool hidewindows = true;
    int[,] sightsense;
    public pleaseWork(int[,] SIghtSense)
    {
        sightsense = SIghtSense;
    }
    public void ThreadRun()
    {
        Pool pool = new Pool(0, 0, 0, 0, 0, 3, new List<Species>());
        int i = 0;
        
        
        while (i < 200)
        {
           
                Genome genome = newGenome(pool);
                pool.addToSpecies(genome);
                i++;
            

        }
        saveNetwork(pool);
        loaded = true;
        hidewindows = false;
    }
    public void loadGenomes()
    {
        Pool pool = new Pool(0, 0, 0, 0, 0, 3, new List<Species>());

        int i = 0;
        while (i < 300)
        {
            if (Time.frameCount % 60 == 0)
            {
                Genome genome = newGenome(pool);
                pool.addToSpecies(genome);
                i++;
            }

        }
        lock(this)
        {
            saveNetwork(pool);
            loaded = true;
            hidewindows = false;
        }

    }
    Genome newGenome(Pool pool)
    {

        Genome genome = new Genome(0, new List<Neuron>(), 0, 0, 0.25f, 2.0f, 0.4f, 0.5f, 0.2f, 0.4f, 0.1f, new List<Genes>());
        genome.setMaxNeuron(51);

        genome.mutate(pool, sightsense);

        return genome;
    }
    void saveNetwork(Pool pool)
    {
        /*1st method, too slow
        Pool.current.setGeneration(pool.getGeneration());
        Pool.current.setCurrentSpecies(pool.getcurrentspecies());
        Pool.current.setCurrentGenome(pool.getcurrentgenome());
        Pool.current.setMaxFitness(pool.getMaxFitness());
        Pool.current.setInnovation(pool.getInnovation());
        Pool.current.setSpecies(pool.getSpecies());


        //they should be equal already but just in case
        pool = Pool.current;
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        FileStream file = File.Create(Application.persistentDataPath + "/MonsterPool.gd"); //you can call it anything you want
        bf.Serialize(file, pool);
        file.Close();*/

        if (!Directory.Exists("DataFiles"))
        {
            DirectoryInfo DataFilesfolder = Directory.CreateDirectory("DataFiles"); // returns a DirectoryInfo object

        }
        if (!Directory.Exists("DataFiles/IA"))
        {
            DirectoryInfo DataFilesfolder = Directory.CreateDirectory("DataFiles/IA"); // returns a DirectoryInfo object

        }
        if (File.Exists("DataFiles/IA/monsters.xml"))
        {
            var file = File.Create("DataFiles/IA/monsters.xml"); // returns a FileInfo object
            file.Close();
        }


        var serializer = new XmlSerializer(typeof(Pool));
        var stream = new FileStream("DataFiles/IA/monsters.xml", FileMode.Create);
        serializer.Serialize(stream, pool);
        stream.Close();
    }
}
