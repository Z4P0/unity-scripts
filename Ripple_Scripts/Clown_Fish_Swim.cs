using UnityEngine;
using System.Collections;

public class Clown_Fish_Swim : MonoBehaviour {

	// Unity variables
	GameObject clownFish;
	CharacterController cc;
	Animator anim;

	// Movement variables
	public float turnSpeed, accSpeed, maxSpeed, VEL, ACC, DRAG;

	

	void Start () {
		// Set vars
		turnSpeed = 2.5f;
		accSpeed = 0.5f;
		maxSpeed = 10.0f;
		VEL = 0f;
		ACC = 0f;
		DRAG = 0.08f;

		//Galacticod ENGAGE!
		clownFish = GameObject.Find("_P_Clown_Fish_Character_");
		cc = clownFish.GetComponent<CharacterController>();
		anim = clownFish.GetComponent<Animator>();
	}



	void Update () {
		// Update swim animation parameter based on current velocity
		anim.SetFloat ("Swim_Speed", VEL);

		// Update velocity
		UpdateVelocity ();
	}
	


/*
 * Velocity functions
*/
	void UpdateVelocity() 
	{
		ApplySpeed ();

		ApplyDrag ();

		// Move fish by multiplying velocity float by fish's forward facing normalized vector
		cc.Move(transform.forward*VEL * Time.deltaTime);
		
		//reset acceleration
		ACC = 0.0f;		
	}

	void ApplySpeed()
	{
		VEL += ACC;
		
		//limit speed
		if(VEL > maxSpeed){
			VEL = maxSpeed;
		}
	}

	public void ApplyDrag()
	{
		//if velocity is greater than 0, apply drag force (decrement velocity),
		//otherwise, set velocity = 0
		if(VEL > 0){
			VEL -= DRAG;
		}
		else{
			VEL = 0;
		}
	}





/*
 * Movement functions
*/
	public void MoveForward()
	{
		ACC += accSpeed;
	}
	public void TurnRight()
	{
		transform.Rotate(0,(turnSpeed/2),0,Space.World);
	}
	public void TurnLeft() 
	{
		transform.Rotate(0,(-turnSpeed/2), 0,Space.World);
	}
	public void TurnUp()
	{
		transform.Rotate(Vector3.left*turnSpeed/4,Space.Self);
	}
	public void TurnDown()
	{
		transform.Rotate(Vector3.right*turnSpeed/4,Space.Self);
	}
}
