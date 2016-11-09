using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    private float startTime;
    [HideInInspector]
    public int restSeconds;
    private int roundedRestSeconds;
    private int displaySeconds;
    private int displayMinutes;
    private string text;
    private TeamEnergy teamEnergy;

    [HideInInspector]
    public float guiTime;

    public int countDownSeconds = 300;   // in seconds

    void Start()
    {
        GameObject controlPanel = GameObject.Find("Control_Objects");
        teamEnergy = controlPanel.GetComponent<TeamEnergy>();
    }

    void Awake()
    {
        startTime = Time.time;
    }

    void Update()
    {


        guiTime = Time.time - startTime;

        restSeconds = (int)(countDownSeconds - (guiTime));

        //display the timer
        roundedRestSeconds = Mathf.CeilToInt(restSeconds);
        displaySeconds = roundedRestSeconds % 60;
        displayMinutes = roundedRestSeconds / 60;

        text = string.Format("{0:00}:{1:00}", displayMinutes, displaySeconds);

        if (displaySeconds < 0)
        {
            teamEnergy.TimesUp();
        }

    }

    void OnGUI()
    {
        GUI.skin.box.fontStyle = FontStyle.Bold;
        GUI.skin.box.fontSize = 20;
        GUI.skin.box.alignment = TextAnchor.MiddleCenter;
        GUI.skin.button.fontStyle = FontStyle.Bold;
        GUI.skin.button.fontSize = 20;
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;

        // GUI.Box(new Rect(Screen.width / 2 - ((Screen.width / 2) / 2), 0, Screen.width / 2, 50), text);
        GUI.Box(new Rect(0, 0, 60, 25), text);
    }
}