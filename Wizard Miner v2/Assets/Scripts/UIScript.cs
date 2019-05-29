using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//User interface for the game
public class UIScript : MonoBehaviour {

    public Image magicImage;
    public Text magicText;
    public Text timeText;
    public Text collectedText;
    public Text quotaText;
    public Text scoreText;

    public Sprite[] magicIcons;
    private GameManager gmManager;
    public LevelGenerator lgScript;

    // Use this for initialization
    void Start () 
    {
        gmManager = GameObject.Find("GameController").GetComponent<GameManager>();	
        lgScript = GameObject.Find("GameController").GetComponent<LevelGenerator>();
    }
	
	// Update is called once per frame
	void Update () 
    {
        //Timer
        magicImage.sprite = lgScript.spells[lgScript.magicId].sprite;
        magicText.text = lgScript.magicAmount.ToString();

        //Get timer to flash when less than 10 seconds left
        timeText.text = gmManager.time.ToString();
        if (gmManager.time < 10)
        {
            timeText.gameObject.GetComponent<TextFlash>().isFlashing = true;
            if (gmManager.time == 0)
            {
                timeText.color = timeText.gameObject.GetComponent<TextFlash>().onColour;
            }
        }
        else
            timeText.gameObject.GetComponent<TextFlash>().isFlashing = false;

        //Change colour when collected quota
        collectedText.text = Functions.StringDigit(gmManager.collected, 3);
        quotaText.text = "/"+Functions.StringDigit(gmManager.quota, 3);
        if (gmManager.collected >= gmManager.quota)
        {
            collectedText.color = Color.green;
            quotaText.color = Color.green;
        }
        else
        {
            collectedText.color = Color.white;
            quotaText.color = Color.white;
        }
    }
}
