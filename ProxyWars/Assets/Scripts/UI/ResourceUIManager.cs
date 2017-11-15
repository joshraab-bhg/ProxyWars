using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceUIManager : MonoBehaviour {

	public ResourceType[] ResourcesInUse;
	public GameObject ResourceContainer;
	public ResourceUI[] ResourceUIList;

	public const int RESOURCE_UI_Y_OFFSET = 45;

	void Start () {
		foreach (ResourceUI r in ResourceUIList) {
			if (r.Resource != ResourceType.None) {
				r.Setup (r.Resource);
			}
			else {
				Debug.LogWarning ("Resource UI did not have resource type set");
			}
		}

//		float curY = ResourceContainer.transform.position.y;

		/*
		List<ResourceType> resourcesUsed = new List<ResourceType> ();

		foreach (ResourceType nextResource in ResourcesInUse) {
			if (resourcesUsed.Contains (nextResource)) {
				Debug.LogWarning ("List of resources in use contains multiple instances of the same resource type");
				break;
			}
			resourcesUsed.Add (nextResource);
				
			Vector3 newPos = new Vector3 (ResourceContainer.transform.position.x, curY, Main.Z_UI);
			GameObject go = (GameObject)GameObject.Instantiate (Util.GetMain ().uiLibrary.ResourceUIPrefab, newPos, Quaternion.identity);
			go.transform.SetParent (ResourceContainer.transform, false);
			go.GetComponent <ResourceUI> ().Setup (nextResource);
			ResourceUIList.Add (nextResource, go.GetComponent <ResourceUI> ());
			curY -= RESOURCE_UI_Y_OFFSET;
		}
		*/
	}

	public void UpdateResourceUI () {
		foreach (ResourceUI r in ResourceUIList) {
			r.UpdateUI ();
		}
	}
}
