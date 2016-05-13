using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Neuron  {
   
    public List<Genes> into;
    public float value;

    public Neuron(List<Genes> Into, float Value)
    {
        into = Into;
        value = Value;
    }
    public Neuron() : base()
    {

    }
    public void addIncomingGene(Genes geneComing)
    {
        into.Add(geneComing);
    }
    public float getValue()
    {
        return value;
    }
    public void setValue(float Value)
    {
        value = Value;
    }
    public List<Genes> getInto()
    {
        return into;
    }
}
