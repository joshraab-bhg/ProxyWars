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

	public ModeAttackData[] AttackDataArray;

	public int EnemyInfluence_Seconds;
	public int EnemyInfluence_Percentage;

	public int MissionRefresh_Seconds;

	public ModeResourceData GetResourceData (ResourceType resource) {
		foreach (ModeResourceData data in ResourceDataArray) {
			if (data.Type == resource) {
				return data;
			}
		}

		Debug.LogWarning ("Could not find resource data type - check current mode's resource data array");
		return null;
	}

	public ModeAttackData GetAttackData (AttackType type) {
		foreach (ModeAttackData data in AttackDataArray) {
			if (data.Type == type) {
				return data;
			}
		}

		Debug.LogWarning ("Could not find attack data type - check current mode's attack data array");
		return null;
	}

}
