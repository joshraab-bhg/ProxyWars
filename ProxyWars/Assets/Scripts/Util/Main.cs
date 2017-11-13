using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	public Library l;
	public UILibrary uiLibrary;
	public ResourceLibrary resourceLibrary;

	public GameSettings gs;
	public MapSettings ms;

	public UIManager uiManager;
	public ResourceUIManager resourceUIManager;

	public Timer timer;

	public Player p;

	// Lower numbers will be drawn in front of higher numbers
	public const int Z_CAMERA = -10;
	public const int Z_UI = -9;
	public const int Z_CITY = 5;
	public const int Z_CONNECTION = 7;
	public const int Z_TILE_BG = 10;

	void Start () {
		generateMap ();
	}
	
	void Update () {
		handleMouseInput ();
		handleKeyboardDebugInput ();
	}
		
	private void generateMap () {
		MapGenerator mg = GameObject.Find ("Map Generator").GetComponent<MapGenerator> ();
		mg.GenerateNewMap ();
	}

	private void handleMouseInput () {

		// Tappin
		if (Input.GetMouseButtonDown (0)) {
			Vector2 worldPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast (worldPos, Vector2.zero, 10000, 1 << LayerMask.NameToLayer ("Clickables"));
			if (hit.collider != null) {
				GameObject go = hit.collider.gameObject;

				if (go.GetComponent<CompleteMissionButton> () != null) {
					uiManager.CityPopup.TryToCompleteMission ();
				}

				else if (go.GetComponent<City> () != null) {
					go.GetComponent<City> ().Tap ();
				}
			}
			else {
				uiManager.CityPopup.gameObject.SetActive (false);
			}
		}
	}

	private void handleKeyboardDebugInput () {
		if (Input.GetKeyDown ("r")) {
			UnityEngine.SceneManagement.SceneManager.LoadScene (0);	
		}

		if (Input.GetKeyDown ("p")) {
			if (!timer.IsTimeOver ()) {
				if (timer.Ticking) {
					timer.Pause ();
				}
				else {
					timer.Unpause ();
				}
			}
		}
	}


}
