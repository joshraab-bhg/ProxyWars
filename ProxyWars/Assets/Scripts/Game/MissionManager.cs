using UnityEngine;
using System.Collections;

public class MissionManager : MonoBehaviour {

	private Main main;

	public City city;
	public Mission CurrentMission;
	public MissionStatus CurrentMissionStatus;

	void Awake () {
		main = Util.GetMain ();
	}
		
	public void UpdateMissionStatus () {
		if (city.IM.Locked) {
			CurrentMissionStatus = MissionStatus.Locked;
		}
		else if (CurrentMission == null) {
			CurrentMissionStatus = MissionStatus.NoMission;
		}
		else if (!city.IsConnectedToFaction (Faction.Player)) {
			CurrentMissionStatus = MissionStatus.CannotDoMissions;
		}
		else if (!hasEnoughResourcesForMission ()) {
			CurrentMissionStatus = MissionStatus.NotEnoughResources;
		}
		else {
			CurrentMissionStatus = MissionStatus.CanCompleteMission;
		}
	}

	public void GenerateMission () {
		
		// Should replace these magic numbers with configurable stuff based on the game mode
		ResourceType resourceType = Util.GetRandomResourceType ();
		int resourceAmount = Random.Range (50, 151);
		int influenceToGain = resourceAmount / 2;

		CurrentMission = new Mission (resourceType, resourceAmount, influenceToGain);
		if (main.uiManager.CityPopup.gameObject.activeSelf) {
			main.uiManager.CityPopup.UpdateUI ();
		}
	}
		
	public void TryToCompleteMission () {
		if (hasEnoughResourcesForMission () && CurrentMissionStatus == MissionStatus.CanCompleteMission) {
			completeMission ();
		}
		else {
			// Show some bubble text to indicate that the mission couldn't be completed
		}
	}

	private void completeMission () {
		main.p.SpendResource (CurrentMission.RequiredResource, CurrentMission.RequiredAmount);
		city.IM.AddInfluence (Faction.Player, CurrentMission.InfluenceToGain);
		GenerateMission ();		// Maybe have this be on a timer (i.e. doesn't instantly regenerate)
	}

	private bool hasEnoughResourcesForMission () {
		return main.p.GetResourceDictionary () [CurrentMission.RequiredResource] >= CurrentMission.RequiredAmount;
	}

}
