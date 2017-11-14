using UnityEngine;
using System.Collections;

public abstract class GameModeData : MonoBehaviour {

	public int MaxTime;

	public ModeResourceData[] ResourceDataArray;
	public int SecondsToGenerateResources;
	public int StartingCrowns;
	public int ResourcePercentageFilledOnPurchase;

	public int TroopTrainCost;
	public ResourceType TroopTrainResource;
	public int TroopsTrained;

	public int EnemyInfluence_Seconds;
	public int EnemyInfluence_Percentage;

	public int MissionRefresh_Seconds;

	public ModeResourceData GetResourceData (ResourceType resource) {
		foreach (ModeResourceData modeResourceData in ResourceDataArray) {
			if (modeResourceData.ResourceType == resource) {
				return modeResourceData;
			}
		}

		Debug.LogWarning ("Could not find resource data type - check current mode's resource data array");
		return null;
	}

}
