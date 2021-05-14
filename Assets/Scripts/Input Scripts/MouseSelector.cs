using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//change this -- centralized selector thing so we don't need a bunch of stuff...
//a lot of this also collides with selectable or whatever it is we have...
public class MouseSelector : MonoBehaviour {
    

    public UnityEvent OnSelect;
    public UnityEvent OnDeselect;

    public GameObject selectorImagePrefab;
    
    [HideInInspector]
    private GameObject selectorImage;

#pragma warning disable CS0414 // The field 'MouseSelector.isSelected' is assigned but its value is never used
    private bool isSelected = false;
#pragma warning restore CS0414 // The field 'MouseSelector.isSelected' is assigned but its value is never used

	// Use this for initialization
	void Start () {

        //this sets the selector image based on the prefab and on the characteristics of the game object this script is attached to.
        selectorImage = Instantiate(selectorImagePrefab, this.gameObject.transform);
        selectorImage.GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
        selectorImage.SetActive(false);
        //maybe selector image default off.  when this is selected, turn it on.

    }
	
	void Update () {
		
	}
    
    public bool Select()
    {
        selectorImage.SetActive(true);
        isSelected = true;
        OnSelect.Invoke();
        return true; // returns whether it is selected
    }
    
    public bool DeSelect()
    {
        selectorImage.SetActive(false);
        isSelected = false;
        OnDeselect.Invoke();
        return false; // returns whether it is selected
    }


}
