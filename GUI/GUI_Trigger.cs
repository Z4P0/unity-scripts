using UnityEngine;
using System.Collections;

public class GUI_Trigger : MonoBehaviour {

	private float triggerTimer;
	public float timerLimit = 5.0f;
	private bool activated = false;

	public GUI_Manager gui;
	private KnowledgeDrop kDrop;

	void Start ()
	{
		kDrop = GetComponent<KnowledgeDrop> ();
	}

	void OnTriggerStay()
	{
		if (!activated) {
			triggerTimer += Time.deltaTime;
			Debug.Log("loading...");

			// activate after a few seconds
			if (triggerTimer > timerLimit) {
				activated = true;
				gui.TurnOnGUI(this.gameObject);

				// start knowledge drop
				kDrop.TurnOn();
			}
		}
	}

	void OnTriggerExit()
	{
		activated = false;
		triggerTimer = 0.0f;
		kDrop.TurnOff ();
		gui.TurnOffGUI ();
	}

	public void CancelTrigger()
	{
		triggerTimer = 0.0f;
		// add to time limit
		timerLimit = timerLimit + timerLimit / 2;
	}
}
