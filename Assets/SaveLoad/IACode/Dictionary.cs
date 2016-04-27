using UnityEngine;
using System.Collections;
using System;

public class Dictionary : IComparable<Dictionary> {

	public Dictionary(string word, int number)
    {
        
    }

    public Dictionary(string word, float number)
    {

    }

    public int CompareTo(Dictionary other)
    {
        throw new NotImplementedException();
    }
}
