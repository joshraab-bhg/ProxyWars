using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

	public Main main;

	public int Width;
	public int Height;

	public Tile[,] TileGrid;
	public List<City> Cities;
	public List<City> CitiesOnLeft = new List<City>();
	public List<City> CitiesOnRight = new List<City>();

	public City PlayerStartCity;
	public City EnemyStartCity;

	private float enemyInfluenceTime;

	public void Awake () {
		main = Util.GetMain ();
		SetEnemyInfluenceTimeToMax ();
	}

	public void Start () {
	}

	public void FrameTick () {
		enemyInfluenceTime -= Time.deltaTime;

		// Calculate, then adjust enemy influence in all cities
		if (enemyInfluenceTime <= 0) {
			foreach (City city in Cities) {
				city.IM.CalculateInfluenceThisFrame ();
			}
			foreach (City city in Cities) {
				city.IM.AdjustInfluence ();
			}
			SetEnemyInfluenceTimeToMax ();
		}
	}

	public void SetEnemyInfluenceTimeToMax () {
		enemyInfluenceTime += Util.GetCurrentGameModeData ().EnemyInfluence_Seconds;
	}

	public float GetEnemyInfluenceTime () {
		return enemyInfluenceTime;
	}
}
