using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<int> NumHits;
    public List<float> NumAvgRounds;

    public SaveData()
    {

    }

    public SaveData(Dictionary<RoomModifier, Tuple<int, float>> data)
    {
        NumHits      = new List<int>();
        NumAvgRounds = new List<float>();

        for (int i = 0; i < 3; i++) {
            NumHits.Add(0);
            NumAvgRounds.Add(0);
        }

        foreach(RoomModifier key in data.Keys)
        {
            //disgusting, but it works! - sam
            NumHits[(int)key] = data[key].Item1;
            NumAvgRounds[(int)key] = data[key].Item2;
        }
    }
}