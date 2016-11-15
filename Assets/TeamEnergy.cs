using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TeamEnergy : MonoBehaviour {
	public float redTeamEnergyValue;
	public float blueTeamEnergyValue;
	public float currentRedTeamEnergy;
	public float currentBlueTeamEnergy;
	public GameObject redScore;
	public GameObject blueScore;

    [HideInInspector]
    public GameObject redTeamWinUI;
    [HideInInspector]
    public GameObject blueTeamWinUI;
    [HideInInspector]
    public GameObject redTeamLoseUI;
    [HideInInspector]
    public GameObject blueTeamLoseUI;
    [HideInInspector]
    public GameObject noWinAndLoseUI;

    private Energy energy;
    private GameObject player;
	private float redScaleY;
	private float blueScaleY;
    // Use this for initialization
    void Start () {

        redTeamWinUI = GameObject.Find("RedWinButton");
        blueTeamWinUI = GameObject.Find("BlueWinButton");
        redTeamLoseUI = GameObject.Find("RedLoseButton");
        blueTeamLoseUI = GameObject.Find("BlueLoseButton");
        noWinAndLoseUI = GameObject.Find("DrawButton");
        redTeamWinUI.SetActive(false);
        blueTeamWinUI.SetActive(false);
        redTeamLoseUI.SetActive(false);
        blueTeamLoseUI.SetActive(false);
        noWinAndLoseUI.SetActive(false);
		redScaleY = redScore.transform.localScale.y;
		blueScaleY = blueScore.transform.localScale.y;
        //energy = player.GetComponent<Energy>();
    }
	
	// Update is called once per frame
	void Update () {
		redScore.transform.localScale = new Vector3 (redScore.transform.localScale.x, redScaleY * currentRedTeamEnergy / redTeamEnergyValue, redScore.transform.localScale.z);
		blueScore.transform.localScale = new Vector3 (blueScore.transform.localScale.x, blueScaleY * currentBlueTeamEnergy / blueTeamEnergyValue, blueScore.transform.localScale.z);
        ViewWinLoseConditions();
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

    public void ViewWinLoseConditions()
    {

            //if (energy.isRedTeam == true)
            //{
                if (currentRedTeamEnergy >= redTeamEnergyValue)
                {
                    redTeamWinUI.SetActive(true);
                }

                if (currentBlueTeamEnergy >= blueTeamEnergyValue)
                {
                    redTeamLoseUI.SetActive(true);
                }
            

    }

    public void TimesUp()
    {
        if (currentRedTeamEnergy > currentBlueTeamEnergy)
        {
            redTeamWinUI.SetActive(true);
        }

        if (currentRedTeamEnergy < currentBlueTeamEnergy)
        {
            blueTeamWinUI.SetActive(true);
        }

        if (currentRedTeamEnergy == currentBlueTeamEnergy)
        {
            noWinAndLoseUI.SetActive(true);
        }
    }
    public void PassPlayer2(GameObject o)
    {

        player = o;

    }
    public void ReLoadGame()
    {
        Application.LoadLevel("DemoRPGMovement-Scene");
        redTeamWinUI.SetActive(false);
        blueTeamWinUI.SetActive(false);
        noWinAndLoseUI.SetActive(false);
    }

    	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    	{
    		if (stream.isWriting)
    		{
    			//  send the others our data
    
    			stream.SendNext(currentRedTeamEnergy);
    			stream.SendNext(currentBlueTeamEnergy);
    		}
    		else
    		{
    			// Network , receive data
    			this.currentRedTeamEnergy = (float)stream.ReceiveNext();
    			this.currentBlueTeamEnergy = (float)stream.ReceiveNext();
    		}
    	}

}
