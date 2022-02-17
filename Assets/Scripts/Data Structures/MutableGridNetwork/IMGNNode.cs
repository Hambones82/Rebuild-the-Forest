using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMGNNode
{
    int NetworkID { get; set; }
    List<IMGNNode> GetAdjacentNodes();
    bool IsValid();
}
