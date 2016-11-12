using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class DemoRPGMovement : MonoBehaviour 
{
    public RPGCamera Camera;
	public Transform spawnPositionRed;
	public Transform spawnPositionBlue;
	public Transform spawnPositionBall;
	public float PositionOffset = 2.0f;
	public GameObject inputButton;

    public GameObject control;
    void OnJoinedRoom()
    {
		//Instantiate The Ball to Push
		Vector3 ballposition = spawnPositionBall.position;

		if (PhotonNetwork.isMasterClient)  {
			GameObject Ball = PhotonNetwork.Instantiate( "Ball", ballposition, Quaternion.identity, 0 );
		}
		AssignedTeams ();
    }

	void CreatePlayerObject(Transform spawnPosition,bool isRed)
    {
		Vector3 spawnPos = Vector3.up;
		if (spawnPosition != null)
		{
			spawnPos = spawnPosition.position;
		}
		Vector3 random = Random.insideUnitSphere;
		random.y = 0;
		random = random.normalized;
		spawnPos = spawnPos + this.PositionOffset * random;
		GameObject newPlayerObject = PhotonNetwork.Instantiate( "Robot Kyle RPG", spawnPos, Quaternion.identity, 0 );
        Camera.Target = newPlayerObject.transform;
		newPlayerObject.SendMessage ("IsInRedTeam",isRed);
		inputButton.SendMessage ("PassPlayer",newPlayerObject);
        control.SendMessage("PassPlayer2", newPlayerObject);


    }

	void AssignedTeams(){
		//RedTeamList
		List<PhotonPlayer> redTeamPlayers = PunTeams.PlayersPerTeam[PunTeams.Team.red];
		//BlueTeamList
		List<PhotonPlayer> blueTeamPlayers = PunTeams.PlayersPerTeam[PunTeams.Team.blue];
		if (blueTeamPlayers.Count <= redTeamPlayers.Count) {//blue less or equal to redteam
			CreatePlayerObject(spawnPositionBlue,false);
			PhotonNetwork.player.SetTeam (PunTeams.Team.blue);//join blue team
		} else {
			CreatePlayerObject(spawnPositionRed,true);
			PhotonNetwork.player.SetTeam (PunTeams.Team.red);//join red team
		}
	
	}
}
