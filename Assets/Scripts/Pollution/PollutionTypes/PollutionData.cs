using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PollutionData", menuName = "Pollution/Pollution Data")]
public class PollutionData : ScriptableObject
{    
    //droptable
    [SerializeField]
    private DropTable _dropTable;
    
    //priority
    [SerializeField]
    private int _priority;
    public int Priority { get => _priority; }
    //a/t else?            
}

