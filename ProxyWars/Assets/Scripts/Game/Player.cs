using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	private Main main;

	private Dictionary<ResourceType, int> resources;
	private int crowns;

	private int resourceGainTime;

	// lol this cannot be the right way to do this
	public const float LOOT_PERC_RAID_FAILED_MIN = 0f;
	public const float LOOT_PERC_RAID_FAILED_MAX = 0.1f;

	public const float LOOT_PERC_RAID_MODERATE_SUCCESS_MIN = 0.4f;
	public const float LOOT_PERC_RAID_MODERATE_SUCCESS_MAX = 0.6f;

	public const float LOOT_PERC_RAID_GREAT_SUCCESS_MIN = 0.8f;
	public const float LOOT_PERC_RAID_GREAT_SUCCESS_MAX = 1f;

	public const float LOOT_PERC_ASSAULT_0_STARS_MIN = 0.1f;
	public const float LOOT_PERC_ASSAULT_0_STARS_MAX = 0.2f;

	public const float LOOT_PERC_ASSAULT_1_STARS_MIN = 0.25f;
	public const float LOOT_PERC_ASSAULT_1_STARS_MAX = 0.35f;

	public const float LOOT_PERC_ASSAULT_2_STARS_MIN = 0.4f;
	public const float LOOT_PERC_ASSAULT_2_STARS_MAX = 0.5f;

	public const float LOOT_PERC_ASSAULT_3_STARS_MIN = 0.55f;
	public const float LOOT_PERC_ASSAULT_3_STARS_MAX = 0.65f;

	public const float LOOT_PERC_ASSAULT_4_STARS_MIN = 0.7f;
	public const float LOOT_PERC_ASSAULT_4_STARS_MAX = 0.8f;

	public const float LOOT_PERC_ASSAULT_5_STARS_MIN = 0.9f;
	public const float LOOT_PERC_ASSAULT_5_STARS_MAX = 1f;

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
			if (typesChecked.Contains (modeResourceData.Type)) {
				Debug.LogWarning ("Mode resource data was set up incorrectly. Check this mode's resource data array");
			}
			else {
				typesChecked.Add (modeResourceData.Type);
				resources [modeResourceData.Type] = modeResourceData.StartingAmount;
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
			if (resources.ContainsKey (modeResourceData.Type)) {
				GainResource (modeResourceData.Type, modeResourceData.AmountGenerated);
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
		if (CanTrainTroops ()) {
			GameModeData gmData = Util.GetCurrentGameModeData ();
			SpendResource (gmData.TroopTrainResource, gmData.TroopTrainCost);
			GainResource (ResourceType.Troops, gmData.TroopsTrained);
		}
		else {
			// Figure out why you can't train troops and show appropriate UI
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
		if (Input.GetMouseButtonDown (0)) {
			Vector2 worldPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast (worldPos, Vector2.zero, 10000, 1 << LayerMask.NameToLayer ("Clickables"));
			if (hit.collider != null) {
				GameObject go = hit.collider.gameObject;

				if (go.GetComponent<AttackButton> () != null) {
					attemptToAttack (go.GetComponent<AttackButton> ().AttackUI.Type);
				}
			}
		}
	}

	private void attemptToAttack (AttackType type) {
		if (CanAttack (type)) {
			ModeAttackData data = Util.GetCurrentGameModeData ().GetAttackData (type);

			// Determine success, troops lost, and loot gained
			switch (type) {
			case AttackType.Raid:
				doRaid ();
				break;
			case AttackType.Assault:
				doAssault ();
				break;
			default:
				Debug.LogWarning ("Unsupported attack type");
				break;
			}
		}
		else {
			// Figure out why you can't attack and show appropriate UI
		}
	}

	public bool CanAttack (AttackType type) {
		ModeAttackData data = Util.GetCurrentGameModeData ().GetAttackData (type);
		return (resources [ResourceType.Troops] >= data.TroopsRequired);
	}

	private void doRaid () {
		/* A raid simulates throwing soldiers/raiders at an enemy base with no expectation of survival.
		 * They have 3 degrees of success: failure (0), moderate success (1), and great success (2).
		 * The degree of success determines how much loot you get.
		 * However, like in a real DomiNations raid, you always lose all troops committed.
		 */

		ModeAttackData data = Util.GetCurrentGameModeData ().GetAttackData (AttackType.Raid);

		int success = Random.Range (0, 3);

		if (success == 0) {
			// Failure - you get 0-10% of max resources
			float perc = 0.1f;
			int maxFood = Util.GetPerc (data.MaxFoodGained, perc);
			int maxGold = Util.GetPerc (data.MaxGoldGained, perc);
			int maxTGs = Util.GetPerc (data.MaxTGsGained, perc);


		}
		else if (success == 1) {	// Moderate success

		}
		else {						// Great success

		}

		int troopsSent = data.TroopsRequired;
		resources [ResourceType.Troops] -= troopsSent;
	}

	private void doAssault () {
		/* Assaults simulate a full attack with your entire army.
		 * They have 6 degrees of success (1 each for 0-5 stars).
		 * The degree of success determines how much loot you get.
		 * On a 5-star success, some troops return. Otherwise they all die.
		 */
	}
}
