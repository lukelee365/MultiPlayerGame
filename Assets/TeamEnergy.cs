using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeamEnergy : MonoBehaviour {
	public float redTeamEnergyValue;
	public float blueTeamEnergyValue;
	public float currentRedTeamEnergy;
	public float currentBlueTeamEnergy;
	public Text redScore;
	public Text blueScore;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		redScore.text = ((int)currentRedTeamEnergy).ToString();
		blueScore.text = ((int)currentBlueTeamEnergy).ToString();
	}
	[PunRPC]
	public void ModifyRedTeamEnergy(float amt){
		// 0 - 100 just do wahtever
		if (currentRedTeamEnergy > 1f && currentRedTeamEnergy <= redTeamEnergyValue) {
			currentRedTeamEnergy -= amt;
		} else if (currentRedTeamEnergy > redTeamEnergyValue) {//Energy Greater than 100
			currentRedTeamEnergy = redTeamEnergyValue;
		} else if (currentRedTeamEnergy <= 1f) {// add with negative Taken
			if (amt <= 0f){
				currentRedTeamEnergy -= amt;// 
			}
			else{
				currentRedTeamEnergy = 0f; //not smaller than 0
			}
		}
	}
	[PunRPC]
	public void ModifyBlueTeamEnergy(float amt){
		// 0 - 100 just do wahtever
		if (currentBlueTeamEnergy > 1f && currentBlueTeamEnergy <= blueTeamEnergyValue) {
			currentBlueTeamEnergy -= amt;
		} else if (currentBlueTeamEnergy > blueTeamEnergyValue) {//Energy Greater than 100
			currentBlueTeamEnergy = blueTeamEnergyValue;
		} else if (currentBlueTeamEnergy <= 1f) {// add with negative Taken
			if (amt <= 0f){
				currentBlueTeamEnergy -= amt;// 
			}
			else{
				currentBlueTeamEnergy = 0f; //not smaller than 0
			}
		}
	}
		

//	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//	{
//		if (stream.isWriting)
//		{
//			//  send the others our data
//
//			stream.SendNext(currentRedTeamEnergy);
//			stream.SendNext(currentBlueTeamEnergy);
//		}
//		else
//		{
//			// Network , receive data
//			this.currentRedTeamEnergy = (float)stream.ReceiveNext();
//			this.currentBlueTeamEnergy = (float)stream.ReceiveNext();
//		}
//	}
}
