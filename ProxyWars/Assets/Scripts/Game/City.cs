using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class City : MonoBehaviour {

	Main main;

	public SpriteRenderer SR;
	public SpriteRenderer Outline_SR;
	public EnemyCircle EnemyCircle;

	public Faction Owner;
	public InfluenceManager IM;
	public MissionManager MM;

	private Tile tile;
	private List<City> connectedCities = new List<City> ();

	void Awake () {
		main = Util.GetMain ();
	}

	public void Setup () {
		IM.Setup (this);
		MM.GenerateMission ();
	}

	public void Tap () {
		main.uiManager.CityPopup.Activate (this);
	}

	public void SetTile (Tile t) {
		tile = t;
		t.SetCity (this);
	}

	public Tile GetTile () {
		return tile;
	}

	public void SetOwner (Faction f) {
		Owner = f;
		setColor ();
	}

	private void setColor () {
		switch (Owner) {
		case Faction.Player:
			SR.color = Util.GetMain ().l.PlayerColor;
			break;
		case Faction.Enemy:
			SR.color = Util.GetMain ().l.EnemyColor;
			break;
		case Faction.Neutral:
			SR.color = Util.GetMain ().l.NeutralColor;
			break;
		}
	}

	public List<City> GetConnectedCities () {
		return connectedCities;
	}

	public void AddConnection(City other) {
		connectedCities.Add (other);
	}

	public bool IsConnectedToCity (City other) {
		return connectedCities.Contains (other);
	}

	public bool IsConnectedToFaction (Faction f) {
		if (Owner == f) {
			return true;
		}
		foreach (City c in connectedCities) {
			if (c.Owner == f) {
				return true;
			}
		}
		return false;
	}
		
	public void CompleteMission () {
		IM.AddInfluence (Faction.Player, MM.CurrentMission.InfluenceToGain);
		MM.GenerateMission ();
	}

	public void SetLockSprite () {
		Outline_SR.color = Color.white;
	}
}