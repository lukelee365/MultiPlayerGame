using UnityEngine;
using System.Collections;
using Photon;

public class BallMove : Photon.MonoBehaviour {


	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;
	private Rigidbody rigid;
	public float maxSpeed;
	void Start(){
		rigid = GetComponent<Rigidbody> ();
		correctPlayerPos = transform.position;
		correctPlayerRot = transform.rotation;
	}
	// Update is called once per frame
	void Update()
	{
		//Debug.Log (rigid.velocity.magnitude);
		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
		}
	}
	//used for Update Phsysic
	void FixedUpdate(){
		
		if(rigid.velocity.magnitude > maxSpeed)
		{
			rigid.velocity = rigid.velocity.normalized * maxSpeed;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our data
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);

		}
		else
		{
			// Network player, receive data
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
}
