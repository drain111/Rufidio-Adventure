using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Serialization;
[XmlRoot("Genes")]

public class Genes : IComparable<Genes>
{
    [XmlAttribute("into")]
    public int into;
    [XmlAttribute("exit")]
    public int exit;
    [XmlAttribute("weight")]
    public float weight;
    [XmlAttribute("enabled")]
    public bool enabled;
    [XmlAttribute("geneinnovation")]
    public int innovation;
    

    public Genes(int Into, int Exit, float Weight, bool Enabled, int Innovation)
    {
        into = Into;
        exit = Exit;
        weight = Weight;
        enabled = Enabled;
        innovation = Innovation;
    }
    public Genes() : base()
    {

    }
    public Genes copyGene()
    {
        return new Genes(into, exit, weight, enabled, innovation);
        
    }
    public float getweight()
    {
        return weight;
    }
    public void setweight(float Weight)
    {
        weight = Weight;
    }
    public int getInnovation()
    {
        return innovation;
    }
    public void setInnovation(int Innovation)
    {
        innovation = Innovation;
    }
    public int getInto()
    {
        return into;
    }
    public void setInto(int setinto)
    {
        into = setinto;
    }
    public int getOut()
    {
        return exit;
    }
    public void setOut(int setexit)
    {
        exit = setexit;
    }
    public bool getenabled()
    {
        return enabled;
    }
    public void setenabled(bool state)
    {
        enabled = state;
    }
    

    int IComparable<Genes>.CompareTo(Genes other)
    {
        if (exit < other.getOut())
        {
            return 1;
        }
        if (exit > other.getOut())
        {
            return -1;
        }
        return 0;
    }
}
