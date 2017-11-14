﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private Main main;

	private Dictionary<ResourceType, int> resources;
	private int crowns;

	private int resourceGainTime;

	void Awake () {
		main = Util.GetMain ();
		resources = new Dictionary<ResourceType, int> ();
	}

	void Start () {
		setStartingCrowns ();
		setStartingResources ();

		SetResourceGainTimeToMax ();			
	}

	void Update () {
		checkStorePurchases ();
		checkTroopTrain ();
		checkAttack ();
	}

	public void SecondTick () {
		resourceGainTime--;

		if (resourceGainTime == 0) {
			GainResources ();
			SetResourceGainTimeToMax ();
		}
	}

	private void setStartingCrowns () {
		crowns = Util.GetCurrentGameModeData ().StartingCrowns;
	}

	private void setStartingResources () {
		foreach (ResourceData resourceData in main.resourceLibrary.Resources) {
			resources.Add (resourceData.Type, 0);
		}

		List<ResourceType> typesChecked = new List<ResourceType> ();
		GameModeData curMode = Util.GetCurrentGameModeData ();

		foreach (ModeResourceData modeResourceData in curMode.ResourceDataArray) {
			if (typesChecked.Contains (modeResourceData.ResourceType)) {
				Debug.LogWarning ("Mode resource data was set up incorrectly. Check this mode's resource data array");
			}
			else {
				typesChecked.Add (modeResourceData.ResourceType);
				resources [modeResourceData.ResourceType] = modeResourceData.StartingAmount;
			}
		}

		main.resourceUIManager.UpdateResourceUI ();
	}
		
	public void SetResourceGainTimeToMax () {
		resourceGainTime = Util.GetCurrentGameModeData ().SecondsToGenerateResources;
	}

	public int GetResourceGainTime () {
		return resourceGainTime;
	}

	public Dictionary <ResourceType, int> GetResourceDictionary () {
		return resources;
	}

	public void GainResources () {
		GameModeData gmData = Util.GetCurrentGameModeData ();

		foreach (ModeResourceData modeResourceData in gmData.ResourceDataArray) {
			if (resources.ContainsKey (modeResourceData.ResourceType)) {
				GainResource (modeResourceData.ResourceType, modeResourceData.AmountGenerated);
			}
		}
	}

	public void GainResource (ResourceType resource, int amount) {
		resources [resource] += amount;
		resources [resource] = Mathf.Clamp (resources [resource], 0, Util.GetCurrentGameModeData ().GetResourceData (resource).ResourceCap);
		main.resourceUIManager.UpdateResourceUI ();
	}

	public void SpendResource (ResourceType resource, int amount) {
		resources [resource] -= amount;
		main.resourceUIManager.UpdateResourceUI ();
	}

	public int GetCrowns () {
		return crowns;
	}

	public void SpendCrowns (int amount) {
		crowns -= amount;
	}

	private void checkStorePurchases () {
		if (Input.GetMouseButtonDown (0)) {
			Vector2 worldPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast (worldPos, Vector2.zero, 10000, 1 << LayerMask.NameToLayer ("Clickables"));
			if (hit.collider != null) {
				GameObject go = hit.collider.gameObject;

				if (go.GetComponent<PurchaseButton> () != null) {
					attemptToCrownPurchase (go.GetComponent<PurchaseButton> ().GetResourceType ());
				}
			}
		}
	}
		
	private void attemptToCrownPurchase (ResourceType resource) {
		if (CanCrownPurchase (resource)) {
			if (!main.gs.FreePurchases) {
				SpendCrowns (main.resourceLibrary.GetResourceData (resource).CrownCost);
			}
			GainResource (resource, Util.GetCurrentGameModeData ().GetResourceData (resource).GetResourcesFromPurchase ());

		}
		else {
			// Figure out why you can't purchase and show appropriate UI
		}
	}

	public bool CanCrownPurchase (ResourceType resource) {
		bool atCap = resources [resource] >= Util.GetCurrentGameModeData ().GetResourceData (resource).ResourceCap;
		if (atCap) {
			return false;
		}

		bool hasEnoughCrowns = crowns >= main.resourceLibrary.GetResourceData (resource).CrownCost;
		return (main.gs.FreePurchases || hasEnoughCrowns);
	}
		
	private void checkTroopTrain () {
		if (Input.GetMouseButtonDown (0)) {
			Vector2 worldPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast (worldPos, Vector2.zero, 10000, 1 << LayerMask.NameToLayer ("Clickables"));
			if (hit.collider != null) {
				GameObject go = hit.collider.gameObject;

				if (go.GetComponent<TroopTrainButton> () != null) {
					attemptToTrainTroops ();
				}
			}
		}
	}

	// Right now you can only train troops with 1 resource
	private void attemptToTrainTroops () {
		GameModeData gmData = Util.GetCurrentGameModeData ();

		if (CanTrainTroops ()) {
			SpendResource (gmData.TroopTrainResource, gmData.TroopTrainCost);
			GainResource (ResourceType.Troops, gmData.TroopsTrained);
		}
	}

	public bool CanTrainTroops () {
		GameModeData gmData = Util.GetCurrentGameModeData ();

		bool atCap = resources [ResourceType.Troops] >= gmData.GetResourceData (ResourceType.Troops).ResourceCap;
		if (atCap) {
			return false;
		}

		ResourceType trainingResource = gmData.TroopTrainResource;
		bool hasEnoughFood = resources [trainingResource] >= gmData.TroopTrainCost;
		return (hasEnoughFood);
	}

	private void checkAttack () {

	}

}
