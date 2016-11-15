using UnityEngine;
using System.Collections;

[RequireComponent( typeof( CharacterController ) )]
public class RPGMovement : MonoBehaviour
{
    public float ForwardSpeed;
    public float BackwardSpeed;
	public float RunSpeedMutlifier;
    public float StrafeSpeed;
    public float RotateSpeed;
	public float EnergyConsume;
	public float EnergyRefillMutlifier;
	public float PushAddSpeed;
	public float stealRationForTeam;
	//For Attack
	public float attackRadius ;
	public Transform attackTrans;
	//public float attackPower ;
	//public float attackUpModifer;
	public float energySuckValue;
	public float energyStealTimer;
	//Visual Feed back
	public ParticleSystem attackParticle;
	public ParticleSystem attackBallParticle;
	public ParticleSystem lossEnergy;
    //// end of visual
	private float energyConsumeOnPushing= 8f;
	[HideInInspector]
	public bool anotherMovementControlForButton;
	private Energy m_energy;
	private TeamEnergy t_Energy;
	private Energy hitEnergy;
	private PhotonView pv;
	private float minRefillRatio;
    CharacterController m_CharacterController;
    Vector3 m_LastPosition;
    Animator m_Animator;
    PhotonView m_PhotonView;
	PhotonView tE_PhotonView;
    PhotonTransformView m_TransformView;
	GameObject pre_Connected;
    float m_AnimatorSpeed;
    Vector3 m_CurrentMovement;
	public float boundTreshHold;
    float m_CurrentTurnSpeed;
	bool enableControll;
	private GameObject ball;
	private PhotonView ball_PV;
	private float timeToSteal;


    void Start()
	{
		
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_PhotonView = GetComponent<PhotonView>();
        m_TransformView = GetComponent<PhotonTransformView>();
		m_energy = GetComponent<Energy>();
		t_Energy = GameObject.FindGameObjectWithTag ("Manager").GetComponent<TeamEnergy> ();
		tE_PhotonView = GameObject.FindGameObjectWithTag ("Manager").GetComponent<PhotonView> ();
		anotherMovementControlForButton = false;
		enableControll = true;
		if (PhotonNetwork.isMasterClient) {
			EnergyConsume = EnergyConsume/3.0f;
		}
		ball = GameObject.FindGameObjectWithTag ("Ball");
		ball_PV = GameObject.FindGameObjectWithTag("Ball").GetComponent<PhotonView> ();
		minRefillRatio = 1.3f; // means 0.3f
	}

    void Update()
    {
		//Create Chain to Energy
	
        if( m_PhotonView.isMine == true )
        {
            ResetSpeedValues();
			if (enableControll&&anotherMovementControlForButton) {
				UpdateRotateMovement ();
				UpdateForwardMovement ();
				UpdateBackwardMovement ();
                UpdateStrafeMovement ();

			}

			Attack();
            MoveCharacterController();
            ApplyGravityToCharacterController();

            ApplySynchronizedValues();
        }

        UpdateAnimation();

    }



    void UpdateAnimation()
    {
        Vector3 movementVector = transform.position - m_LastPosition;

        float speed = Vector3.Dot( movementVector.normalized, transform.forward );
        float direction = Vector3.Dot( movementVector.normalized, transform.right );
        //float speed2 = Vector3.Dot(movementVector.normalized, transform.right);

//        if ( Mathf.Abs( speed ) < 0f && Mathf.Abs(speed2) < 0f)
//        {
//            speed = 0f;
//			//Stand Still Refill Energy
//			m_energy.TakenEnergy(-EnergyConsume*EnergyRefillMutlifier);
//		    //m_PhotonView.RPC("TakenEnergy",PhotonTargets.All,-EnergyConsume);
//        }
//
		if( Mathf.Abs( speed ) <0.1f)
		{
			speed = 0f;
			//Stand Still Refill Energy
		
			float temp = ((m_energy.energyValue*minRefillRatio-m_energy.currentEnergy)/ m_energy.energyValue)*EnergyRefillMutlifier;//Energy Refill;
			m_energy.TakenEnergy(-EnergyConsume*temp);
//			Debug.Log ("RefillEnergy");
			//m_PhotonView.RPC("TakenEnergy",PhotonTargets.All,-EnergyConsume);
		}
        if( speed > 0.6f )
        {
            speed = 1f;
            direction = 0f;
        }

        if( speed >= 0f )
        {
            if( Mathf.Abs( direction ) > 0.7f )
            {
                speed = 1f;
            }
        }

        m_AnimatorSpeed = Mathf.MoveTowards( m_AnimatorSpeed, speed, Time.deltaTime * 5f );

        m_Animator.SetFloat( "Speed", m_AnimatorSpeed );
        m_Animator.SetFloat( "Direction", direction );

        m_LastPosition = transform.position;
    }

    void ResetSpeedValues()
    {
        m_CurrentMovement = Vector3.zero;
        m_CurrentTurnSpeed = 0;
    }

    void ApplySynchronizedValues()
    {
        m_TransformView.SetSynchronizedValues( m_CurrentMovement, m_CurrentTurnSpeed );
    }

    void ApplyGravityToCharacterController()
    {
        m_CharacterController.Move( transform.up * Time.deltaTime * -9.81f );
    }

    void MoveCharacterController()
    {
        m_CharacterController.Move( m_CurrentMovement * Time.deltaTime );
    }

    void UpdateForwardMovement()
    {
        if( Input.GetKey( KeyCode.W ) || Input.GetAxisRaw("Vertical") > 0.1f )
        {
			//For Running
			if (Input.GetKey (KeyCode.LeftShift)||Input.GetKey (KeyCode.RightShift)) {
				// tempoary calculate the amount
				float tempCal = EnergyConsume * RunSpeedMutlifier;
				//call method in net work
				m_energy.TakenEnergy(tempCal);

				//m_PhotonView.RPC ("TakenEnergy",PhotonTargets.All,tempCal);

				m_CurrentMovement = transform.forward * ForwardSpeed * RunSpeedMutlifier;
			} else {
				
				m_energy.TakenEnergy(EnergyConsume);
				//m_PhotonView.RPC ("TakenEnergy",PhotonTargets.All,EnergyConsume);
				m_CurrentMovement = transform.forward * ForwardSpeed;
			}
        }
    }

    void UpdateBackwardMovement()
    {
        if( Input.GetKey( KeyCode.S ) || Input.GetAxisRaw("Vertical") < -0.1f )
        {
			//For Running
			if (Input.GetKey (KeyCode.LeftShift)||Input.GetKey (KeyCode.RightShift)) {
				// tempoary calculate the amount
				float tempCal = EnergyConsume * RunSpeedMutlifier;
	
				m_energy.TakenEnergy(tempCal);
				//call method in net work
				//m_PhotonView.RPC ("TakenEnergy",PhotonTargets.All,tempCal);

				m_CurrentMovement =  -transform.forward * BackwardSpeed * RunSpeedMutlifier;
			} else {


				m_energy.TakenEnergy(EnergyConsume);
				//m_PhotonView.RPC ("TakenEnergy",PhotonTargets.All,EnergyConsume);
				m_CurrentMovement =  -transform.forward * BackwardSpeed;
               // print("Energy2:" + EnergyConsume);
            }
        }
    }

    void UpdateStrafeMovement()
    {
		float temp = ((m_energy.energyValue*minRefillRatio-m_energy.currentEnergy)/ m_energy.energyValue)*EnergyRefillMutlifier;//Energy Refill;

		if( Input.GetKey( KeyCode.A ) == true|| Input.GetAxisRaw("Horizontal") < -0.1f )
		{
		    CancelInvoke();
			m_energy.TakenEnergy(EnergyConsume+EnergyConsume*temp);
           // print("Energy1:" + m_energy);
            //m_PhotonView.RPC ("TakenEnergy",PhotonTargets.All,EnergyConsume);
            m_CurrentMovement = -transform.right * StrafeSpeed;
        }

		if( Input.GetKey( KeyCode.D ) == true|| Input.GetAxisRaw("Horizontal") > 0.1f  )
		{
			CancelInvoke();
			m_energy.TakenEnergy(EnergyConsume+EnergyConsume*temp);
			//m_PhotonView.RPC ("TakenEnergy",PhotonTargets.All,EnergyConsume);
            m_CurrentMovement = transform.right * StrafeSpeed;
        }
    }

    void UpdateRotateMovement()
    {
        if( Input.GetKey( KeyCode.A ) || Input.GetAxisRaw("Horizontal") < -0.1f )
        {
            m_CurrentTurnSpeed = -RotateSpeed;
            transform.Rotate(0.0f, -RotateSpeed * Time.deltaTime, 0.0f);
        }

        if( Input.GetKey( KeyCode.D ) || Input.GetAxisRaw("Horizontal") > 0.1f )
        {
            m_CurrentTurnSpeed = RotateSpeed;
            transform.Rotate(0.0f, RotateSpeed * Time.deltaTime, 0.0f);
        }
    }
	void OnCollisionStay(Collision coll) {
		if (Input.GetButton ("Fire1")) {
			
		}
	}
	//  Suck Gun Attack
	//Suck Energy is 3 times faster of Energy Consume 
	void Attack(){
		if (Input.GetButton ("Fire1")) {
			m_PhotonView.RPC ("AttackVisualFeedBack", PhotonTargets.All,1f);
			//Vector3 explosionPos = transform.position+Vector3.forward*forwardOffset;
			Collider[] colliders = Physics.OverlapSphere (attackTrans.position, attackRadius);
			foreach (Collider hit in colliders) {
				if (hit.tag == "BackPack") {// hit the backPack 
					hitEnergy = hit.gameObject.transform.parent.parent.GetComponent<Energy> ();
					pv = hit.gameObject.transform.parent.parent.GetComponent<PhotonView> ();

						if (Time.time >= timeToSteal) {
							//can attack now
							timeToSteal = Time.time + energyStealTimer;
						//Debug.Log ("attack");
						bool isSameTeam = false;
						if (hitEnergy.isRedTeam) {
							if (m_energy.isRedTeam)
								isSameTeam = true;
							else
								isSameTeam = false;
						} else {
							if (m_energy.isRedTeam)
								isSameTeam = false;
							else
								isSameTeam = true;
						}

						//if target is sameteam
						if (isSameTeam) {

							// both of the the teammemeber have enough energy to trade more than the steal amount 
							if (energySuckValue <= hitEnergy.currentEnergy && hitEnergy.currentEnergy <= hitEnergy.energyValue - energySuckValue && energySuckValue <= m_energy.currentEnergy && m_energy.currentEnergy <= m_energy.energyValue - energySuckValue) {
								m_energy.TakenEnergy (-energySuckValue);
								pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
								m_PhotonView.RPC ("SuckGreenEnergyVisualFeedBack", PhotonTargets.All,1f);
							}
						} else {
							m_PhotonView.RPC ("SuckGreenEnergyVisualFeedBack", PhotonTargets.All,1f); // suck Energy Visual FeedBack
							// if target is in different team and this player is in Red team
							if (m_energy.isRedTeam) { // This is RedTeam

								// if the target energy is lower than energy gona get sucked, and my Energy is not full
								if (hitEnergy.currentEnergy <= energySuckValue) {// Enemy Energy has less energy than suck value
									if (m_energy.currentEnergy < m_energy.energyValue - energySuckValue) {// my Energy is not going to full
										float tempEnergy = energySuckValue - hitEnergy.currentEnergy; // how much energy the enemy lack from suck value
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
										if (t_Energy.currentBlueTeamEnergy >= 0f) {//Blue Team Energy pool Have Energy
											//t_Energy.ModifyBlueTeamEnergy (EnergyConsume * energySuckValue);//blue team minus
											tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, tempEnergy * stealRationForTeam);
										} 
									} else {// my Energy is going to Full
										float tempEnergy = energySuckValue - hitEnergy.currentEnergy;// energy Blue Team should Lose
										float tempEnergy2 = energySuckValue + m_energy.currentEnergy - m_energy.energyValue;//energy RedTeamShould Added
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
										//t_Energy.ModifyRedTeamEnergy (-EnergyConsume * energySuckValue);//red team add
										tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, -tempEnergy2 * stealRationForTeam);
										if (t_Energy.currentBlueTeamEnergy >= 0) {//Blue Team Energy pool Have Energy
											//t_Energy.ModifyBlueTeamEnergy (EnergyConsume * energySuckValue);//blue team minus
											tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, tempEnergy * stealRationForTeam); 
										}
									}
								} else {// Enemy have more energy than suck value
									if (m_energy.currentEnergy < m_energy.energyValue - energySuckValue) {// my Energy is not going to full
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);

									} else {// my Energy is going to Full
										float tempEnergy = energySuckValue + m_energy.currentEnergy - m_energy.energyValue;//energy RedTeamShould Added
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
										//t_Energy.ModifyRedTeamEnergy (-EnergyConsume * energySuckValue);//red team add
										tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, -tempEnergy * stealRationForTeam);
									}
								}
							} else {//Blue Team

								// if the target energy is lower than energy gona get sucked, and my Energy is not full
								if (hitEnergy.currentEnergy <= energySuckValue) {// Enemy Energy has less energy than suck value
									if (m_energy.currentEnergy < m_energy.energyValue - energySuckValue) {// my Energy is not going to full
										float tempEnergy = energySuckValue - hitEnergy.currentEnergy; // how much energy the enemy lack from suck value
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
										if (t_Energy.currentRedTeamEnergy >= 0f) {//red Team Energy pool Have Energy

											tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, tempEnergy * stealRationForTeam);//red team minus
										} 
									} else {// my Energy is going to Full
										float tempEnergy = energySuckValue - hitEnergy.currentEnergy;// energy Blue Team should Lose
										float tempEnergy2 = energySuckValue + m_energy.currentEnergy - m_energy.energyValue;//energy RedTeamShould Added
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
										tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, -tempEnergy2 * stealRationForTeam);//blue team add
										if (t_Energy.currentRedTeamEnergy >= 0) {//red Team Energy pool Have Energy
											tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, tempEnergy * stealRationForTeam); //red team minus
										}
									}
								} else {// Enemy have more energy than suck value
									if (m_energy.currentEnergy < m_energy.energyValue - energySuckValue) {// my Energy is not going to full
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
									} else {// my Energy is going to Full
										float tempEnergy = energySuckValue + m_energy.currentEnergy - m_energy.energyValue;//energy RedTeamShould Added
										m_energy.TakenEnergy (-energySuckValue);
										pv.RPC ("TakenEnergy", PhotonTargets.All, energySuckValue);
										//Blue team add
										tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, -tempEnergy * stealRationForTeam);
									}
								}

							}
						}

						}

				} else if (hit.tag == "Ball") {
					Vector3 tempVel = transform.forward * PushAddSpeed;
					ball_PV.RPC ("AddVelocity", PhotonTargets.All, tempVel);
					m_energy.TakenEnergy (EnergyConsume*energyConsumeOnPushing);
					attackBallParticle.Play ();
					m_PhotonView.RPC ("AttackBallVisualFeedBack", PhotonTargets.All,1f);
				}
			}

		}
	}

	public void EnableControl(bool b){
		enableControll = b;
	}
	[PunRPC]
	public void AttackVisualFeedBack(float t){
		attackParticle.Play ();
	}
	[PunRPC]
	public void AttackBallVisualFeedBack(float t){
		attackBallParticle.Play ();
	}
	[PunRPC]
	public void SuckGreenEnergyVisualFeedBack(float t){
		lossEnergy.Play ();
	}
		
}
