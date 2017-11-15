using UnityEngine;
using System.Collections;

public class CityCounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int myCity = transform.parent.GetComponent<City>().cityId;
		GetComponent<TextMesh>().text = myCity.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
