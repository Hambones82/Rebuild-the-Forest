using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Paths
{
    public static class Types
    {
        public static class UnitActions
        {
            private static readonly string UnitActionFolder = Application.dataPath + @"\Resources\Types\ResourceActions\";
            public static readonly string CleanPollution = UnitActionFolder + "Clean Pollution Action Type";
            public static readonly string Move = UnitActionFolder + "Move Action Type";
            public static readonly string Idle = UnitActionFolder + "Idle Action Type";
        }
    }
}
