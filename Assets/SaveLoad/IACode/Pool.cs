using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool
{
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
    public int getCUrrentFrame()
    {

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
