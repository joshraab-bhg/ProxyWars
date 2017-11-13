using UnityEngine;
using System.Collections;

public class Mission {

	public ResourceType RequiredResource;
	public int RequiredAmount;
	public int InfluenceToGain;

	public Mission (ResourceType requiredResource, int requiredAmount, int influenceToGain) {
		RequiredResource = requiredResource;
		RequiredAmount = requiredAmount;
		InfluenceToGain = influenceToGain;
	}

}
