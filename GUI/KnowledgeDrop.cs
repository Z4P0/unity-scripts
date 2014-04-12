using UnityEngine;
using System.Collections;

public class KnowledgeDrop : MonoBehaviour {

	// knowledge son
	public Texture2D icon;
	public string topic;
	public string title;

	public string[] Pages;
	public string question;

	public string[] Answers;
	public int correctAnswer;

	public string closingFact;

	public float timeLimit;

	// basic vars
	private bool activated = false;
	private int pageIndex = 0;
	private float timer = 0.0f;

	// mode things
	/*
	 * inactive = 0
	 * intro = 1
	 * drop = 2
	 * question = 3
	 * finale = 4
	 * idle = 5
	 */
	public string mode = "inactive";
//	public int currentMode = 0;


	public void TurnOn()
	{
		activated = true;
		mode = "intro";
	}

	public void Next()
	{
//		currentMode++;
	}

	void OnGUI () {
		if (activated)
		{
			if(mode == "intro")
			{
				Intro();

				// animate intro icon
				timer += Time.deltaTime;
				if(timer > 5.0f)
				{
					mode = "drop";
					timer = 0.0f;
				}
			}
			
			if(mode == "drop")
			{
				ShowPage(pageIndex);


//				timer += Time.deltaTime;
//				if (timer > timeLimit)
//				{
//					NextPage();
//					if(pageIndex > Pages.Length-1)
//					{
//						mode = "question";
//						timer = 0.0f;
//					}
//				}
			}
			
			if(mode == "question")
			{
				AskQuestion();
				ListAnswers();
				timer += Time.deltaTime;
				if (timer > timeLimit)
				{
					mode = "finale";
					timer = 0.0f;
				}
			}
			
			if(mode == "finale")
			{
				Finale();
				timer += Time.deltaTime;
				if (timer > timeLimit)
				{
					mode = "inactive";
					activated = false;
					timer = 0.0f;
				}
			}
		}

	}


	public void Stop()
	{
		activated = false;
	}

	public void Intro()
	{
		Debug.Log ("Topic: " + topic);

//		guiText.text = topic;
//		guiText.transform.position = new Vector2 (50, 50);
//
//		topic.transform.position = new Vector2(50, 50);

		// show icon
		// show title
//		GUI.Box(new Rect(150, 150, 500, 250),topic);

		// do animation

		// when done change mode to drop (this is where we start to drop knowledge)
//		mode = "intro";
	}



	// Pages
	// ----------------------------
	public void ShowPage(int page)
	{
		Debug.Log (Pages[page]);
//		GUI.Box(new Rect(150, 150, 500, 250),Pages[page]);
		
	}
	public void NextPage()
	{
		pageIndex++;
	}



	// Question/Answer
	// ----------------------------
	public void AskQuestion()
	{
		Debug.Log ("Question: " + question);
	}
	public void ListAnswers()
	{
		for (int x = 0; x < Answers.Length; x++) 
		{
			Debug.Log((x+1) + ").\t" + Answers[x]);
		}
	}


	// Finale
	// ----------------------------
	public void Finale()
	{
		Debug.Log (closingFact);
	}
}
