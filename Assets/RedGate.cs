using UnityEngine;
using System.Collections;

public class RedGate : MonoBehaviour {
	public GameObject something;
	private TeamEnergy tEnergy;
	public bool isRedGate;
	public float energyToBeAdded;
	public Transform ballResapwnPoint;
	// Use this for initialization
	void Start () {
		tEnergy = GameObject.FindGameObjectWithTag ("Manager").GetComponent<TeamEnergy> ();
		something.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//ball score 
	void OnTriggerEnter(Collider other) {
		if (tEnergy != null&&other.tag == "Ball") {
			if (isRedGate) {//Score Red gate
				tEnergy.ModifyBlueTeamEnergy (-energyToBeAdded);
			} else {//Score Blue Gate
				tEnergy.ModifyRedTeamEnergy (-energyToBeAdded);
			}
			//set ball back to original postion
			other.gameObject.transform.position = ballResapwnPoint.position;
			//indicator happens
			something.SetActive (true);
			//reset the something after 2s
			Invoke ("SomethingDisableAgain",2.0f);
		}
	}

	//something happen when score 
	void SomethingDisableAgain(){
		something.SetActive (false);
	}
}
