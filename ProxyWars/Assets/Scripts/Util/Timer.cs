using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

	private Main main;
	private Map map;

	private int secondsLeft;
	private bool timeOver;

	public bool Ticking;
	private float lastTickTime;
	private float currentTime;
	private bool wasPausedLastFrame;
	private float pauseTime;

	void Awake () {
		main = Util.GetMain ();
		map = Util.GetMap ();
	}

	void Start () {
		Reset ();
		Ticking = true;
		timeOver = false;
	}

	void Update () {		
		if (wasPausedLastFrame) {
			lastTickTime += Time.deltaTime;
		}

		if (Ticking) {
			currentTime += Time.deltaTime;
			frameTick ();

			if (wasPausedLastFrame) {
				wasPausedLastFrame = false;
			}
		}
		else {
			wasPausedLastFrame = true;
		}

		if (currentTime - lastTickTime >= 1) {
			lastTickTime = currentTime;
			secondTick ();
		}

		if (main.gs.FreezeOnTimeOver && secondsLeft <= 0 && !timeOver) {
			EndTime ();
		//	main.uiManager.TimeOverScreen.SetActive (true);
		}
	}

	public void Pause () {
		Ticking = false;
		main.uiManager.PauseText.gameObject.SetActive (true);
	}

	public void Unpause () {
		Ticking = true;
		main.uiManager.PauseText.gameObject.SetActive (false);
	}

	public void EndTime () {
		timeOver = true;
		Ticking = false;
		main.uiManager.TimeOverText.gameObject.SetActive (true);
//		Util.GetMap ().Freeze ();
	}

	public bool IsTimeOver () {
		return timeOver;
	}

	public int GetSecondsLeft () {
		return secondsLeft;
	}

	public void Reset () {
		secondsLeft = Util.GetCurrentGameModeData().MaxTime;
		timeOver = false;
		Ticking = true;
	}

	private void frameTick () {
		map.FrameTick ();
	}

	private void secondTick () {
		secondsLeft--;
		main.p.SecondTick ();
		main.uiManager.SecondTick ();
	}
}