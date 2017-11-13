using UnityEngine;
using System.Collections;

public class PurchaseButton : MonoBehaviour {

	public ResourceUI ResourceUI;

	public ResourceType GetResourceType () {
		return ResourceUI.Resource;
	}

}
