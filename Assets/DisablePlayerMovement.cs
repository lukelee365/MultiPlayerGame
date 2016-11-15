using UnityEngine;
using System.Collections;

public class DisablePlayerMovement : MonoBehaviour {


	private RPGMovement playerRPG;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PassPlayer(GameObject o){
		
		playerRPG = o.GetComponent<RPGMovement> ();

	}

  

    public void PlayerCanMoveNow(){
		//enable the movement
		Cursor.visible = false;
		playerRPG.anotherMovementControlForButton = true;

	}

}
