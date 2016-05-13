using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
[XmlRoot("Pool")]
public class Pool
{
    int StaleSpecies = 10;
    [XmlAttribute("generation")]
    public int generation;
    [XmlAttribute("currentspecies")]
    public int currentspecies;
    [XmlAttribute("currentgenome")]
    public int currentgenome;
    [XmlAttribute("currentframe")]
    public int currentframe;
    [XmlAttribute("maxfitness")]
    public int maxfitness;
    [XmlAttribute("innovation")]
    public int innovation;
    [XmlArray("species")]
    [XmlArrayItem("specie")]
    public List<Species> species;
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
    public Pool() : base()
    {
        generation = 0;
        currentspecies = 0;
        currentgenome = 0;
        currentframe = 0;
        maxfitness = 0;
        innovation = 0;
        species = new List<Species>();
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
            int breed = (int)Mathf.Floor((float) species[i].getAverageFitness() / (float) sum * 300.0f) - 1;
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
            int breed = (int) Mathf.Floor((float) specie.getAverageFitness() / (float) sum * 300.0f);
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
            if (specie.getGenomes()[0].getDistanceTraveled() > specie.getTopFitness())
            {
                specie.setTopFitness(specie.getGenomes()[0].getDistanceTraveled());
                specie.setStaleness(0);
            }
            else
            {
                specie.setStaleness(specie.getStaleness() + 1);
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
        aux.Reverse();

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

            float remaining =  Mathf.Ceil(aux.getGenomes().Count / 2.0f);

            if (sentence)
            {
                remaining = 1;
            }

            while (remaining < aux.getGenomes().Count)
            {
                aux.getGenomes().RemoveAt(aux.getGenomes().Count - 1);

            }
            
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
    public List<Species> getSpecies()
    {
        return species;
    }
    public void setSpecies(List<Species> specie)
    {
        species = specie;
    }
    public int getcurrentspecies()
    {
        return currentspecies;
    }
    public int getcurrentgenome()
    {
        return currentgenome;
    }
    public int getGeneration()
    {
        return generation;
    }
    public void setGeneration(int Gen)
    {
        generation = Gen;
    }
    public int getInnovation()
    {
        return innovation;
    }
    public void setInnovation(int newthing)
    {
        innovation = newthing;
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
