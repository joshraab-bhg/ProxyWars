using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResourceUI : MonoBehaviour {

	public ResourceType Resource;
	public SpriteRenderer ResourceIcon;
	public Text NumberOwnedText;
	public Text CapText;
	public Text ResourcesToGainText;
	public SpriteRenderer CrownOutline;
	public SpriteRenderer CrownIcon;
	public Text CrownCostText;
	public Text ResourcesFromPurchaseText;
	private bool fadingOut;

	void Update () {
		/*
		 * Probably completely unnecessary - resources are not "used" on their own, just spent on missions
		if (Input.GetMouseButtonDown (0)) {
			Vector2 worldPos = new Vector2 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, Camera.main.ScreenToWorldPoint (Input.mousePosition).y);
			RaycastHit2D hit = Physics2D.Raycast (worldPos, Vector2.zero, 10000, 1 << LayerMask.NameToLayer ("Clickables"));
			if (hit.collider != null && hit.collider.gameObject == ItemIcon.gameObject) {
				Util.GetMain ().p.TryToUseItem (Item);
			}
		} */
	}

	public void Setup (ResourceType type) {
		Main main = Util.GetMain ();
		ResourceData resourceData = main.resourceLibrary.GetResourceData (type);
		ModeResourceData modeResourceData = Util.GetCurrentGameModeData ().GetResourceData (type);

		Resource = type;
		ResourceIcon.sprite = resourceData.Icon;
		CapText.text = "/ " + modeResourceData.ResourceCap;
		ResourcesToGainText.text = "(+" + modeResourceData.AmountGenerated + ")";
		CrownIcon.sprite = main.l.CrownSprite;
		CrownCostText.text = resourceData.CrownCost + "";
		ResourcesFromPurchaseText.text = "(+" + modeResourceData.GetResourcesFromPurchase () + ")";
	}
}
