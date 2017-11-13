using UnityEngine;
using System.Collections;

public class ResourceLibrary : MonoBehaviour {
	
	public ResourceData[] Resources;

	public ResourceData GetResourceData (ResourceType type) {
		for (int i = 0; i < Resources.Length; i++) {
			if (Resources [i].Type == type) {
				return Resources [i];
			}
		}

		Debug.LogError ("Could not find resource in library");
		return null;
	}

}
