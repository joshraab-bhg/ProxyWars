using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CityPopupUI : MonoBehaviour {

	private Main main;

	public Image CompleteMissionButtonBG; 
	public Text CompleteMissionButtonText;

	public Text MissionRequiredAmount;
	public Image MissionRequiredResource;
	public Text InfluenceToGain;

	public Text PlayerInfluencePerc;
	public Text EnemyInfluencePerc;
	public Text NeutralInfluencePerc;

	private City activeCity;

	private Vector3 normalOffset = new Vector3 (0, -4.6f, 0);
	private Vector3 bottomOffset = new Vector3 (0, 4.6f, 0);
	private int bottomCutoff = 8;

	public void Awake () {
		main = Util.GetMain ();
	}

	public void Start () {
		gameObject.SetActive (false);
	}

	public void Activate (City c) {
		gameObject.SetActive(true);
		activeCity = c;
		UpdateUI ();
		setPosition ();
	}

	public void SecondTick () {
		UpdateUI ();
	}

	public void UpdateUI () {
		if (gameObject.activeSelf) {
			updateMissionUI ();
			updateInfluenceUI ();
		}
	}

	private void updateMissionUI () {
		activeCity.MM.UpdateMissionStatus ();

		if (activeCity.MM.CurrentMission != null) {

			switch (activeCity.MM.CurrentMissionStatus) {

			case MissionStatus.Locked: 
				CompleteMissionButtonText.text = "LOCKED";
				CompleteMissionButtonBG.color = main.uiLibrary.Missioncolor_NotAvailable;
				break;

			case MissionStatus.NoMission: 
				CompleteMissionButtonText.text = "NO MISSION";
				CompleteMissionButtonBG.color = main.uiLibrary.Missioncolor_NotAvailable;
				break;

			case MissionStatus.CannotDoMissions: 
				CompleteMissionButtonText.text = "NOT CONNECTED";
				CompleteMissionButtonBG.color = main.uiLibrary.Missioncolor_NotAvailable;
				break;

			case MissionStatus.NotEnoughResources:
				CompleteMissionButtonText.text = "NEED RESOURCES";
				CompleteMissionButtonBG.color = main.uiLibrary.Missioncolor_NotAvailable;
				break;
			
			case MissionStatus.CanCompleteMission:
				CompleteMissionButtonText.text = "COMPLETE";
				CompleteMissionButtonBG.color = main.uiLibrary.MissionColor_Available;
				break;
			}

			MissionRequiredAmount.text = "" + activeCity.MM.CurrentMission.RequiredAmount;
			MissionRequiredResource.sprite = Util.GetMain ().resourceLibrary.GetResourceData (activeCity.MM.CurrentMission.RequiredResource).Icon;
			InfluenceToGain.text = "Gain " + Util.GetInfluencePercentage (activeCity.MM.CurrentMission.InfluenceToGain) + "% Influence";

		}
		else {
			MissionRequiredAmount.text = "";
			MissionRequiredResource.sprite = null;
			InfluenceToGain.text = "";
		}
	}

	private void updateInfluenceUI () {
		float fPlayerPerc = Util.GetInfluencePercentage (activeCity.IM.PlayerInfluence);
		float fEnemyPerc = Util.GetInfluencePercentage (activeCity.IM.EnemyInfluence);
		float fNeutralPerc = Util.GetInfluencePercentage (activeCity.IM.GetNeutralInfluence ());

		PlayerInfluencePerc.text = Util.GetFormattedPercentage (fPlayerPerc);
		EnemyInfluencePerc.text = Util.GetFormattedPercentage (fEnemyPerc);
		NeutralInfluencePerc.text = Util.GetFormattedPercentage (fNeutralPerc);
	}

	private void setPosition () {
		Vector3 newPos = activeCity.transform.position;

		if (newPos.y > bottomCutoff) {
			newPos += normalOffset;
		}
		else {
			newPos += bottomOffset;
		}
			
		gameObject.transform.position = newPos;
	}

	public void TryToCompleteMission () {
		activeCity.MM.TryToCompleteMission ();
	}
}
