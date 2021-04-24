using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Types 
{

    static Types()
    {
        //load all of the scriptableobjects using resources.load or whatever.
    }
    public static class Actions
    {
        public static readonly UnitActionType Move;
        public static readonly UnitActionType CleanPollution;
        public static readonly UnitActionType Idle;
    }


}
