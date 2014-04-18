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

	// vars for LEAP data
	float leftOrRight;
	float upOrDown;
	

	// set LEAP ranges
	public float horizontal_range = 15f; 	// 15 cm
	public float vertical_range = 12f;		// 12 cm

	public float leapHorizontalRange = 500f;// based on output from the leap
	public float leapVerticalRange = 600f;



	// setting divisions for LEAP control
	private float neutral_zone;

	private float left_box;
	private float right_box;
	private float center_box;

	private float top_box;
	private float bottom_box;



	// mode bools
	/* HEY YO. THIS IS JUST FOR DEBUGGING.*/
	public bool guiMode = true;
	/* UNCOMMENT THE LINE BELOW */
	// public bool guiMode = false;

	public string mode = "idle";
	
	// for tracking the change in time
	private float idleTime = 0f;
	public float idleTimeLimit = 20.0f;



	
	// Clownfish
	public Clown_Fish_Swim clownfish; /* this is set in the Inspector editor */
	public ClownfishOrbit orbitCamera;



	// other scripts
	private LEAP_HUD hud;
	public GUI_Manager gui;




	
	void Start () {
		// HUD
		hud = GetComponent<LEAP_HUD> ();

		// set up LEAP
		controller = new Controller();
		controller.EnableGesture (Gesture.GestureType.TYPESWIPE);
		controller.Config.SetFloat("Gesture.Swipe.MinVelocity", 500f);
		controller.Config.SetFloat("Gesture.Swipe.MinLength", 10f);
		controller.Config.Save();

		// set control area
		SetupLEAP ();
	}

	void SetupLEAP() {
		// 15 cm = LEAP area
		/*
		 * 	neutral zone = 2 / 15 = 13%
		 * 	control zone = 13 / 15 = 87%
		 * 		left box = 5 / 13 = 38%
		 * 		center box = 3 / 13 = 24%
		 * 		right box = 5 / 13 = 38%
		 */

		// neutral area
		neutral_zone = (2 / horizontal_range) * leapHorizontalRange;	// 0 - 2cm

		// horizontal zones (left / right)
		left_box = (4.5f / horizontal_range) * leapHorizontalRange;		// 2cm - 7cm
		right_box = (4.5f / horizontal_range) * leapHorizontalRange;	// 10cm - 15cm
		center_box = (4 / horizontal_range) * leapHorizontalRange;		// 7cm - 10cm

		// vertical zones
		top_box = (3 / vertical_range) * leapVerticalRange; 			// 8cm - 12cm
		bottom_box = top_box / 2;										// 0 - 4cm
	}


	void Update () {
		// LEAP stuff
		frame = controller.Frame ();

		// if we have a hand
		if(frame.Hands.Count > 0)
		{
			// update variables
			hand = frame.Hands[0];
			fingers = hand.Fingers.Count;
			palm = hand.PalmPosition.ToUnity(); // ToUnity() is called from Plugins/LeapUnityExtensions.cs
			gestures = frame.Gestures ();

			leftOrRight = palm.y;
			upOrDown = palm.x * -2;


			// are we in GUI mode, or free swim?
			if(guiMode) {
				GuiControls();
			} else {
				// free swim
				Swim();
			}

		}
		// end hand check
		// (hand check? what is this high school? thanks mom)



		// for easy debugging
		WASDInput ();
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

		/* 	fist	=	brake	*/
		if(fingers == 0) ApplyDrag();
		
		
		/* 	1 finger	=	camera	*/
		if(fingers == 1)
		{
			Debug.Log("camera");
		}
		
		
		/* 	open hand (3 or more)	=	swime	*/
		if(fingers > 2)
		{
			// swipe = swim forward
			if (gestures.Count > 0) 
			{
				MoveForward();
			}
			
			
			/* LEFT or RIGHT */
			if(leftOrRight > neutral_zone) {
				// left box
				if(leftOrRight < neutral_zone + left_box) {
					TurnLeft();
				}
				// center box
				if(leftOrRight < neutral_zone + left_box + center_box && leftOrRight > neutral_zone + left_box + center_box) {
					Debug.Log("- -");
				}
				// right box
				if(leftOrRight > neutral_zone + left_box + center_box) {
					TurnRight();
				}
			} else {
				Debug.Log("in the neutral zone");
			}
			
			/* UP or DOWN */
			if(upOrDown > top_box * 3) {
				TurnUp();
			} else if(upOrDown < bottom_box) {
				TurnDown();
			}
		}

	}

	

	// Movement functions
	// - speed up/down
	void MoveForward() {
		clownfish.MoveForward();
		hud.direction = 1;
	}
	void ApplyDrag() {
		clownfish.ApplyDrag();
		hud.direction = 1;
	}

	// Turn functions
	// - move the clownfish & update HUD
	void TurnLeft(){
		clownfish.TurnLeft();
		hud.direction = 5;
	}
	void TurnRight(){
		clownfish.TurnRight();
		hud.direction = 3;
	}
	void TurnUp(){
		clownfish.TurnUp();
		hud.direction = 2;
	}
	void TurnDown(){
		clownfish.TurnDown();
		hud.direction = 4;
	}






/* GUI Interactions */
	void GuiControls() {
		if (fingers >= 1) {
			Debug.Log("move cursor");
//			Debug.Log(leftOrRight);
//			Debug.Log(upOrDown);
//			gui.UpdateCursor(leftOrRight, upOrDown);
			/* LEFT or RIGHT */
//			if(leftOrRight > neutral_zone) {
//				// left box
//				if(leftOrRight < neutral_zone + left_box) {
//					gui.MoveCursorLeft();
//				}
//				// center box
//				if(leftOrRight < neutral_zone + left_box + center_box && leftOrRight > neutral_zone + left_box + center_box) {
//					Debug.Log("- -");
//				}
//				// right box
//				if(leftOrRight > neutral_zone + left_box + center_box) {
//					gui.MoveCursorRight();
//				}
//			} else {
//				Debug.Log("in the neutral zone");
//			}
//			
//			/* UP or DOWN */
//			if(upOrDown > top_box * 3) {
//				gui.MoveCursorUp();
//			} else if(upOrDown < bottom_box) {
//				gui.MoveCursorDown();
//			}
		
			gui.UpdateCursor(leftOrRight , upOrDown );
		
		
		}
//				// cancel gui activation
//				if(fingers > 2)
//				{
////					// be ready for a swipe cancel
////					GestureList gestures = frame.Gestures ();
////					
////					// CANCEL if there was a swipe
////					if (gestures.Count > 0) 
////					{
////						Debug.Log("CANCEL");
////						gui.TurnOffGUI();
////					}
//				}
//		gui.UpdateCursor(Input.mousePosition.x, Input.mousePosition.y);

	}

	
//	void LookAround()
//	{
//		// enable/disable cameras
////		GameObject.Find ("Main Camera").camera.enabled = false;
////		GameObject.Find ("Orbit Camera").camera.enabled = true;
////
//		finger = hand.Fingers [0];
//		Vector3 unityFinger = finger.TipPosition.ToUnity ();
//		Debug.Log (unityFinger);
////		Debug.Log (finger.TipPosition.y + ", " + finger.TipPosition.x);
//
////		orbitCamera.SetLEAPInput (finger.TipPosition.y, finger.TipPosition.x);
////		orbitCamera.SetLEAPInput (unityFinger.y, unityFinger.x);
//
////		x += finger.TipPosition.y * xSpeed * distance * 0.02f;
////		y -= finger.TipPosition.x * ySpeed * 0.02f;
//
//	}

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
		if (Input.GetKey(KeyCode.W)) { MoveForward(); }
		if (Input.GetKey(KeyCode.S)) { ApplyDrag(); }
		// left/right
		if (Input.GetKey(KeyCode.D)) { TurnRight(); }
		if (Input.GetKey(KeyCode.A)) { TurnLeft(); }

		// up/down
		if (Input.GetKey ("up")) { TurnUp(); }
		if (Input.GetKey ("down")) { TurnDown(); }
	}

}
