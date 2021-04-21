using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionToStatDef : ScriptableObject
{
    [System.Serializable]
    private class DefEntry
    {
        public Type type;
        public StatScriptableObject stat;
    }

    [SerializeField]
    private List<DefEntry> statDefs;
}
