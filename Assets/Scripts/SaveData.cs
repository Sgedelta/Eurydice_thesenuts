using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<string> combatRooms = new List<string>();
    public List<int> combatTurns = new List<int>();
}