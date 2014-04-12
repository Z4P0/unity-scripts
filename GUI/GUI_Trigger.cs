using UnityEngine;
using System.Collections;

public class GUI_Trigger : MonoBehaviour {

	private float triggerTimer;
	public float timerLimit;
	private bool acivated = false;

	public LEAP_Manager leap;
	public GUI_Manager gui;
	private KnowledgeDrop kDrop;

	void Start ()
	{
		timerLimit = 5.0f;
		kDrop = GetComponent<KnowledgeDrop> ();
	}


	void OnTriggerExit()
	{
		CancelGUI ();
	}

	void OnTriggerStay()
	{
		if (!acivated) {
			triggerTimer += Time.deltaTime;
			Debug.Log("loading gui...");

			// activate after a few seconds
			if (triggerTimer > timerLimit) {
				acivated = true;
				leap.mode = "gui";
//				Debug.Log(this.gameObject);
				gui.TurnOnGUI(this.gameObject);

				kDrop.Intro();
			}

			// reset activation if user swipes
			if(Input.GetKey(KeyCode.Escape)) CancelGUI();
		}
	}

	public void CancelGUI()
	{

		acivated = false;
		triggerTimer = 0.0f;

		leap.mode = "swim";

		// hide everything
		gui.TurnOffGUI ();
	}
}
