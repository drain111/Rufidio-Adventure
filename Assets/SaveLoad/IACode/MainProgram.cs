using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainProgram : MonoBehaviour{
    int Population = 300;
    sightsense sightsense;
    int rightmost = 0;
    Pool pool;
    int timeout = 0;
    int timeoutconstant = 3 * 60;
    Hashtable outputs;
    string[] nameOfOutputs;
    bool initialized = false;
	// Use this for initialization
	void Start () {
        
        outputs = new Hashtable();
        outputs.Add("Jump", false);
        outputs.Add("Left", false);
        outputs.Add("Right", false);
        nameOfOutputs = new string[] { "Jump", "Left", "Right"};
        
        //Creating Basic Genome
        //Genome genome = newGenome();
        
        
        //genome.setMaxNeuron(sightsense.i * sightsense.j + 1);
        //genome = mutate(genome, pool);
        
	}
	
	// Update is called once per frame
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
            Genome genome = pool.getspecies()[pool.getcurrentspecies()].getGenomes()[pool.getcurrentgenome()];
            if (Time.frameCount % 5 == 0)
            {
                evaluateCurrent();
            }
            if (sightsense.checkDistance(rightmost))
            {
                timeout = timeoutconstant;
            }
            timeout--;
        }
	}
    void initializeRun()
    {
        //put monster at the beggining TODO
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
        if ((bool) controller["Left"] && (bool) controller["Right"]) 
        {
            controller["Left"] = false;
            controller["Right"] = true;
        }
        sightsense.movemonster(controller);

    }
    Genome newGenome(Pool pool)
    {
        
        Genome genome = new Genome(0, 0, new List<Neuron>(), 0, 0, 0.25f, 2.0f, 0.4f, 0.5f, 0.2f, 0.4f, 0.1f, new List<Genes>());
        genome.setMaxNeuron(3);

        mutate(genome, pool);

        return genome;
    }
    Genome mutate(Genome genome, Pool pool)
    {
        Hashtable aux = new Hashtable();
        
        foreach(DictionaryEntry entry in genome.getmutationrates())
        {
            aux.Add(entry.Key, entry.Value);
            if (Random.Range(1,2) == 1)
            {
                aux[entry.Key] = 0.95f * (float) entry.Value;

            }

            else
            {
                aux[entry.Key] = 1.05263f * (float ) entry.Value;
            }
        }
        genome.setMutationRates(aux);
        
        if (Random.Range(0f,1f) < (float) genome.getmutationrates()["connections"])
        {
            genome.PointMutates();
        }

        for (float i = (float)genome.getmutationrates()["link"]; i > 0; i--)
        {
            if (Random.Range(0f, 1f) < i)
            {
                genome.LinkMutates(false, sightsense.i * sightsense.j + 1, pool);
            }
        }

        for (float i = (float)genome.getmutationrates()["bias"]; i > 0; i--)
        {
            if (Random.Range(0f, 1f) < i)
            {
                genome.LinkMutates(false, sightsense.i * sightsense.j + 1, pool);
            }
        }
        for (float i = (float)genome.getmutationrates()["node"]; i > 0; i--)
        {
            if (Random.Range(0f, 1f) < i)
            {
                genome.NodeMutates(pool);
            }
        }
        for (float i = (float)genome.getmutationrates()["enable"]; i > 0; i--)
        {
            if (Random.Range(0f, 1f) < i)
            {
                genome.enableDisableMutates(true);
            }
        }
        for (float i = (float)genome.getmutationrates()["disable"]; i > 0; i--)
        {
            if (Random.Range(0f, 1f) < i)
            {
                genome.enableDisableMutates(false);
            }
        }
        return genome;
    }
  
}
