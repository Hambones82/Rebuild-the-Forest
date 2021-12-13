using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionDiscoverableOverlayController : MonoBehaviour
{
    private TileDataMap tdm;

    [SerializeField]
    private UIOverlayTile discoverableTile;

    [SerializeField]
    DiscoverableManager discoverableManager;

    private void Awake()
    {
        tdm = GetComponent<TileDataMap>();
        discoverableManager.OnDiscoverableChange += SetDiscoverable;
    }

    private void SetDiscoverable(int x, int y, bool b)
    {
        if (!b)
        {
            tdm.SetTileAt(discoverableTile, new Vector2Int(x, y));
        }
    }
}
