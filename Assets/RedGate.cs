using UnityEngine;
using System.Collections;

public class RedGate : MonoBehaviour {
	public GameObject something;
	private TeamEnergy tEnergy;
	public bool isRedGate;
	public float energyToBeAdded;
	public Transform ballResapwnPoint;
	private PhotonView tE_PhotonView;
    public ParticleSystem redGateGoal;
    public ParticleSystem blueGateGoal;
    //PhotonView m_PhotonView;

    // Use this for initialization
    void Start () {
		tEnergy = GameObject.FindGameObjectWithTag ("Manager").GetComponent<TeamEnergy> ();
		tE_PhotonView = tEnergy.GetComponent<PhotonView> ();
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
                //tE_PhotonView.RPC("RedGateGoal", PhotonTargets.All, 1f);

                //tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, -energyToBeAdded);
            } else {//Score Blue Gate
				tEnergy.ModifyRedTeamEnergy (-energyToBeAdded);
                //tE_PhotonView.RPC("BlueGateGoal", PhotonTargets.All, 1f);

                //tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, -energyToBeAdded);
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

    [PunRPC]
    public void RedGateGoal(float t)
    {
        redGateGoal.Play();
    }

    [PunRPC]
    public void BlueGateGoal(float t)
    {
        blueGateGoal.Play();
    }

}
