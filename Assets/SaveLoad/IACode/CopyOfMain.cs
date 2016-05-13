using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine.SceneManagement;

public class CopyOfMain : MonoBehaviour {
    int Population = 300;
    sightsense sightsense;
    int rightmost = 0;
    Pool pool;
    int timeout = 0;
    int timeoutconstant = 10;
    Hashtable outputs;
    string[] nameOfOutputs;
    Vector3 initialPosition;
    public Transform ending;
    Transform monster;
    public GameObject textObject;
    public GameObject imageObject;

    bool loaded = false;
    int lastframetotal = 0;
    int whentosave = 0;
    monsterdata savedGame;
    bool jumplearned;
    int percentagelearnt;
    // Use this for initialization
    public void Start () {
        monster = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        initialPosition = monster.position;
        outputs = new Hashtable();
        outputs.Add("Jump", false);
        outputs.Add("Left", false);
        outputs.Add("Right", false);
        nameOfOutputs = new string[] { "Jump", "Left", "Right" };
        Canvas thiscanvas = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Canvas>();
        
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
            sightsense.changeMoneyText();
        }
        //loadfile TODO
        //Creating basic pool, if couldn't load

        pool = new Pool(0, 0, 0, 0, 0, 3, new List<Species>());
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
            initializeRun();
            
            loaded = true;

        }
        else
        {
            StartCoroutine(loadGenomes());
        }
        Destroy(textObject);
        Destroy(imageObject);




    }
    public void saveGame()
    {

        monsterdata.current.jumplearned = jumplearned;
        monsterdata.current.money = sightsense.money;
        monsterdata.current.learningjump = percentagelearnt;
        savedGame = monsterdata.current;
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        print(Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); //you can call it anything you want
        bf.Serialize(file, savedGame);
        file.Close();

    
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


    }

    // Update is called once per frame
    void Update () {
        //print(pool.getspecies().Count);
        if (loaded)
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
            
            timeout--;
            float timeoutBonus = (Time.frameCount - lastframetotal) / 4;
            if (timeout + timeoutBonus <= 0 || monster.position.y < ending.position.y)
            {
                int fitness = sightsense.getRightMost() - (Time.frameCount - lastframetotal) / 4;
                if (sightsense.getRightMost() > ending.position.x)
                {
                    //game has ended, so give a better fitness than others
                    fitness += 1000;
                }
                if (fitness == 0)
                {
                    fitness = -1;

                }
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
                            pool.newGeneration(sightsense);
                            if (jumplearned)
                            {
                                saveNetwork();

                            }
                            saveGame();
                            SceneManager.LoadScene("adventure map");
                        }
                    }


                }
                initializeRun();
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
            DirectoryInfo DataFilesfolder = Directory.CreateDirectory( "DataFiles"); // returns a DirectoryInfo object

        }
        if (!Directory.Exists( "DataFiles/IA"))
        {
            DirectoryInfo DataFilesfolder = Directory.CreateDirectory("DataFiles/IA"); // returns a DirectoryInfo object

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
        rightmost = (int) monster.position.x;
        pool.setCurrentFrame(0);
        timeout = timeoutconstant;
        clearOutputs();
        Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        genome.generateNetwork(sightsense.thingsseen.Length);
        evaluateCurrent();

    }
    void clearOutputs()
    {
        outputs = new Hashtable();
        outputs.Add("Jump", false);
        outputs.Add("Left", false);
        outputs.Add("Right", false);
        sightsense.movemonster(outputs);
    }
    void evaluateCurrent()
    {
        Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        int[,] input = sightsense.thingsseen;

        outputs = genome.evaluateNetwork(input, nameOfOutputs);
        if ((bool)outputs["Left"] && (bool)outputs["Right"])
        {
            outputs["Left"] = false;
            outputs["Right"] = false;
        }
        if (!jumplearned)
        {
            outputs["Jump"] = false;

        }
        sightsense.movemonster(outputs);

    }
    Genome newGenome(Pool pool)
    {

        Genome genome = new Genome(0, new List<Neuron>(), 0, 0, 0.25f, 2.0f, 0.4f, 0.5f, 0.2f, 0.4f, 0.1f, new List<Genes>());
        genome.setMaxNeuron(sightsense.thingsseen.GetLength(0) * sightsense.thingsseen.GetLength(1) + 1 );

        genome.mutate(pool, sightsense);

        return genome;
    }

}
