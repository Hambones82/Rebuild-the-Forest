using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseoverInfo : MonoBehaviour
{
    [SerializeField]
#pragma warning disable CS0649 // Field 'MouseoverInfo.text' is never assigned to, and will always have its default value null
    private string text;
#pragma warning restore CS0649 // Field 'MouseoverInfo.text' is never assigned to, and will always have its default value null
    public string Text { get => text; }
}
