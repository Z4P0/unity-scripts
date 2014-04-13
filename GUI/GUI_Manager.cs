using UnityEngine;
using System.Collections;

public class GUI_Manager : MonoBehaviour {

	// basic things
	public bool activeGUI = false; // changed by LEAP manager

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
	private float guiTimer = 0f;
	public float guiTimeLimit = 5.0f;
	
	// not really sure what these new vars are for but there they are
	private Color auxColor;
	private Color cColor;

	/* ---------------- */
	// the GUI manager knows which fact we are trying to view
	public GameObject currentTrigger;
	public KnowledgeDrop currentKDrop;

	/* ---------------- */
	
	/* TEXT SHIITE */
	public GUIText topicText;
	public GUIText titleText;
	public GUIText bodyText;

	public GUIText[] answerTexts;




	/* vars for positioning text */
	private bool factReady = false;
	private float width;
	private float height;
	public int padding = 25; // aesthetics
	public int margin = 50;
	// starting points for text placement
	private Vector2 topTextPosition;
	private Vector2 bottomTextPosition;















	void Start () {
		// set cursor origin
		cursor_origin = new Vector2 (Screen.width/2, Screen.height/2);

		// set outline position
		background_origin = new Vector2 (Screen.width - ((background.width/2) + margin), (background.height/2) + margin);

		// set continue btn position
		continueBtn_origin = new Vector2 (background_origin.x, background_origin.y + ((background.height/2) + (continueBtn.height/2) + (margin/2) ));

		TurnOffGUIText ();

		width = background.width;
		height = background.height;
	}
	

	void OnGUI() 
	{
		auxColor = cColor = GUI.color;

		if(activeGUI)
		{
			ShowCursor();
			ShowBackground();
			ShowKnowledge();
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

	public void ShowKnowledge()
	{
		// get the current state of the Knowledge Drop
//		Debug.Log (currentKDrop.mode);

		if(currentKDrop.mode == "intro")
		{
			if(factReady){
				ShowIntro();
			} else {
				SetIntro();
			}

		}

						
		if(currentKDrop.mode == "drop")
		{
			if(factReady) {
			} else {
				DropKnowledgeSetup();
				// lol
			}
		}
			//			
			//			if(mode == "question")
			//			{
			//				AskQuestion();
			//				ListAnswers();
			//				timer += Time.deltaTime;
			//				if (timer > timeLimit)
			//				{
			//					mode = "finale";
			//					timer = 0.0f;
			//				}
			//			}
			//			
			//			if(mode == "finale")
			//			{
			//				Finale();
			//				timer += Time.deltaTime;
			//				if (timer > timeLimit)
			//				{
			//					mode = "inactive";
			//					activated = false;
			//					timer = 0.0f;
			//				}
			//			}

		// this set the text of the GUI
		// sets the position of the text
	}


	// these are called by the LEAP manager
	// -------------------------------------
	public void UpdateCursor(float _x, float _y)
	{
		cursor_x = _x;
		cursor_y = Screen.height - _y;
	}



	public void SetIntro()
	{
		bodyText.enabled = false;
		titleText.enabled = false;
		
		topicText.text = "Intro";
//		topicText.text = currentKDrop.topic;
		//		topicText.text = "asdkfaldkfasdlfkasdlfkjasdlfkjajhsdlfkasjdflkasjd;flaksjdf;laksjdf;laksdjf;alksdjfa;lskdj";
		
		float newX = -margin - background.width / 2 - topicText.GetScreenRect ().width / 2;
		float newY = -margin - topicText.GetScreenRect().height / 2;
		topTextPosition = new Vector2 (newX, newY);
		
		topicText.pixelOffset = topTextPosition;

		factReady = true;
	}

	public void ShowIntro()
	{
		guiTimer += Time.deltaTime;
		if(guiTimer > guiTimeLimit)
		{
			currentKDrop.mode = "drop";
			factReady = false;
		}
		// set its position
//		topicText.pixelOffset = new Vector2 (0, Screen.height - 50);

		//		topicText.enabled = true;

		// show icon
		// show title

		// do animation

		// when done change mode to drop (this is where we start to drop knowledge)
//		mode = "intro";
	}



	public void DropKnowledgeSetup()
	{
		topicText.enabled = true;
		bodyText.enabled = true;

		topicText.text = currentKDrop.topic;
		bodyText.text = currentKDrop.Pages[currentKDrop.pageIndex];

		bottomTextPosition = topTextPosition - new Vector2 (0, padding);
		bodyText.pixelOffset = bottomTextPosition;
		factReady = true;
	}









	/*
	 * GUI stuff
	 */
	public void TurnOnGUI(GameObject _trigger)
	{
		activeGUI = true;
		cursor_x = cursor_origin.x;
		cursor_y = cursor_origin.y;

		// set current trigger & kDrop
		currentTrigger = _trigger;
		currentKDrop = currentTrigger.GetComponent<KnowledgeDrop> ();

		TurnOnGUIText ();

//		float newX = -margin - background.width / 2 - topicText.GetScreenRect ().width / 2;
//		float newY = -margin - topicText.GetScreenRect().height / 2;
//		topTextPosition = new Vector2 (newX, newY);
		
		//		Debug.Log (newY);
//		topicText.pixelOffset = new Vector2 (-margin - background.width/2 - topicText.GetScreenRect().width/2, newY); 
		// set its position
	}
	public void TurnOffGUI()
	{
		activeGUI = false;
		TurnOffGUIText ();
	}

	void TurnOffGUIText()
	{
		// turn off GUI text
		topicText.enabled = false;
		titleText.enabled = false;
		bodyText.enabled = false;
	}
	void TurnOnGUIText()
	{
		// turn on GUI text
		topicText.enabled = true;
		titleText.enabled = true;
		bodyText.enabled = true;
	}







	
//	// Pages
//	// ----------------------------
//	public void ShowPage(int page)
//	{
//		Debug.Log (Pages[page]);
//		//		GUI.Box(new Rect(150, 150, 500, 250),Pages[page]);
//		
//	}
//	public void NextPage()
//	{
//		pageIndex++;
//	}
//	
	
	
//	// Question/Answer
//	// ----------------------------
//	public void AskQuestion()
//	{
//		Debug.Log ("Question: " + question);
//	}
//	public void ListAnswers()
//	{
//		for (int x = 0; x < Answers.Length; x++) 
//		{
//			Debug.Log((x+1) + ").\t" + Answers[x]);
//		}
//	}
//	
//	
//	// Finale
//	// ----------------------------
//	public void Finale()
//	{
//		Debug.Log (closingFact);
//	}
//

}
