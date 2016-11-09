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

	//For Attack
	public float attackRadius ;
	public Transform attackTrans;
	//public float attackPower ;
	//public float attackUpModifer;
	public float energySuckSpeed;
	private Energy m_energy;
	private TeamEnergy t_Energy;
	private Energy hitEnergy;
	private PhotonView pv;
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

    void Start()
	{
		
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_PhotonView = GetComponent<PhotonView>();
        m_TransformView = GetComponent<PhotonTransformView>();
		m_energy = GetComponent<Energy>();
		t_Energy = GameObject.FindGameObjectWithTag ("Manager").GetComponent<TeamEnergy> ();
		tE_PhotonView = GameObject.FindGameObjectWithTag ("Manager").GetComponent<PhotonView> ();
		enableControll = true;
		if (PhotonNetwork.isMasterClient) {
			EnergyConsume = EnergyConsume/2.0f;
		}
	}

    void Update()
    {
		//Create Chain to Energy
	
        if( m_PhotonView.isMine == true )
        {
            ResetSpeedValues();
			if (enableControll) {
				UpdateRotateMovement ();
				UpdateForwardMovement ();
				UpdateBackwardMovement ();
				UpdateStrafeMovement ();
				Attack ();
			}
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

        if( Mathf.Abs( speed ) < 0.2f )
        {
            speed = 0f;
			//Stand Still
			m_energy.TakenEnergy(-EnergyConsume);
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
			}
        }
    }

    void UpdateStrafeMovement()
    {
		if( Input.GetKey( KeyCode.A ) == true|| Input.GetAxisRaw("Horizontal") < -0.1f )
		{
			CancelInvoke();

			m_energy.TakenEnergy(EnergyConsume);
			//m_PhotonView.RPC ("TakenEnergy",PhotonTargets.All,EnergyConsume);
            m_CurrentMovement = -transform.right * StrafeSpeed;
        }

		if( Input.GetKey( KeyCode.D ) == true|| Input.GetAxisRaw("Horizontal") > 0.1f  )
		{
			CancelInvoke();

			m_energy.TakenEnergy(EnergyConsume);
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
			//Vector3 explosionPos = transform.position+Vector3.forward*forwardOffset;
			Collider[] colliders = Physics.OverlapSphere (attackTrans.position, attackRadius);
			foreach (Collider hit in colliders) {
				if (hit.tag == "BackPack") {// hit the backPack 
					hitEnergy = hit.gameObject.transform.parent.GetComponent<Energy> ();
					pv = hit.gameObject.transform.parent.GetComponent<PhotonView> ();
					pre_Connected = hit.gameObject;

				}
			}
			float tempDist =1000f;
			if (pre_Connected != null) {
				tempDist = Vector3.Distance (pre_Connected.transform.position, transform.position);
			}
			if (tempDist <= boundTreshHold) {//insidebound will continuous suck energy
				if (hitEnergy != null) {
					bool isSameTeam = hitEnergy.isRedTeam && m_energy.isRedTeam;
					//if target is sameteam
					if (isSameTeam) {
						// both of the the teammemeber have enough energy to trade
						if (2 <= hitEnergy.currentEnergy && hitEnergy.currentEnergy <= hitEnergy.energyValue && 2 <= m_energy.currentEnergy && m_energy.currentEnergy <= m_energy.energyValue) {
							m_energy.TakenEnergy (-EnergyConsume * energySuckSpeed);
							pv.RPC ("TakenEnergy", PhotonTargets.All, EnergyConsume * energySuckSpeed);
						}
					} else {
						// if target is in different team and this player is in Red team
						if (m_energy.isRedTeam) { 
							// if the target energy is lower than 1, and my Energy is not full
							if (hitEnergy.currentEnergy <= 1) {
								if (m_energy.currentEnergy >= (hitEnergy.energyValue - 1f)) {// my Energy is Full
									if (t_Energy.currentBlueTeamEnergy >= 0) {//Blue Team Energy pool Have Energy
										//t_Energy.ModifyRedTeamEnergy (-EnergyConsume * energySuckSpeed);//red team add
										tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, -EnergyConsume * energySuckSpeed);
										//t_Energy.ModifyBlueTeamEnergy (EnergyConsume * energySuckSpeed);//blue team minus
										tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, EnergyConsume * energySuckSpeed);
									}
								}
							} else if (hitEnergy.currentEnergy > 1 && hitEnergy.currentEnergy <= hitEnergy.energyValue) {
								// target Have Energy
								if (m_energy.currentEnergy >= (hitEnergy.energyValue - 1f)) {// my Energy is Full
									//t_Energy.ModifyRedTeamEnergy (-EnergyConsume * energySuckSpeed);//red team add
									tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, -EnergyConsume * energySuckSpeed);
									pv.RPC ("TakenEnergy", PhotonTargets.All, EnergyConsume * energySuckSpeed * 2f);//target miss energy
								} else {
									m_energy.TakenEnergy (-EnergyConsume * energySuckSpeed);
									pv.RPC ("TakenEnergy", PhotonTargets.All, EnergyConsume * energySuckSpeed * 2f);//target miss energy
								}
							}
						} else {//Blue Team
							// if the target energy is lower than 1, and my Energy is not full
							if (hitEnergy.currentEnergy <= 1) {
								if (m_energy.currentEnergy >= (hitEnergy.energyValue - 1f)) {// my Energy is Full
									if (t_Energy.currentRedTeamEnergy >= 0) {//Red Team Energy pool Have Energy
										//t_Energy.ModifyBlueTeamEnergy (-EnergyConsume * energySuckSpeed);//Blue team add
										tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, -EnergyConsume * energySuckSpeed);
										//t_Energy.ModifyRedTeamEnergy (EnergyConsume * energySuckSpeed);//Red team minus
										tE_PhotonView.RPC ("ModifyRedTeamEnergy", PhotonTargets.All, EnergyConsume * energySuckSpeed);
									}
								}
							} else if (hitEnergy.currentEnergy > 1 && hitEnergy.currentEnergy <= hitEnergy.energyValue) {
								// target Have Energy
								if (m_energy.currentEnergy >= (hitEnergy.energyValue - 1f)) {// my Energy is Full
									//t_Energy.ModifyBlueTeamEnergy (-EnergyConsume * energySuckSpeed);//Blue team add
									tE_PhotonView.RPC ("ModifyBlueTeamEnergy", PhotonTargets.All, -EnergyConsume * energySuckSpeed);
									pv.RPC ("TakenEnergy", PhotonTargets.All, EnergyConsume * energySuckSpeed * 2f);//target miss energy
								} else {
									m_energy.TakenEnergy (-EnergyConsume * energySuckSpeed);
									pv.RPC ("TakenEnergy", PhotonTargets.All, EnergyConsume * energySuckSpeed * 2f);//target miss energy
								}
							}

						}
					}

				} else {//break the bound
					Debug.Log("BreakBound");
					pre_Connected =null;
					hitEnergy = null;
					pv = null;

				}
			}
		}
	}

	public void EnableControl(bool b){
		enableControll = b;
	}

		
}
