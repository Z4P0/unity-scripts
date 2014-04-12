using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Leap;

public class LEAP_Manager : MonoBehaviour {

	// LEAP variables
	Controller controller;
	Frame frame = null;
	Hand hand = null;
	Finger finger = null;
	int fingers = 0;
	Vector3 palm;
	GestureList gestures;

	float leftOrRight;
	float upOrDown;


	// Movement variables
	private float ogX, ogY, ogZ, ogMargin; /* 'og' is for OG which stands for Original Gangsta */
	public float sensitivity = 15.0f;
	private bool controlZoneSet;
	public Vector3 handOrigin;

	// HUD vars
	private LEAP_HUD hud;
	public GUI_Manager gui;

	// swim, camera, gui, neutral, idle
	public string mode = "idle";
	
	// for tracking the change in time
	private float idleTime = 0f;
	public float idleTimeLimit = 20.0f;

	// Clownfish
	public Clown_Fish_Swim clownfish; /* this is set in the Inspector editor */
	public ClownfishOrbit orbitCamera;
	

	
	void Start () {
		// set vars
		ogX = 0f;
		ogY = 0f;
		ogZ = 0f;
		ogMargin = 20.0f;
		controlZoneSet = false;

		hud = GetComponent<LEAP_HUD> ();

		// set up LEAP
		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 500f);
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 10f);
		controller.Config.Save();
	}



	void Update () {
		// LEAP stuff
		frame = controller.Frame ();
		if(frame.Hands.Count > 0) // if we have a hand
		{
			// update variables
			hand = frame.Hands[0];
			fingers = hand.Fingers.Count;
			palm = hand.PalmPosition.ToUnity(); // ToUnity() is called from Plugins/LeapUnityExtensions.cs
			gestures = frame.Gestures ();

			leftOrRight = palm.y;
			upOrDown = palm.x * -2;


//			PalmPosition = m_Hand.PalmPosition.ToUnityTranslated();
//			PalmNormal = m_Hand.PalmNormal.ToUnity();				
//			PalmDirection = m_Hand.Direction.ToUnity();


			// are we in GUI?
			if(mode == "loading")
			{
				Debug.Log("if q swipe is dectected here, cncel the GUI");
			}
			if(mode == "gui")
			{
				/*
				 * Controls:
				 * - 1 finger or whole hand?
				 * - swipe to cancel
				 */
				if(fingers == 1)
				{
//					Debug.Log("move cursor // scaled");
					// X and Y
//					MoveCursor();
					palm = hand.PalmPosition.ToUnityScaled();
					leftOrRight = palm.y;
					upOrDown = palm.x * -2;
					Debug.Log("x, y [scaled] : " + leftOrRight + ", " + upOrDown);
					gui.UpdateCursor(leftOrRight, upOrDown);

				}

				// cancel gui activation
				if(fingers > 3)
				{
//					// be ready for a swipe cancel
//					GestureList gestures = frame.Gestures ();
//					
//					// CANCEL if there was a swipe
//					if (gestures.Count > 0) 
//					{
//						Debug.Log("CANCEL");
//						gui.TurnOffGUI();
//					}
				}
//				gui.UpdateCursor(Input.mousePosition.x, Input.mousePosition.y);

				// reset timer
				idleTime = 0f;

			} else {
			// we're swimming
				/* 	fist */
				if(fingers == 0)
				{
					mode = "neutral";
					clownfish.ApplyDrag();
					hud.direction = 0;
				}


				/* 	1 finger */
				if(fingers == 1)
				{
					if(mode == "neutral" || mode == "idle")
					{
						mode = "camera";
					}
				}


				/* 	open hand - (4 or more) */
				if(fingers > 3)
				{
					mode = "swim";
					Swim ();
				}
			}
			// reset timer
			idleTime = 0f;
			
		} else
		{
			UpdateIdleTimer (); // no one's there
		}


		// for easy debugging
		WASDInput ();
		if(mode == "gui")
		{
//			if(gui.activeGUI == false) { gui.TurnOnGUI(); }
			// update cursor position
//			gui.UpdateCursor(Input.mousePosition.x, Input.mousePosition.y);
			idleTime = 0f;
		}
	}
	





	void UpdateIdleTimer() 
	{
		idleTime += Time.deltaTime;
		if (idleTime >= idleTimeLimit)
		{
			idleTime = 0f;
			mode = "idle";
		}
	}
	



/*
 * Special Features [..:: sparkle, sparkle ::..]
*/
	void Swim()
	{
		// Check hand position
		CheckHandPosition (palm);

		// hand rotation
//		float leftOrRight = palm.y;
//		float upOrDown = palm.x * -2;
//
		
		// swipe
		hud.direction = 0;

		// if there was a swipe
		if (gestures.Count > 0) 
		{	// swim forward
			hud.direction = 1;
			clownfish.MoveForward();
		} else
		{
			// find "origin"
			// origin - the neutral zone
			// x + y
			//
//			Debug.Log("left/right: " + leftOrRight + "\nup/down: " + upOrDown);
			if (hand.PalmPosition.y < 200) { 
				clownfish.TurnLeft ();
				hud.direction = 5;
			}
			if (hand.PalmPosition.y > 350) { 
				clownfish.TurnRight (); 
				hud.direction = 3;
			}
			if (hand.PalmPosition.x > 45) { 
				clownfish.TurnDown (); 
				hud.direction = 4;
			}
			if (hand.PalmPosition.x < -65) { 
				clownfish.TurnUp (); 
				hud.direction = 2;
			}


//			float upOrDown = palm.x * -2;
			// left/right
			if(leftOrRight < ogX - sensitivity) {
				clownfish.TurnLeft();
				hud.direction = 5;
			}
			else if (leftOrRight > ogX + (sensitivity - 5)) {
				clownfish.TurnRight();
				hud.direction = 3;
			}
			
			// up/down
			if(upOrDown + (sensitivity * 2) < ogY) {
				clownfish.TurnDown();
				hud.direction = 4;
			}
			else if (upOrDown > ogY + (sensitivity * 2)) {
				clownfish.TurnUp();
				hud.direction = 2;
			}
		}
		
	}
	
	void CheckHandPosition(Vector3 newHand)
	{
		// if no origin point yet, set one
		if(!controlZoneSet) {
			SetOriginPoint(newHand);
		}
		// ----------------------

//		Debug.Log ("Hand Position: " + newHand.ToString() + "\t\tOrigin: " + handOrigin.ToString());

		// do we need to draw a new control zone?
		bool drawNewZone = false;
		
		
		/* X-axis */
		if(newHand.y > ogX + ogMargin) drawNewZone = true;	// too far right
		if(newHand.y < ogX - (ogMargin * 2)) drawNewZone = true; // too far left
		
		
		/* Y-axis */
		float yBuffer = ogY - (ogMargin * 8);
		if(newHand.x < yBuffer) drawNewZone = true;
		if(newHand.x > ogY + (ogMargin*2)) drawNewZone = true;
		
		
		/* Z-axis */
		if(newHand.z > ogZ + ogMargin) drawNewZone = true;
		if(newHand.z < ogZ - ogMargin) drawNewZone = true;
		
		// draw new zone if needed
		if(drawNewZone) SetOriginPoint(newHand);
		
	}
	void SetOriginPoint(Vector3 newHandOrigin)
	{
		ogX = newHandOrigin.y;
		ogY = newHandOrigin.x * -2;
		ogZ = newHandOrigin.z;
		controlZoneSet = true;
//		Debug.Log ("Origin: " + ogX + ", " + ogY);
	}
	
	





	
	void LookAround()
	{
		// enable/disable cameras
//		GameObject.Find ("Main Camera").camera.enabled = false;
//		GameObject.Find ("Orbit Camera").camera.enabled = true;
//
		finger = hand.Fingers [0];
		Vector3 unityFinger = finger.TipPosition.ToUnity ();
		Debug.Log (unityFinger);
//		Debug.Log (finger.TipPosition.y + ", " + finger.TipPosition.x);

//		orbitCamera.SetLEAPInput (finger.TipPosition.y, finger.TipPosition.x);
//		orbitCamera.SetLEAPInput (unityFinger.y, unityFinger.x);

//		x += finger.TipPosition.y * xSpeed * distance * 0.02f;
//		y -= finger.TipPosition.x * ySpeed * 0.02f;

	}

//	void ResetCamera()
//	{
//		GameObject.Find ("Main Camera").camera.enabled = true;
//		GameObject.Find ("Orbit Camera").camera.enabled = false;
//	}
	


/*
 * WASD input (for easy debugging)
*/
	void WASDInput()
	{
		//add to acceleration if forward movement key is being held
		if (Input.GetKey(KeyCode.W)) { clownfish.MoveForward(); hud.direction = 1; }
		if (Input.GetKey(KeyCode.S)) { clownfish.ApplyDrag(); hud.direction = 1; }
		// left/right
		if (Input.GetKey(KeyCode.D)) { clownfish.TurnRight(); hud.direction = 3; }
		if (Input.GetKey(KeyCode.A)) { clownfish.TurnLeft(); hud.direction = 5; }

		// up/down
		if (Input.GetKey ("up")) { clownfish.TurnUp(); hud.direction = 2; }
		if (Input.GetKey ("down")) { clownfish.TurnDown(); hud.direction = 4; }

		if (Input.GetKey (KeyCode.G))
		{
			mode = "gui";
		}
//		if (Input.GetKey(KeyCode.C)) { mode = "camera"; }
//		if (Input.GetKey(KeyCode.O)) { controlZoneSet = false; }
	}

}
