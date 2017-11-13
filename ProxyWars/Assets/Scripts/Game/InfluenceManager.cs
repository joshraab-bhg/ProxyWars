using UnityEngine;
using System.Collections;

public class InfluenceManager : MonoBehaviour {

	Main main;
	City city;

	private int maxInfluence;
	public int PlayerInfluence;
	public int EnemyInfluence;
	public bool Locked;

	private int influenceToAdd;

	public void Setup (City c) {
		main = Util.GetMain ();
		city = c;

		maxInfluence = main.gs.MaxInfluence;
		PlayerInfluence = 0;
		EnemyInfluence = 0;
		Locked = false;
	}

	public void AddInfluence (Faction f, int amount) {
		if (f == Faction.Player) {
			if (PlayerInfluence + amount >= maxInfluence) {
				SetCompleteControl (Faction.Player);
			}
			else if (amount > GetNeutralInfluence ()) {
				EnemyInfluence -= (amount - GetNeutralInfluence ());
				PlayerInfluence += amount;
			}
			else {
				PlayerInfluence += amount;
			}
		}
		else if (f == Faction.Enemy) {
			if (Locked) {
				// Enemies cannot influence locked cities
			}
			else if (EnemyInfluence + amount >= maxInfluence) {
				SetCompleteControl (Faction.Enemy);
			}
			else if (amount > GetNeutralInfluence ()) {
				PlayerInfluence -= (amount - GetNeutralInfluence ());
				EnemyInfluence += amount;
			}
			else {
				EnemyInfluence += amount;
			}
		}

		if (PlayerInfluence >= (maxInfluence / 2)) {
			city.SetOwner (Faction.Player);
		}
		else if (EnemyInfluence >= (maxInfluence / 2)) {
			city.SetOwner (Faction.Enemy);
		}
		else {
			city.SetOwner (Faction.Neutral);
		}
	}

	public void SetCompleteControl (Faction f) {
		if (f == Faction.Player) {
			PlayerInfluence = maxInfluence;
			lockCity ();
			EnemyInfluence = 0;
		}
		else if (f == Faction.Enemy) {
			EnemyInfluence = maxInfluence;
			PlayerInfluence = 0;
		}
	}

	private void lockCity () {
		Locked = true;
		city.SetLockSprite ();
		city.MM.UpdateMissionStatus ();
	}

	public int GetNeutralInfluence () {
		return maxInfluence - PlayerInfluence - EnemyInfluence;
	}

	/// For now this is just a simple "if connected to enemy network, add a fixed amount of influence"
	/// though it will have to at least account for absolute influence mode.
	public void CalculateInfluenceThisFrame () {
		if (city.IsConnectedToFaction (Faction.Enemy)) {
			GameModeData gmData = Util.GetCurrentGameModeData ();
			float fInfluence = ((float) (gmData.EnemyInfluence_Percentage) / 100f) * (float) (main.gs.MaxInfluence);
			influenceToAdd = Mathf.FloorToInt (fInfluence);
		}
	}

	/// Adjusts influence based on the above calculation, then resets it to 0.
	/// This is a separate function because we want all cities to be influenced at once
	/// so that they are not affected by other cities' adjustments in the same frame. 
	public void AdjustInfluence () {
		AddInfluence (Faction.Enemy, Mathf.FloorToInt (influenceToAdd));
		influenceToAdd = 0;
	}
}
