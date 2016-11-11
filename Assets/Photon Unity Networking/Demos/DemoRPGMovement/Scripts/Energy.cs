using UnityEngine;
using System.Collections;

public class Energy : Photon.MonoBehaviour {
	public float energyValue = 100f;
	public Transform backPack;
	public float  currentEnergy;
	float backPackScaleY;
	private RPGMovement moveScript;
	public bool isRedTeam;
	public TextMesh textMesh_PlayerName;
	private bool redColor;
	private string playerName;
	// Use this for initialization
	void Start () {
		currentEnergy = energyValue;
		backPackScaleY = backPack.localScale.y;
		moveScript = GetComponent<RPGMovement> ();
	

	}
	
	// Update is called once per frame
	void Update(){
//		if(playerName==null){
			playerName = this.photonView.owner.name;
			textMesh_PlayerName.text = playerName;
			if (isRedTeam) {				
				textMesh_PlayerName.color = Color.red;
			} else {			
				textMesh_PlayerName.color = Color.blue;
			}
//		}
		backPack.localScale = new Vector3 (backPack.localScale.x, backPackScaleY * currentEnergy / energyValue, backPack.localScale.z);
	}

	[PunRPC]
	public void TakenEnergy(float amt){
		
		// 0 - 100 just do whatever
		if (currentEnergy > 1f && currentEnergy <= energyValue) {
			currentEnergy -= amt;
			moveScript.EnableControl(true); 
		} else if (currentEnergy > energyValue) {//Energy Greater than 100
			currentEnergy = energyValue;
		} else if (currentEnergy <= 1f) {// not Energy will refill with negative Taken
			moveScript.EnableControl(false); // Disable Character control
			if (amt <= 0f){
				currentEnergy -= amt;// not Energy will refill with negative Taken
			}
			else{
				currentEnergy = 0f; //not smaller than 0
			}
		}
	}
	//[PunRPC]
	public void IsInRedTeam(bool b){
		isRedTeam = b;
	}

	[PunRPC]
	public float getEnergy(){
		return currentEnergy;
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(currentEnergy);
			stream.SendNext(playerName);
			stream.SendNext(isRedTeam);
		}
		else
		{
			// Network player, receive data
			this.currentEnergy = (float)stream.ReceiveNext();
			this.playerName =(string)stream.ReceiveNext();
			this.isRedTeam = (bool)stream.ReceiveNext();
		}
	}
}
