using UnityEngine;
using System.Collections;

public class KnowledgeDrop : MonoBehaviour {

	// knowledge son
	public Texture2D icon;
	public string topic;
	public string title;

	public string[] Pages;
	public int pageIndex = 0;
	public string question;

	public string[] Answers;
	public int correctAnswer;

	public string closingFact;

	public float timeLimit;

	// basic vars
	public string mode = "inactive";
	/*
	 * inactive
	 * intro
	 * drop
	 * question
	 * finale
	 * idle
	 */


	public void TurnOn()
	{
		mode = "intro";
	}

	public void TurnOff()
	{
		mode = "idle";
	}

	public void NextPage()
	{
		pageIndex++;
		if(pageIndex > Pages.Length) pageIndex = Pages.Length - 1;
	}
	
}