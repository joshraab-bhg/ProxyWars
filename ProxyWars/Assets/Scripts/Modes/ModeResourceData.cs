using UnityEngine;
using System.Collections;

public class ModeResourceData : MonoBehaviour {

	public ResourceType ResourceType;
	public int StartingAmount;
	public int AmountGenerated;
	public int ResourceCap;

	public int GetResourcesFromPurchase () {
		return (int) (ResourceCap * ((float) Util.GetCurrentGameModeData ().ResourcePercentageFilledOnPurchase / 100f));
	}
}
