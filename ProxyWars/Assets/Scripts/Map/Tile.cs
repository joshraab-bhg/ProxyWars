using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public int X;
	public int Y;

	private City city;

	public void SetCity (City c) {
		city = c;
	}

	public City GetCity () {
		return city;
	}

	public bool HasCity () {
		return city != null;
	}
}
