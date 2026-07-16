using NUnit.Framework;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveLoadScript
{
    public class SaveObject
    {
        public float[] playerPosition;
        public int money;
        public int waterLevel;
        public int health;
        public GameObject[] farmFields;

    }

    public class SaveFarmField
    {
        public float[] position;
        public float waterTimer;
        public bool isWatered;
        public float growthTimer;
        public bool isOccupied;
    }
}
