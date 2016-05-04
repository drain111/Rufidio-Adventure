using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool
{
    int StaleSpecies = 15;

    int generation;
    int currentspecies;
    int currentgenome;
    int currentframe;
    int maxfitness;
    int innovation;
    List<Species> species;
    public Pool(int Generation, int CurrentSpecies, int CurrentGenome, int CurrentFrame, int MaxFitness, int Innovation, List<Species> Species)
    {
        generation = Generation;
        currentspecies = CurrentSpecies;
        currentgenome = CurrentGenome;
        currentframe = CurrentFrame;
        maxfitness = MaxFitness;
        innovation = Innovation;
        species = Species;

    }
    public void newGeneration(sightsense sightsense)
    {

        cullSpecies(false);
        rankGlobally();
        removeStaleSpecies();
        rankGlobally();
        for (int i = 0; i < species.Count; i++)
        {
            species[i].calculateAverageFitness();
        }
        removeWeakSpecies();
        int sum = totalAverageFitness();
        List <Genome> children = new List<Genome>();
        for (int i = 0; i < species.Count; i++)
        {
            int breed = (int)Mathf.Floor(species[i].getAverageFitness() / sum * 300) - 1;
            for (int j = 0; j < breed; j++)
            {
                children.Add(species[i].breedChild(this,sightsense));
            }
        }
        cullSpecies(true);
        while (children.Count + species.Count < 300)
        {
            children.Add(species[Random.Range(0, species.Count)].breedChild(this, sightsense));
        }
        for (int i = 0; i < children.Count; i++)
        {
            Genome child = children[i];
            addToSpecies(child);
        }

        generation++;

        //write to file
        currentspecies = 0;


    }
    void removeWeakSpecies()
    {
        List<Species> survived = new List<Species>();

        int sum = totalAverageFitness();
        for (int i = 0; i < species.Count; i++)
        {
            Species specie = species[i];
            //300 population
            int breed = (int) Mathf.Floor(specie.getAverageFitness() / sum * 300);
            if (breed >= 1)
            {
                survived.Add(specie);
            }
        }
        species = survived;
    }
    int totalAverageFitness()
    {
        int sum = 0;
        for (int i = 0; i < species.Count; i++)
        {
            sum += species[i].getAverageFitness();
        }
        return sum;
    }
    void removeStaleSpecies()
    {
        List<Species> survived = new List<Species>();
        for (int i = 0; i < species.Count; i++)
        {
            Species specie = species[i];
            //Note debug to prove that a > b, max to min
            specie.getGenomes().Sort();
            specie.getGenomes().Reverse();
            if (specie.getGenomes()[0].getDistanceTraveled() > specie.getTopFitness())
            {
                specie.setTopFitness(specie.getGenomes()[0].getDistanceTraveled());
                specie.setStaleness(0);
            }
            if( specie.getStaleness() < StaleSpecies || specie.getTopFitness() >= maxfitness) {
                survived.Add(specie);

            }
        }
        species = survived;

    }
    void rankGlobally()
    {
        List<Genome> aux = new List<Genome>();
        for (int i = 0; i < species.Count; i++)
        {
            for (int j = 0; j < species[i].getGenomes().Count; j++)
            {
                aux.Add(species[i].getGenomes()[j]);

            }
        }
        aux.Sort();
        for (int i = 0; i < aux.Count; i++)
        {
            aux[i].setRank(i);
        }
    }
    void cullSpecies(bool sentence)
    {
        for (int i = 0; i < species.Count; i++)
        {
            Species aux = species[i];
            aux.getGenomes().Sort();
            aux.getGenomes().Reverse();

            int remaining = (int) Mathf.Ceil(aux.getGenomes().Count / 2);

            if (sentence)
            {
                remaining = 1;
            }
            
            aux.getGenomes().RemoveRange(remaining, aux.getGenomes().Count - remaining);
            
        }
    }
    public void setCurrentSpecies(int CurrentSpecies)
    {
        currentspecies = CurrentSpecies;
    }
    public void setCurrentGenome(int CurrentGenome)
    {
        currentgenome = CurrentGenome;
    }
    public int getMaxFitness()
    {
        return maxfitness;
    }
    public void setMaxFitness(int MaxFitness)
    {
        maxfitness = MaxFitness;
    }
    public int newInnovation()
    {
        return innovation++;
    }
    public void setCurrentFrame(int frame)
    {
        currentframe = frame;
    }
    public List<Species> getspecies()
    {
        return species;
    }
    public int getcurrentspecies()
    {
        return currentspecies;
    }
    public int getcurrentgenome()
    {
        return currentgenome;
    }
   
    public void addToSpecies(Genome genome)
    {
        //first see if we can qualify this genome into an specie
        bool speciesfound = false;

        for (int i = 0; i < species.Count; i++)
        {
            Species aux = species[i];
            if (!speciesfound && aux.aresame(genome))
            {
                aux.addtogenome(genome);
                speciesfound = true;
            }
        }
        if (!speciesfound)
        {
            Species newspecies = new Species(0, 0, new List<Genome>(), 0);
            newspecies.addtogenome(genome);
            species.Add(newspecies);
        }
    }
}
