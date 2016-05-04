using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Species {
    float CrossoverChance = 0.75f;
    int topfitness;
    int staleness;
    List<Genome> genomes;
    int averagefitness;
    float DeltaDisjoint = 2.0f;
    float DeltaWeights = 0.4f;
    float DeltaThreshold = 1.0f;

	public Species (int TopFitness, int Staleness, List<Genome> Genome, int AverageFitness) {
        topfitness = TopFitness;
        staleness = Staleness;
        genomes = Genome;
        averagefitness = AverageFitness;
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
            if (genes[gene1[i].getInnovation()] != null)
            {
                sum += (int) Mathf.Abs(gene1[i].getweight() - genes[gene1[i].getInnovation()].getweight());
                coincident++;
            }
        }
        return sum / coincident;
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
            child = genomes[Random.Range(1, genomes.Count)].crossover(genomes[Random.Range(1, genomes.Count)], pool, sightsense);
        }
        else
        {
            child = genomes[Random.Range(1, genomes.Count)].copyGenome();
        }
        child.mutate(pool, sightsense);

        return child;
    }
}
