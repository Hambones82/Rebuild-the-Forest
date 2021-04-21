using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public static class GizmosExtensionMethods {

	public static void DrawCameraParallelRectangle(Vector3 topLeft, Vector3 bottomRight)
    {
        Assert.IsTrue(topLeft.z == bottomRight.z);
        Vector3 topRight = new Vector3(bottomRight.x, topLeft.y, topLeft.z);
        Vector3 bottomLeft = new Vector3(topLeft.x, bottomRight.y, topLeft.z);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, bottomRight);
    }
}

