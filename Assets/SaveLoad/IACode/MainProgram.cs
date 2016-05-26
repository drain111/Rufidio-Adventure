using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainProgram : MonoBehaviour{
    int Population = 300;
    sightsense sightsense;
    int rightmost = 0;
    Pool pool;
    int timeout = 0;
    int timeoutconstant = 5;
    Hashtable outputs;
    string[] nameOfOutputs;
    bool initialized = false;
    public Transform ending;

    // Use this for initialization
    void Start() {

        outputs = new Hashtable();
        outputs.Add("Jump", false);
        outputs.Add("Left", false);
        outputs.Add("Right", false);
        nameOfOutputs = new string[] { "Jump", "Left", "Right"};
        initialized = false;

        //Creating Basic Genome
        //Genome genome = newGenome();


        //genome.setMaxNeuron(sightsense.i * sightsense.j + 1);
        //genome = mutate(genome, pool);

    }

    // Update is called once per frame     line 35
    void Update () {
        if (!initialized)
        {
            //things seen
            sightsense = GameObject.FindGameObjectWithTag("Player").GetComponent<sightsense>();
            //loadfile TODO

            //Creating basic pool, if couldn't load

            pool = new Pool(Population, 0, 0, 0, 0, 3, new List<Species>());
            for (int i = 0; i < Population; i++)
            {
                Genome genome = newGenome(pool);
                pool.addToSpecies(genome);
            }

            initializeRun();
            initialized = true;
        }
        else
        {
            Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
            if (Time.frameCount % 5 == 0)
            {
                evaluateCurrent();
            }
            if (sightsense.checkDistance())
            {
                timeout = timeoutconstant;
            }
            timeout--;
            float timeoutBonus = Time.frameCount / 4;
            if (timeout + timeoutBonus <= 0)
            {
                int fitness = sightsense.getRightMost() - Time.frameCount / 4;
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
                while (pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()].getDistanceTraveled() != 0)   
                {
                    pool.setCurrentGenome(pool.getcurrentgenome() + 1);
                    if (pool.getcurrentgenome() > pool.getSpecies()[pool.getcurrentspecies()].getGenomes().Count)
                    {
                        pool.setCurrentGenome(0);
                        pool.setCurrentSpecies(pool.getcurrentspecies() + 1);
                        if (pool.getcurrentspecies() > pool.getSpecies().Count)
                        {
                            pool.newGeneration(sightsense);
                        }
                    }



                }
                initializeRun();


            }
            int measured = 0;
            int total = 0;
            foreach (Species specie in pool.getSpecies())
            {
                foreach (Genome geno in specie.getGenomes())
                {
                    total++;
                    if(geno.getDistanceTraveled() != 0)
                    {
                        measured++;
                    }
                }
            }
        }
	}
    void initializeRun()
    {
        //put monster at the beggining TODO
        rightmost = 0;
        pool.setCurrentFrame(0);
        timeout = timeoutconstant;
        Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        genome.generateNetwork(sightsense.thingsseen.Length);
        evaluateCurrent();
        
    }
    void evaluateCurrent()
    {
        Genome genome = pool.getSpecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
        int[,] input = sightsense.thingsseen;
        
        Hashtable controller = genome.evaluateNetwork(input, nameOfOutputs);
        if ((bool) controller["Left"] && (bool) controller["Right"]) 
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

        genome.mutate(pool, sightsense.thingsseen);

        return genome;
    }
    
  
}
