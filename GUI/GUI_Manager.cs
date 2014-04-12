using UnityEngine;
using System.Collections;

public class GUI_Manager : MonoBehaviour {

	// basic things
	public bool activeGUI = false; // changed by LEAP manager
	public int padding = 50; // aesthetics

	// cursor stuff
	public Texture2D cursor;
	private Rect cursor_image;
	private Vector2 cursor_origin; // should be center of screen
	private float cursor_x;
	private float cursor_y;

	// background stuff
	public Texture2D background;
	private Rect background_image;
	private Vector2 background_origin;
	
	// continue btn stuff
	public Texture2D continueBtn;
	private Rect continueBtn_image;
	private Vector2 continueBtn_origin;

	// btn timer
	private float btnTimer = 0f;
	
	// not really sure what these new vars are for but there they are
	private Color auxColor;
	private Color cColor;

	/* ---------------- */
	// the GUI manager knows which fact we are trying to view
	public GameObject currentTrigger;

	/* ---------------- */
	







	void Start () {
		// set cursor origin
		cursor_origin = new Vector2 (Screen.width/2, Screen.height/2);

		// set outline position
		background_origin = new Vector2 (Screen.width - ((background.width/2) + padding), (background.height/2) + padding);

		// set continue btn position
		continueBtn_origin = new Vector2 (background_origin.x, background_origin.y + ((background.height/2) + (continueBtn.height/2) + (padding/2) ));
	}
	

	void OnGUI() 
	{
		auxColor = cColor = GUI.color;

		if(activeGUI)
		{
			ShowCursor();
			ShowBackground();
			ShowContinueBtn();

			// do hit test for continue
			if (continueBtn_image.Overlaps (cursor_image)) {
				Debug.Log ("continue...");
				btnTimer += Time.deltaTime;
			}

			// if we've been ther for 3 seconds
			if(btnTimer > 5.0f) {
				btnTimer = 0f;
				Debug.Log("select");
			}

		}

		GUI.color = cColor;
	}




	public void ShowCursor()
	{
		GUI.color = auxColor;
		cursor_image = new Rect (cursor_x - cursor.width/2, cursor_y - cursor.height/2, cursor.width, cursor.height);
		GUI.DrawTexture (cursor_image, cursor);
	}

	public void ShowBackground()
	{
		GUI.color = auxColor;
		background_image = new Rect (background_origin.x - background.width/2, background_origin.y - background.height/2, background.width, background.height);
		GUI.DrawTexture (background_image, background);
	}

	public void ShowContinueBtn()
	{
		GUI.color = auxColor;
		continueBtn_image = new Rect (continueBtn_origin.x - continueBtn.width/2, continueBtn_origin.y - continueBtn.height/2, continueBtn.width, continueBtn.height);
		GUI.DrawTexture (continueBtn_image, continueBtn);
	}




	// these are called by the LEAP manager
	// -------------------------------------
	public void UpdateCursor(float _x, float _y)
	{
		cursor_x = _x;
		cursor_y = Screen.height - _y;
	}
	public void TurnOnGUI(GameObject _trigger)
	{
		activeGUI = true;
		cursor_x = cursor_origin.x;
		cursor_y = cursor_origin.y;

		currentTrigger = _trigger;
	}
	public void TurnOffGUI()
	{
		Debug.Log ("turn off gui");
		activeGUI = false;
		// if we have a trigger set, cancel it
		Debug.Log (currentTrigger.ToString());
//		currentTrigger.GetComponent<GUI_Trigger> ().CancelGUI ();
	}


}
