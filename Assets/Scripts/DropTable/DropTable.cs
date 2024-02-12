using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class DropTable 
{
    [SerializeField]
    [Range(0,100)]
    private float dropChance;

    [System.Serializable]
    private class DropTableEntry
    {
        [Range(0, 10)]
        public int rarity;
        public BuildingType buildingType;
    }

    [SerializeField]
    private List<DropTableEntry> dropTableEntries;

    private int totalDropTable;

    //what's the purpose of this?  maybe just to show at runtime (inspector) how many of these things there are?
    [SerializeField]
    private int[] count;

    public void Initialize()
    {
        totalDropTable = dropTableEntries.Select((entry) => entry.rarity).Sum();
        //Debug.Log($"Total drop table: {totalDropTable}");
        count = new int[dropTableEntries.Count];
    }

    

    public BuildingType GetDroppedBuildingType()
    {
        if (totalDropTable == 0) return null;
        float dropped = UnityEngine.Random.Range(0, 100f);
        if(dropped > dropChance)
        {
            //Debug.Log("drop chance too small");
            return null;
        }
        else
        {
            int selector = UnityEngine.Random.Range(0, totalDropTable) + 1;
            //Debug.Log($"selector: {selector}");
            for (int i = 0; i < dropTableEntries.Count; i++)
            {
                selector -= dropTableEntries[i].rarity;
                //Debug.Log($"selector updated iteration {i}: {selector}");
                if (selector <= 0)
                {
                    count[i]++;
                    return dropTableEntries[i].buildingType;
                }
            }
        }
        throw new InvalidOperationException("the drop table percentages are bugged -- this code path should not happen");
    }
}
