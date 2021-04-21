using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ok so the backround input modules are the things that control "default" input.  i.e., if no "foreground" input modules are loaded, this controls the keyboard input
//the idea is that this is used as a game object.  the game object has an input subscriber that defines an input module, which defines the actual keypresses for the background.
public class BackgroundInputManager : MonoBehaviour {

    //the purpose of this is to manage when different background input modules are loaded and unloaded.

	// Use this for initialization
	void Start () {
        GetComponent<InputSubscriber>().Activate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
