using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CopyOfMain : MonoBehaviour {
    int Population = 300;
    sightsense sightsense;
    int rightmost = 0;
    Pool pool;
    int timeout = 0;
    int timeoutconstant = 5;
    Hashtable outputs;
    string[] nameOfOutputs;
    Vector3 initialPosition;
    public Transform ending;
    Transform monster;
    public GameObject textObject;
    bool loaded = false;
    int lastframetotal = 0;
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

        //loadfile TODO
        //Creating basic pool, if couldn't load

        sightsense = GameObject.FindGameObjectWithTag("Player").GetComponent<sightsense>();
        pool = new Pool(Population, 0, 0, 0, 0, 3, new List<Species>());
        StartCoroutine(loadGenomes());
        

        
        

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
            Genome genome = pool.getspecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
            if (Time.frameCount % 5 == 0)
            {
                evaluateCurrent();
            }
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
                }//line 75
                if (fitness == 0)
                {
                    fitness = -1;

                }
                genome.setDistanceTraveled(fitness);
                if (fitness > pool.getMaxFitness())
                {
                    pool.setMaxFitness(fitness);
                    //savenetwork
                }
                pool.setCurrentGenome(0);
                pool.setCurrentSpecies(0);
                while (pool.getspecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()].getDistanceTraveled() != 0)   //line 89
                {
                    pool.setCurrentGenome(pool.getcurrentgenome() + 1);
                    if (pool.getcurrentgenome() >= pool.getspecies()[pool.getcurrentspecies()].getGenomes().Count)
                    {
                        pool.setCurrentGenome(0);
                        pool.setCurrentSpecies(pool.getcurrentspecies() + 1);
                        if (pool.getcurrentspecies() >= pool.getspecies().Count)
                        {
                            pool.newGeneration(sightsense);
                        }
                    }



                }
                initializeRun();


            }
            int measured = 0;
            int total = 0;
            foreach (Species specie in pool.getspecies())
            {
                foreach (Genome geno in specie.getGenomes())
                {
                    total++;
                    if (geno.getDistanceTraveled() != 0)
                    {
                        measured++;
                    }
                }
            }
       }
            
        
    }

    void initializeRun()
    {
        lastframetotal = Time.frameCount;
        monster.position = initialPosition;
        sightsense.setRightMost(0);
        rightmost = 0;
        pool.setCurrentFrame(0);
        timeout = timeoutconstant;
        Genome genome = pool.getspecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        genome.generateNetwork(sightsense.thingsseen.Length);
        evaluateCurrent();

    }
    void evaluateCurrent()
    {
        Genome genome = pool.getspecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        int[,] input = sightsense.thingsseen;

        Hashtable controller = genome.evaluateNetwork(input, nameOfOutputs);
        if ((bool)controller["Left"] && (bool)controller["Right"])
        {
            controller["Left"] = false;
            controller["Right"] = true;
        }
        sightsense.movemonster(controller);

    }
    Genome newGenome(Pool pool)
    {

        Genome genome = new Genome(0, new List<Neuron>(), 0, 0, 0.25f, 2.0f, 0.4f, 0.5f, 0.2f, 0.4f, 0.1f, new List<Genes>());
        genome.setMaxNeuron(3);

        genome.mutate(pool, sightsense);

        return genome;
    }

}
