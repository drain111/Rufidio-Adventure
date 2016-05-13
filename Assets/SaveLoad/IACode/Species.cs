using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
[XmlRoot("Species")]

public class Species {
    float CrossoverChance = 0.75f;
    [XmlAttribute("topfitness")]
    public int topfitness;
    [XmlAttribute("staleness")]
    public int staleness;
    [XmlArray("genomes")]
    [XmlArrayItem("genome")]
    public List<Genome> genomes;
    [XmlAttribute("averagefitness")]
    public int averagefitness;
    float DeltaDisjoint = 2.0f;
    float DeltaWeights = 0.4f;
    float DeltaThreshold = 1.0f;

	public Species (int TopFitness, int Staleness, List<Genome> Genome, int AverageFitness) {
        topfitness = TopFitness;
        staleness = Staleness;
        genomes = Genome;
        averagefitness = AverageFitness;
    }
    public Species() : base()
    {

    }
    public List<Genome> getGenomes()
    {
        return genomes;
    }
    public int getTopFitness()
    {
        return topfitness;
    }
    public int getAverageFitness()
    {
        return averagefitness;
    }
    public void setTopFitness(int Fitness)
    {
        topfitness = Fitness;
    }
    public int getStaleness()
    {
        return staleness;
    }
    public void setStaleness(int Stall)
    {
        staleness = Stall;
    }
    public bool aresame(Genome genome)
    {

        float dd = DeltaDisjoint * disjoint(genome.getGenes(), genomes[0].getGenes());
        float dw = DeltaWeights * weights(genome.getGenes(), genomes[0].getGenes());
        return dd + dw < DeltaThreshold;
    }
    public void addtogenome(Genome genome)
    {
        genomes.Add(genome);
    }
    float disjoint (List<Genes> gene1, List<Genes> genes2)
    {
        bool[] aux1 = new bool[1000000];
        bool[] aux2 = new bool[1000000];
        float n = Mathf.Max(gene1.Count, genes2.Count);

        for (int i = 0; i < n; i++)
        {
            if ( i >= genes2.Count && n == gene1.Count)
            {
                aux1[gene1[i].getInnovation()] = true;

            }
            else if (i >= gene1.Count && n == genes2.Count)
            {
                aux2[genes2[i].getInnovation()] = true;
            }
            else
            {
                aux1[gene1[i].getInnovation()] = true;
                aux2[genes2[i].getInnovation()] = true;
            }
            
        }
        int genesdifferent = 0;
        for (int i = 0; i < n; i++)
        {
            if (i >= genes2.Count && n == gene1.Count)
            {
                if (!aux2[gene1[i].getInnovation()])
                {
                    genesdifferent++;
                }

            }
            else if (i >= gene1.Count && n == genes2.Count)
            {
                if (!aux1[genes2[i].getInnovation()])
                {
                    genesdifferent++;
                }
            }
            else
            {
                if (!aux2[gene1[i].getInnovation()])
                {
                    genesdifferent++;
                }
                if (!aux1[genes2[i].getInnovation()])
                {
                    genesdifferent++;
                }
            }
        }

        return genesdifferent / n;

    }
    float weights(List<Genes> gene1, List<Genes> genes2)
    {
        Genes[] genes = new Genes[1000000];
        for (int i = 0; i < genes2.Count; i++)
        {
            genes[genes2[i].getInnovation()] = genes2[i];
        }
        int sum = 0;
        int coincident = 1;
        for (int i = 0; i < gene1.Count; i++)
        {
            Genes aux = genes[gene1[i].getInnovation()];
            if (aux != null)
            {
                sum += (int) Mathf.Abs((float) gene1[i].getweight() - (float) aux.getweight());
                coincident++;
            }
        }
        return (float) sum / (float) coincident;
    }
    public void calculateAverageFitness()
    {
        int total = 0;
        for (int i = 0; i < genomes.Count; i++)
        {
            total += genomes[i].getRank();
        }
        averagefitness = total / genomes.Count;
    }

    public Genome breedChild(Pool pool, sightsense sightsense)
    {
        Genome child;
        if (Random.value < CrossoverChance)
        {
            child = genomes[Random.Range(0, genomes.Count)].crossover(genomes[Random.Range(0, genomes.Count)], pool, sightsense);
        }
        else
        {
            child = genomes[Random.Range(0, genomes.Count)].copyGenome();
        }
        child.mutate(pool, sightsense);

        return child;
    }
}
