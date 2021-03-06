﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Xml;
using System.Xml.Serialization;
[XmlRoot("Genome")]

public class Genome : IComparable<Genome>
{
    [XmlArray("genomegenes")]
    [XmlArrayItem("genomegene")]
    public List<Genes> genes;
    [XmlAttribute("distancetraveled")]
    public int distancetraveled;
    List<Neuron> network;
    [XmlAttribute("maxneuron")]
    public int maxneuron;
    [XmlAttribute("globalrank")]
    public int globalrank;
    [XmlArray("mutationKeys")]
    [XmlArrayItem("Keys")]
    public List<string> mutationKeys;
    [XmlArray("mutationNumbers")]
    [XmlArrayItem("Numbers")]
    public List<float> mutationNumbers;
    Hashtable mutationrates;
    System.Random randomNumber;
    public Genome (int DistanceTraveled, List<Neuron> Network, int MaxNeuron, int GlobalRank, float MutateConnectionsChance, float LinkMutationChance, float BiasMutationChance, float NodeMutationChance, float EnableMutationChance, float DisableMutationChance, float StepSize, List<Genes> Genes)
    {
        distancetraveled = DistanceTraveled;
        network = Network;
        maxneuron = MaxNeuron;
        globalrank = GlobalRank;
        genes = Genes;
        mutationrates = new Hashtable();
        mutationKeys = new List<string>();
        mutationKeys.Add("connections");
        mutationKeys.Add("link");
        mutationKeys.Add("bias");
        mutationKeys.Add("node");
        mutationKeys.Add("enable");
        mutationKeys.Add("disable");
        mutationKeys.Add("step");
        mutationNumbers = new List<float>();
        mutationNumbers.Add(MutateConnectionsChance);
        mutationNumbers.Add(LinkMutationChance);
        mutationNumbers.Add(BiasMutationChance);
        mutationNumbers.Add(NodeMutationChance);
        mutationNumbers.Add(EnableMutationChance);
        mutationNumbers.Add(DisableMutationChance);
        mutationNumbers.Add(StepSize);

        mutationrates.Add("connections", MutateConnectionsChance);
        mutationrates.Add("link", LinkMutationChance);
        mutationrates.Add("bias", BiasMutationChance);
        mutationrates.Add("node", NodeMutationChance);
        mutationrates.Add("enable", EnableMutationChance);
        mutationrates.Add("disable", DisableMutationChance);
        mutationrates.Add("step", StepSize);
        randomNumber = new System.Random();

    }
    public Genome() : base()
    {
        network = new List<Neuron>();
        genes = new List<Genes>();
        randomNumber = new System.Random();

    }
    public void setMutations()
    {
        mutationKeys = mutationrates.Keys.Cast<string>().ToList();
        mutationNumbers = mutationrates.Values.Cast<float>().ToList();
    }
    public void recreateMutations()
    {
        mutationrates = new Hashtable();

        for (int i = 0; i < mutationKeys.Count; i++)
        {
            mutationrates.Add(mutationKeys[i], mutationNumbers[i]);
        }
    }
    
    public void setRank(int Rank)
    {
        globalrank = Rank;
    }
    public int getRank()
    {
        return globalrank;
    }
    int IComparable<Genome>.CompareTo(Genome other)
    {
        if (distancetraveled < other.getDistanceTraveled())
        {
            return 1;
        }
        if (distancetraveled > other.getDistanceTraveled())
        {
            return -1;
        }
        return 0;
    }

    public void setMutationRates(Hashtable aux)
    {
        mutationrates = aux;
        mutationKeys = mutationrates.Keys.Cast<string>().ToList();
        mutationNumbers = mutationrates.Values.Cast<float>().ToList();
    }

    
    public int getDistanceTraveled()
    {
        return distancetraveled;
    }
    public void setDistanceTraveled(int distance)
    {
        distancetraveled = distance;
    }
    public void setMaxNeuron(int max)
    {
        maxneuron = max;
    }
	public Hashtable getmutationrates()
    {
        return mutationrates;
    }
    public List<Genes> getGenes()
    {
        return genes;
    }
    public void mutate( Pool pool, int[,] sightsense, int numberOfOutputs)
    {
        Hashtable aux = new Hashtable();

        foreach (DictionaryEntry entry in mutationrates)
        {
            aux.Add(entry.Key, entry.Value);
            //if (UnityEngine.Random.Range(1, 3) == 1)
            if (randomNumber.Next(1, 3) == 1)
            {
                aux[entry.Key] = 0.95f * (float)entry.Value;

            }

            else
            {
                aux[entry.Key] = 1.05263f * (float)entry.Value;
            }
        }
        mutationrates = aux;
        setMutations();
        //if (UnityEngine.Random.Range(0f, 1f) < (float)mutationrates["connections"])
        if (randomNumber.NextDouble() < (float)mutationrates["connections"])
        {
            PointMutates();
        }

        for (float i = (float)mutationrates["link"]; i > 0; i--)
        {
           // if (UnityEngine.Random.Range(0f, 1f) < i)
            if (randomNumber.NextDouble() < i)
            {
                LinkMutates(false, sightsense.GetLength(0) * sightsense.GetLength(1) + 1, pool, numberOfOutputs);
            }
        }

        for (float i = (float)mutationrates["bias"]; i > 0; i--)
        {
            //if (UnityEngine.Random.Range(0f, 1f) < i)
            if (randomNumber.NextDouble() < i)
            {
                LinkMutates(true, sightsense.GetLength(0) * sightsense.GetLength(1) + 1, pool, numberOfOutputs);
            }
        }
        for (float i = (float)mutationrates["node"]; i > 0; i--)
        {
            // if (UnityEngine.Random.Range(0f, 1f) < i)
            if (randomNumber.NextDouble() < i)
            {
                NodeMutates(pool);
            }
        }
        for (float i = (float)mutationrates["enable"]; i > 0; i--)
        {
            //if (UnityEngine.Random.Range(0f, 1f) < i)
            if (randomNumber.NextDouble() < i)
            {
                enableDisableMutates(true);
            }
        }
        for (float i = (float)mutationrates["disable"]; i > 0; i--)
        {
            //if (UnityEngine.Random.Range(0f, 1f) < i)
            if (randomNumber.NextDouble() < i)
            {
                enableDisableMutates(false);
            }
        }
        
    }

    void PointMutates()
    {
        float step = (float) mutationrates["step"];
        for (int i = 0; i < genes.Count; i++)
        {
            Genes gene = genes[i];
            //if (UnityEngine.Random.value < 0.9f)
            if (randomNumber.NextDouble() < 0.9f)

           // gene.setweight(gene.getweight() + UnityEngine.Random.value * step * 2 - step);
            gene.setweight(gene.getweight() + ((float)randomNumber.NextDouble()) * step * 2 - step);

            else
                //gene.setweight(UnityEngine.Random.value * 4 - 2);
                gene.setweight(((float)randomNumber.NextDouble()) * 4 - 2);

        }
    }
    void LinkMutates(bool forceBias, int inputs, Pool pool, int numberOfOutputs)
    {
        int neuron1 = RandomNeurons(false, inputs, numberOfOutputs);
        int neuron2 = RandomNeurons(true, inputs, numberOfOutputs);

        Genes newLink = new Genes(0,0,0.0f,true, 0);
        if (neuron1 <= inputs && neuron2 <= inputs)
        {
            //Both are input nodes
            return;
        }
        if (neuron2 <= inputs)
        {
            //swap output neuron for the input
            int aux = neuron1;
            neuron1 = neuron2;
            neuron2 = aux;
        }
        newLink.setOut(neuron2);
        if (forceBias)
        {
            newLink.setInto(inputs);

        }
        else
        {
            newLink.setInto(neuron1);
        }
        if (!containsLink(newLink))
        {
            newLink.setInnovation(pool.newInnovation());
            //newLink.setweight(UnityEngine.Random.value * 4 - 2);
            newLink.setweight(((float) randomNumber.NextDouble()) * 4 - 2);

            genes.Add(newLink);
        }
    }
    bool containsLink(Genes newLink)
    {
        
        for (int i = 0; i < genes.Count; i++)
        {
            if (genes[i].getInto() == newLink.getInto() && genes[i].getOut() == newLink.getOut())
            {
                return true;
            }
        }
        return false;
    }
    public int RandomNeurons(bool sentence, int inputs, int numberOfOutputs)
    {

        bool[] neurons = new bool[1000000 + numberOfOutputs];
        if (!sentence)
        {
            for (int i = 0; i < inputs; i++)
            {
                neurons[i] = true;
            }
            
        }
        for (int i = 0; i < genes.Count; i++)
        {
            if (!sentence || genes[i].getInto() > inputs)
            {
                neurons[genes[i].getInto()] = true;
            }
            if (!sentence || genes[i].getOut() > inputs)
            {
                neurons[genes[i].getOut()] = true;
            }
        }
        for (int i = 0; i < numberOfOutputs; i++)
        {
            neurons[1000000 + i] = true;
        }
        int count = 0;
        for (int i = 0; i < neurons.Length; i++)
        {
            if (neurons[i] == true)
            {
                count++;
            }
        }

        //int n = UnityEngine.Random.Range(1, count+1);
        int n = randomNumber.Next(1, count+1);


        for (int i = 0; i < neurons.Length; i++)
        {
            if (neurons[i] == true)
            {
                n--;
                if (n == 0)
                {
                    return i;
                }
            }
        }
        
        return 0;
    }

    void NodeMutates(Pool pool)
    {
        if (genes.Count !=0)
        {
            maxneuron++;
            //Genes gene = genes[UnityEngine.Random.Range(0, genes.Count)];
            Genes gene = genes[randomNumber.Next(0, genes.Count)];
            if (gene.getenabled())
            {
                gene.setenabled(false);
                Genes gene1 = new Genes(gene.getInto(), gene.getOut(), gene.getweight(), gene.getenabled(), gene.getInnovation());
                gene1.setOut(maxneuron);
                gene1.setweight(1.0f);
                gene1.setInnovation(pool.newInnovation());
                gene1.setenabled(true);
                
                

                Genes gene2 = new Genes(gene.getInto(),gene.getOut(),gene.getweight(),gene.getenabled(), gene.getInnovation());
                gene2.setInto(maxneuron);
                gene2.setInnovation(pool.newInnovation());
                gene2.setenabled(true);
                genes.Add(gene1);
                genes.Add(gene2);

            }
        }
    }
    void enableDisableMutates(bool enabled)
    {
        List<Genes> listgenes = new List<Genes>();
        for (int i = 0; i < genes.Count; i++)
        {
            if (genes[i].getenabled() != enabled )
            {
                listgenes.Add(genes[i]);
            }
        }
        if (listgenes.Count != 0)
        {
            //Genes aux = listgenes[UnityEngine.Random.Range(0, listgenes.Count)];
            Genes aux = listgenes[randomNumber.Next(0, listgenes.Count)];
            aux.setenabled(!aux.getenabled());
        }
    }
    public void generateNetwork(int inputs, int numberOutputs)
    {
        List<Neuron> Net = new List<Neuron>();
        //3 is always the number of Outputs for the moment, left, right and jump 
        for (int i = 0; i < inputs; i++)
        {
            Net.Add(new Neuron(new List<Genes>(), 0.0f));
        }
        Neuron empty = null;
        Net.AddRange(Enumerable.Repeat(empty, 1000000- Net.Count));
        
        for (int i = 0; i < numberOutputs; i++)
        {
            //max number of possible nodes
            Net.Add(new Neuron(new List<Genes>(), 0.0f));
        }
        genes.Sort();
        genes.Reverse();

        for (int i = 0; i < genes.Count; i++)
        {
            if (genes[i].getenabled())
            {
                if (Net[genes[i].getOut()] == null)
                {
                    Net[genes[i].getOut()] = new Neuron(new List<Genes>(), 0.0f);

                }
                Net[genes[i].getOut()].addIncomingGene(genes[i]);
                if (Net[genes[i].getInto()] == null)
                {
                    Net[genes[i].getInto()] = new Neuron(new List<Genes>(), 0.0f);

                }
            }
        }

        network = Net;
        
    }

    public Hashtable evaluateNetwork(int[,] inputs, string[] numberOfOutputs)
    {
        int count = 0;
        for (int i = 0; i < inputs.GetLength(0); i++)
        {
            for (int j = 0; j < inputs.GetLength(1); j++)
            {
                if (network[i+count] != null)
                {
                    network[i + count].setValue(inputs[i, j]);
                    count++;
                }
                
            }
            
        }

        foreach (Neuron neuron in network)
        {
            float sum = 0;
            if (neuron != null)
            {
                for (int i = 0; i < neuron.getInto().Count; i++)
                {
                    Neuron aux = network[neuron.getInto()[i].getInto()];
                    sum += neuron.getInto()[i].getweight() * aux.getValue();
                }
                if (neuron.getInto().Count > 0)
                {
                    neuron.setValue(sigmoid(sum));
                }
            }
            
        }

        Hashtable outputs = new Hashtable();
        for (int i = 0; i < numberOfOutputs.Length; i++)
        {
            string button = numberOfOutputs[i];
            //100000 it's max number of neurons
            Neuron neuron = network[1000000 + i];
            if (neuron.getValue() > 0)
            {
                outputs.Add(button, true);
            }
            else
            {
                outputs.Add(button, false);
            }
        }
        return outputs;
    }

    float sigmoid(float sum)
    {
        return 2.0f / (1.0f + Mathf.Exp(-4.9f * sum)) - 1.0f;
    }

    public Genome crossover(Genome b, Pool pool, sightsense sightsense, int numberOfOutputs)
    {
        Genome a = this;
        if (b.getDistanceTraveled() > a.distancetraveled)
        {
            Genome aux = a;
            a = b;
            b = aux;
           
        }
        Genome child = newGenome(pool, sightsense.thingsseen, numberOfOutputs);
        List<Genes> innovations = new List<Genes>();

        Genes auxgene = null;
        for (int i = 0; i < b.getGenes().Count; i++)
        {
            if (innovations.Count <= b.getGenes()[i].getInnovation())
            {
                for (int j = 0; j < b.getGenes()[i].getInnovation(); j++)
                {
                    innovations.Add(auxgene);
                }
                innovations.Add(b.getGenes()[i]);
            }
            else
            {
                innovations[b.getGenes()[i].getInnovation()] = b.getGenes()[i];

            }
        }

        for (int i = 0; i < a.getGenes().Count; i++)
        {
            Genes GeneWild = a.getGenes()[i];

            if (GeneWild.getInnovation() < innovations.Count)
            {
                Genes GeneWilder = innovations[GeneWild.getInnovation()];
                if (GeneWilder != null && randomNumber.Next(1, 3) == 1 && GeneWilder.getenabled())
                {
                    child.getGenes().Add(GeneWilder.copyGene());

                }
                else
                {
                    child.getGenes().Add(GeneWild.copyGene());

                }
            
            }
            else
            {
                child.getGenes().Add(GeneWild.copyGene());
            }

        }
        child.setMaxNeuron(Mathf.Max(a.maxneuron, b.maxneuron));
        foreach (DictionaryEntry entry in a.getmutationrates())
        {
            child.getmutationrates()[entry.Key] = entry.Value;
        }
        return child;
    }
    Genome newGenome(Pool pool, int[,] sightsense, int numberOfOutputs)
    {

        Genome genome = new Genome(0, new List<Neuron>(), 0, 0, 0.25f, 2.0f, 0.4f, 0.5f, 0.2f, 0.4f, 0.1f, new List<Genes>());
        genome.setMaxNeuron(sightsense.GetLength(0) * sightsense.GetLength(1));

        genome.mutate(pool, sightsense, numberOfOutputs);

        return genome;
    }
    public Genome copyGenome()
    {
        Genome aux = new Genome(0, new List<Neuron>(), maxneuron, 0,(float) mutationrates["connections"], (float)mutationrates["link"], (float)mutationrates["bias"], (float)mutationrates["node"], (float)mutationrates["enable"], (float)mutationrates["disable"], 0.1f, new List<Genes>());
        for (int i = 0; i < genes.Count; i++)
        {
            aux.genes.Add(genes[i]);
        }
        return aux;
    }
}
