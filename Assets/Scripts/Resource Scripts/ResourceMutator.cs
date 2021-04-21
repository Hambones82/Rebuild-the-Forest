using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMutator : MonoBehaviour {

    public enum ResourceMutationType { continuous };

    [System.Serializable]
    public class ResourceMutation
    {
        public Resource resourceToMutate;
        public ResourceMutationType resourceMutationType;
        public float resourceMutationAmount;
    }

    public List<ResourceMutation> resourceMutations;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		foreach(ResourceMutation rMutation in resourceMutations)
        {
            if(rMutation.resourceMutationType == ResourceMutationType.continuous)
            {
                rMutation.resourceToMutate.currentAmount -= Time.deltaTime * rMutation.resourceMutationAmount;
            }
        }
	}
}
