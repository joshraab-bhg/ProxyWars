using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceUIManager : MonoBehaviour {

	private Main main;

	public ResourceType[] ResourcesInUse;
	public GameObject ResourceContainer;
	public Dictionary<ResourceType, ResourceUI> ResourceUIList; 

	public const int RESOURCE_UI_Y_OFFSET = 45;

	void Awake () {
		main = Util.GetMain ();
		float curY = ResourceContainer.transform.position.y;
		ResourceUIList = new Dictionary<ResourceType, ResourceUI> ();

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
	}

	public void UpdateResourceUI () {
		foreach (ResourceType resource in ResourceUIList.Keys) {
			if (main.p.GetResourceDictionary ().ContainsKey (resource)) {
				ResourceUIList [resource].NumberOwnedText.text = main.p.GetResourceDictionary () [resource] + "";
				ResourceUIList [resource].CrownOutline.gameObject.SetActive (main.p.CanPurchase (resource));
			}
		}
	}
}
