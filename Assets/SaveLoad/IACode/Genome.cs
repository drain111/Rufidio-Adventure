using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Genome {
    List<Genes> genes;
    int distancetraveled;
    int adjusteddistancetraveled;
    List<Neuron> network;
    int maxneuron;
    int globalrank;
    Hashtable mutationrates;    
    public Genome (int DistanceTraveled, int AdjustedDistanceTraveled,List<Neuron> Network, int MaxNeuron, int GlobalRank, float MutateConnectionsChance, float LinkMutationChance, float BiasMutationChance, float NodeMutationChance, float EnableMutationChance, float DisableMutationChance, float StepSize, List<Genes> Genes)
    {
        distancetraveled = DistanceTraveled;
        adjusteddistancetraveled = AdjustedDistanceTraveled;
        network = Network;
        maxneuron = MaxNeuron;
        globalrank = GlobalRank;
        genes = Genes;
        mutationrates = new Hashtable();
        mutationrates.Add("connections", MutateConnectionsChance);
        mutationrates.Add("link", LinkMutationChance);
        mutationrates.Add("bias", BiasMutationChance);
        mutationrates.Add("node", NodeMutationChance);
        mutationrates.Add("enable", EnableMutationChance);
        mutationrates.Add("disable", DisableMutationChance);
        mutationrates.Add("step", StepSize);

    }
    public void setMutationRates(Hashtable aux)
    {
        mutationrates = aux;
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
    public void PointMutates()
    {
        float step = (float) mutationrates["step"];
        for (int i = 0; i < genes.Count; i++)
        {
            Genes gene = genes[i];
            if (Random.Range(0, 1) < 0.9)
                gene.setweight(gene.getweight() + Random.Range(0, 1) * step * 2 - step);
            else
                gene.setweight(Random.Range(0, 1) * 4 - 2);
        }
    }
    public void LinkMutates(bool forceBias, int inputs, Pool pool)
    {
        int neuron1 = RandomNeurons(false, inputs);
        int neuron2 = RandomNeurons(true, inputs);

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
            newLink.setweight(Random.value * 4 - 2);
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
    public int RandomNeurons(bool sentence, int inputs)
    {

        bool[] neurons = new bool[1000000 + 3];
        if (!sentence)
        {
            for (int i = 0; i < inputs; i++)
            {
                neurons[i] = true;
            }
            for (int i = 0; i < genes.Count; i++)
            {
                if (genes[i].getInto() > inputs)
                {
                    neurons[genes[i].getInto()] = true;
                }
                if (genes[i].getOut() > inputs)
                {
                    neurons[genes[i].getOut()] = true;
                }
            }
        }
        for (int i = 0; i < 3; i++)
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

        int n = Random.Range(1, count);

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

    public void NodeMutates(Pool pool)
    {
        if (genes.Count !=0)
        {
            maxneuron++;
            Genes gene = genes[Random.Range(0, genes.Count)];
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
    public void enableDisableMutates(bool enabled)
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
            listgenes[Random.Range(0, listgenes.Count)].setenabled(!enabled);
        }
    }
    public void generateNetwork(int inputs)
    {

        //3 is always the number of inputs for the moment, left, right and jump 
        for (int i = 0; i < inputs; i++)
        {
            network.Add(new Neuron(new List<Genes>(), 0.0f));
        }
        Neuron empty = null;
        network.AddRange(Enumerable.Repeat(empty, 1000000- network.Count));
        for (int i = 0; i < 3; i++)
        {
            //max number of possible nodes
            network.Add(new Neuron(new List<Genes>(), 0.0f));
        }
        genes.Sort();
        for (int i = 0; i < genes.Count; i++)
        {
            if (genes[i].getenabled())
            {
                if (network[genes[i].getOut()] == null)
                {
                    network[genes[i].getOut()] = new Neuron(new List<Genes>(), 0.0f);

                }
                network[genes[i].getOut()].addIncomingGene(genes[i]);
                if (network[genes[i].getInto()] == null)
                {
                    network[genes[i].getInto()] = new Neuron(new List<Genes>(), 0.0f);

                }
            }
        }
    }

    public Hashtable evaluateNetwork(int[,] inputs, string[] numberOfOutputs)
    {
        int count = 0;
        for (int i = 0; i < inputs.Rank; i++)
        {
            for (int j = 0; j < inputs.GetLength(i); j++)
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
                    sum += neuron.getInto()[i].getweight() + aux.getValue();
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
            if (network[1000000 + i].getValue() > 0)
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
        return 2 / (1 + Mathf.Exp(-4.9f * sum)) - 1;
    }
}
