using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	private Main main;
	private Map map;

	private const float ENEMY_CIRCLE_MAX_SIZE = 2f;
	private const float ENEMY_CIRCLE_MIN_SIZE = 0.6f;
	int enemyInfluenceSeconds;

	public Text TimerText;
	public Text PauseText;
	public Text GainResourcesText;
	public Text EnemyInfluenceText;
	public Text TimeOverText;
	public Text CrownsText;

	private const float BUBBLE_TEXT_DELAY = 1f;
	private float lastMessageTime_NotEnoughCrowns;

	public GameObject VictoryScreen;
	public GameObject TimeOverScreen;

	public CityPopupUI CityPopup;

	void Awake () {
		main = Util.GetMain ();
		map = Util.GetMap ();

//		TimerText.color = main.uiLibrary.GenericTextColor;
	}

	void Start () {
		enemyInfluenceSeconds = Util.GetCurrentGameModeData ().EnemyInfluence_Seconds;
		updateTimer ();
		updateEnemyInfluenceText ();
	}

	void Update () {
		updateCrowns ();
		updateResources ();
		updateEnemyCircles ();
	}

	public void SecondTick () {
		updateTimer ();
		updateEnemyInfluenceText ();
		CityPopup.SecondTick ();
	}
				
	private void updateTimer () {
		TimerText.text = "Time left: " + Util.GetFormattedTime (main.timer.GetSecondsLeft ());
	}

	private void updateCrowns () {
		string crownsString = "" + main.p.GetCrowns ();
		CrownsText.text = crownsString;
	}

	private void updateResources () {
		string resourcesString = "Gain resources in ";
		resourcesString += main.p.GetResourceGainTime () + "s";
		GainResourcesText.text = resourcesString;
	}

	private void updateEnemyInfluenceText () {
		string enemyInfluenceString = "Enemy influence in ";
		enemyInfluenceString += Mathf.CeilToInt (map.GetEnemyInfluenceTime ()) + "s";
		EnemyInfluenceText.text = enemyInfluenceString;
	}

	private void updateEnemyCircles () {
		float perc = map.GetEnemyInfluenceTime () / enemyInfluenceSeconds;

		foreach (City city in map.Cities) {
			if (city.IsConnectedToFaction (Faction.Enemy) && !city.IM.Locked && city.IM.EnemyInfluence < 100) {
				city.EnemyCircle.gameObject.SetActive (true);
				float radius = Mathf.Lerp (ENEMY_CIRCLE_MIN_SIZE, ENEMY_CIRCLE_MAX_SIZE, perc);
				city.EnemyCircle.xradius = radius;
				city.EnemyCircle.yradius = radius;
				city.EnemyCircle.CreatePoints ();
			}
			else {
				city.EnemyCircle.gameObject.SetActive (false);
			}
		}
	}
}