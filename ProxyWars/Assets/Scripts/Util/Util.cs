using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Util {

	public static Main GetMain () {
		return GameObject.Find ("Main").GetComponent<Main> ();
	}

	public static Map GetMap () {
		return GameObject.Find ("Map").GetComponent<Map> ();
	}

	public static GameModeData GetCurrentGameModeData () {
		return GetGameModeData (GetMain().gs.GameMode);
	}

	public static GameModeData GetGameModeData (GameModes mode) {

		if (mode == GameModes.FastMode) {
			return GameObject.Find ("Fast Mode").GetComponent <GameModeData> ();
		}

		Debug.LogWarning ("Unsupported mode. Returning fast mode by default");
		return GameObject.Find ("Fast Mode").GetComponent <GameModeData> ();
	}

	public static string GetFormattedTime (int seconds) {
		int mins = seconds / 60;
		int secs = seconds % 60;

		string secString = "";
		if (secs < 10) {
			secString = "0";
		}
		secString += secs;

		string minString = mins + "";
		return minString + ":" + secString;
	}

	public static string GetFormattedPercentage (float perc) {
		string s = "";
		s = perc.ToString ();
		if (s.Length > 4) {
			s = s.Substring (0, 4);
		}
		return s + "%";
	}

	public static ResourceType GetRandomResourceType () {
		List<ResourceType> resourceTypesInUse = new List<ResourceType> ();

		foreach (ResourceType rt in Util.GetMain ().resourceUIManager.ResourcesInUse) {
			resourceTypesInUse.Add (rt);
		}

		return resourceTypesInUse [Random.Range (0, resourceTypesInUse.Count)];
	}

	public static float GetInfluencePercentage (int rawInfluence) { 
		float fMaxInfluence = (float) Util.GetMain ().gs.MaxInfluence;
		return ((float) rawInfluence) / fMaxInfluence * 100f;
	}

	public static int GetPerc (int num, float perc) {
		return (int) ((float)num * perc);

	}
}
