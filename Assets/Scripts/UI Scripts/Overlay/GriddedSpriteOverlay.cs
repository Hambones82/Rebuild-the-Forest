using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class GriddedSpriteOverlay : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GridMap _gridMap;
    //temporary - for testing only...
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(_spriteRenderer, "GriddedSpriteOverlay requires a SpriteRenderer component.");
        _gridMap = GridMap.Current;        
    }

    public void SetSizeInCells(Vector2Int size)
    {
        Vector2 imageSize = _gridMap.MapSizeToWorldSize(size); // plus effect size...
        _spriteRenderer.size = imageSize;
    }

    public void SetAlpha(float alpha)
    {
        Color newColor = _spriteRenderer.color;
        newColor.a = alpha;
        _spriteRenderer.color = newColor;
    }

    public void EnableOverlay()
    {
        _spriteRenderer.enabled = true;
    }

    public void DisableOverlay()
    {
        _spriteRenderer.enabled = false;
    }
}
