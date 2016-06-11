using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;
using System;
using System.Threading;
public class CopyOfMain : MonoBehaviour {
    int Population = 200;
    sightsense sightsense;
    Rect windowRect ;

    Pool pool;
    int timeout = 0;
    int timeoutconstant = 5;
    Hashtable outputs;
    string[] nameOfOutputs;
    Vector3 initialPosition;
    public Transform ending;
    Transform monster;
    public GameObject textObject;
    public GameObject imageObject;
    int bedcost;
    int foodcost;
    int foodlevel;
    int bedlevel;
    bool loaded = false;
    int lastframetotal = 0;
    int whentosave = 0;
    monsterdata savedGame;
    bool jumplearned;
    int percentagelearnt;
    bool dontsave = false;
    bool[] feelingsarray;
    int moneycost;
    bool witchisthere = false;
    bool threadStarted = false;
    int tutorial;
    Thread newThread;
    giveThreadつo_oつ giveThreadつo_oつ;
    // Use this for initialization
    public void Start () {
        monster = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        initialPosition = monster.position;
        outputs = new Hashtable();
        outputs.Add("Jump", false);
        outputs.Add("Left", false);
        outputs.Add("Right", false);
        feelingsarray = new bool[100];
        windowRect = new Rect(Screen.currentResolution.width / 100, Screen.currentResolution.height / 100, Screen.currentResolution.width * 98 / 100, Screen.currentResolution.height * 98 / 100);
        sightsense = GameObject.FindGameObjectWithTag("Player").GetComponent<sightsense>();
        sightsense.setRightMost((int) monster.position.x);
        monsterdata.current = new monsterdata();

        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            savedGame = (monsterdata)bf.Deserialize(file);
            file.Close();
            jumplearned = savedGame.jumplearned;
            sightsense.money = savedGame.money;
            percentagelearnt = savedGame.learningjump;
            sightsense.hungry = savedGame.hungry;
            sightsense.sleep = savedGame.sleep;
            sightsense.changeMoneyText();
            bedlevel = savedGame.bedlevel;
            foodlevel = savedGame.foodlevel;
            bedcost = savedGame.bedprice;
            foodcost = savedGame.foodprice;
            moneycost = savedGame.moneycost;
            sightsense.coinlevel = savedGame.coinlevel;
            sightsense.witchlevel = savedGame.witchlevel;
            witchisthere = savedGame.withcisthere;
            tutorial = savedGame.tutorial;

        }
        if (witchisthere)
        {
            nameOfOutputs = new string[] { "Jump", "Left", "Right", "Attack" };
            outputs.Add("Attack", false);
        }
        else
        {
            nameOfOutputs = new string[] { "Jump", "Left", "Right"};

        }

        for (int i = 0; i < 100; i++)
        {
            feelingsarray[i] = true;
        }
        //loadfile TODO
        //Creating basic pool, if couldn't load

        pool = new Pool(0, 0, 0, 0, 0, nameOfOutputs.Length, new List<Species>());
        /* write to file 1st try, too slow
        if (File.Exists(Application.persistentDataPath + "/MonsterPool.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/MonsterPool.gd", FileMode.Open);
            pool = (Pool)bf.Deserialize(file);
            file.Close();
            loaded = true;
        }*/
        Application.targetFrameRate = -1;
        if(File.Exists( "DataFiles/IA/monsters.xml"))
        {
            var serializer = new XmlSerializer(typeof(Pool));
            var stream = new FileStream("DataFiles/IA/monsters.xml", FileMode.Open);
            pool = (Pool) serializer.Deserialize(stream);
            List<Species> species = pool.getSpecies();
            for (int i = 0; i < species.Count; i++)
            {
                List<Genome> genomes = species[i].getGenomes();
                for (int j = 0; j < genomes.Count; j++)
                {
                    genomes[j].recreateMutations();
                }
            }
            stream.Close();

            if (pool.getSpecies().Count <= pool.getcurrentspecies() || pool.getSpecies()[pool.getcurrentspecies()].getGenomes().Count <= pool.getcurrentgenome() )
            {
                UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
                SceneManager.LoadScene("Loading Screen");
                giveThreadつo_oつ = new giveThreadつo_oつ(pool, sightsense, nameOfOutputs);
                newThread = new Thread(new ThreadStart(giveThreadつo_oつ.ThreadRun));
                threadStarted = true;

                try
                {
                    newThread.Start();
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
                initializeRun();

            }

            loaded = true;
            Destroy(textObject);
            Destroy(imageObject);

        }
        else
        {
            StartCoroutine(loadGenomes());
        }




    }
    public void saveGame()
    {

        monsterdata.current.jumplearned = jumplearned;
        monsterdata.current.money = sightsense.money;
        monsterdata.current.learningjump = percentagelearnt;
        monsterdata.current.hungry = sightsense.hungry;
        monsterdata.current.sleep = sightsense.sleep;
        monsterdata.current.bedlevel = bedlevel;
        monsterdata.current.bedprice = bedcost;
        monsterdata.current.foodlevel = foodlevel;
        monsterdata.current.foodprice = foodcost;
        monsterdata.current.coinlevel = sightsense.coinlevel;
        monsterdata.current.moneycost = moneycost;
        monsterdata.current.witchlevel = sightsense.witchlevel;
        monsterdata.current.withcisthere = witchisthere;
        monsterdata.current.tutorial = tutorial;

        savedGame = monsterdata.current;
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        print(Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
        bf.Serialize(file, savedGame);
        file.Close();

    
    }
    void OnGUI()
    {
        if (sightsense.finished)
        {
            windowRect = GUI.Window(0, windowRect, DoMyWindow, "My Window");

        }
    }
    void DoMyWindow(int id)
    {

        GUI.TextField(new Rect(windowRect.width / 8, windowRect.height / 8, windowRect.width * 7 / 8, windowRect.height * 7 / 8), "You Won");
        GUI.skin.textField.fontSize = 100;
    }
    public void goHome()
    {
        saveGame();
        SceneManager.LoadScene("house scene");

    }
    private IEnumerator loadGenomes()
    {
        Text text = textObject.GetComponent<Text>();
        for (int i = 0; i < Population; i++)
        {
            int percentage = i * 100 / 300;
            text.text = "percentage " + percentage.ToString();
            Genome genome = newGenome(pool);
            pool.addToSpecies(genome);
            yield return null;

        }
        
        initializeRun();
        loaded = true;
        Destroy(textObject);
        Destroy(imageObject);


    }

    // Tying the gameplay to the frames wasn't a good idea, please use fixedupdate
    void FixedUpdate () {
        //print(pool.getspecies().Count);
        if (loaded && threadStarted == false )
        {
            Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
            if (Time.frameCount % 5 == 0)
            {
                evaluateCurrent();
            }
            sightsense.movemonster(outputs);
            if (sightsense.checkDistance())
            {
                timeout = timeoutconstant;
            }
            
            if (dontsave)
                initializeRun();
            timeout--;
            float timeoutBonus = (Time.frameCount - lastframetotal) / 4;
            if (timeout + timeoutBonus <= 0 || monster.position.y < ending.position.y || sightsense.crashedWithDragon || (sightsense.finished && Time.time % 5 == 0))
            {
                sightsense.crashedWithDragon = false;
                int fitness = (int) sightsense.getRightMost() - (int) initialPosition.x;
                if (sightsense.getRightMost() > ending.position.x)
                {
                    //game has ended, so give a better fitness than others
                    fitness += 1000;
                    
                    sightsense.money += 1000 * (int)Mathf.Pow(2, sightsense.coinlevel);
                            
                }
                if (fitness == 0)
                {
                    fitness = -1;

                }
                sightsense.finished = false;
                    genome.setDistanceTraveled(fitness);
                    if (fitness > pool.getMaxFitness())
                    {
                        pool.setMaxFitness(fitness);
                        //saveNetwork();
                    }
                    pool.setCurrentGenome(0);
                    pool.setCurrentSpecies(0);
                
                
                
                while (pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()].getDistanceTraveled() != 0)   
                {
                    pool.setCurrentGenome(pool.getcurrentgenome() + 1);
                    if (pool.getcurrentgenome() >= pool.getSpecies()[pool.getcurrentspecies()].getGenomes().Count)
                    {
                        pool.setCurrentGenome(0);
                        pool.setCurrentSpecies(pool.getcurrentspecies() + 1);
                        if (pool.getcurrentspecies() >= pool.getSpecies().Count)
                        {
                            saveGame();

                            UnityEngine.Object.DontDestroyOnLoad(this.gameObject);
                            SceneManager.LoadScene("Loading Screen");
                            giveThreadつo_oつ = new giveThreadつo_oつ(pool, sightsense, nameOfOutputs);
                            newThread = new Thread(new ThreadStart(giveThreadつo_oつ.ThreadRun));
                            threadStarted = true;

                            try
                            {
                                newThread.Start();
                                break;
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
                    }


                }
                if (threadStarted == false)
                {
                    initializeRun();
                }


                whentosave++;
                
                
                if (whentosave == 50)
                {
                    whentosave = 0;
                    if (jumplearned)
                    {
                        saveNetwork();

                    }

                }

            }

           
       }
        if (threadStarted == true)
        {
            if (giveThreadつo_oつ.giveFinishedつo_oつ())
            {
                if (jumplearned)
                {
                    saveNetwork();

                }
                SceneManager.LoadScene("adventure map");
                Destroy(this.gameObject);
            }
           
        }
        dontsave = false;

    }
    void saveNetwork()
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
            Directory.CreateDirectory( "DataFiles"); // returns a DirectoryInfo object

        }
        if (!Directory.Exists( "DataFiles/IA"))
        {
            Directory.CreateDirectory("DataFiles/IA"); // returns a DirectoryInfo object

        }
        if (File.Exists( "DataFiles/IA/monsters.xml"))
        {
            var file = File.Create( "DataFiles/IA/monsters.xml"); // returns a FileInfo object
            file.Close();
        }

            
        var serializer = new XmlSerializer(typeof(Pool));
        var stream = new FileStream( "DataFiles/IA/monsters.xml", FileMode.Create);
        serializer.Serialize(stream, pool);
        stream.Close();
    }
    void initializeRun()
    {
        lastframetotal = Time.frameCount;
        monster.position = initialPosition;
        sightsense.setRightMost((int) monster.position.x);

        pool.setCurrentFrame(0);
        timeout = timeoutconstant;
        clearOutputs();
        Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        genome.generateNetwork(sightsense.thingsseen.Length, nameOfOutputs.Length);
        evaluateCurrent();

    }
    void clearOutputs()
    {
        outputs = new Hashtable();
        outputs.Add("Jump", false);
        outputs.Add("Left", false);
        outputs.Add("Right", false);
        if (witchisthere)
        {
           outputs.Add("Attack", false);
        }
        sightsense.movemonster(outputs);
    }
    void evaluateCurrent()
    {
        Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        int[,] input = sightsense.thingsseen;

        outputs = genome.evaluateNetwork(input, nameOfOutputs);
        int falsenumbers = 100 - /*totalfalsenumbers -*/ (sightsense.hungry + sightsense.sleep) / 2;
        //totalfalsenumbers = falsenumbers;
        for (int i = 0; i < falsenumbers; i++)
        {
            feelingsarray[i] = false;
        }
        if ((bool)outputs["Left"] && (bool)outputs["Right"])
        {
            
            outputs["Left"] = false;
            outputs["Right"] = false;
        }
        if (!jumplearned)
        {
            outputs["Jump"] = false;

        }
        if (sightsense.witchlevel < 1000)
        {
            outputs["Attack"] = false;
        }
        if (feelingsarray[UnityEngine.Random.Range(0, feelingsarray.Length)] == false && falsenumbers > 50)
        {
            outputs["Left"] = false;
            outputs["Right"] = false;
            outputs["Jump"] = false;

            dontsave = true;
            

        }
        sightsense.movemonster(outputs);

    }
    Genome newGenome(Pool pool)
    {

        Genome genome = new Genome(0, new List<Neuron>(), 0, 0, 0.25f, 2.0f, 0.4f, 0.5f, 0.2f, 0.4f, 0.1f, new List<Genes>());
        genome.setMaxNeuron(sightsense.thingsseen.GetLength(0) * sightsense.thingsseen.GetLength(1) + 1 );

        genome.mutate(pool, sightsense.thingsseen, nameOfOutputs.Length);

        return genome;
    }

}

public class giveThreadつo_oつ
{
    Pool pool;
    sightsense sightsense;
    string[] nameOfOutputs;
    bool finished = false;
    public giveThreadつo_oつ(Pool Pool, sightsense Sightsense, string[] NameOfOutputs)
    {
        pool = Pool;
        sightsense = Sightsense;
        nameOfOutputs = NameOfOutputs;
    }
    public void ThreadRun()
    {
        pool.newGeneration(sightsense, nameOfOutputs.Length);
        finished = true;
    }
    public Pool givePoolつo_oつ()
    {
        return pool;
    }
    public sightsense giveSightsenseつo_oつ()
    {
        return sightsense;
    }
    public bool giveFinishedつo_oつ()
    {
        return finished;
    }
    public string[] giveDiretideつo_oつ()
    {
        return nameOfOutputs;
    }
}
