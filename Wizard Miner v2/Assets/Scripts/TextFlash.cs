using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Text flashing script
public class TextFlash : MonoBehaviour {

    public bool isFlashing;
    public Color offColour;
    public Color onColour;
    public float flashRate;
    private float timer = 0f;
    private Text text;
	
	// Update is called once per frame
    void Start()
    {
        text = GetComponent<Text>();
    }

	void Update () 
    {
        if (isFlashing)
        {
            timer += Time.deltaTime;
            if (timer > flashRate)
                text.color = onColour;
            else
                text.color = offColour;
            if (timer > flashRate * 2f)
                timer = 0;
        }
        else
            text.color = offColour;
	}
}
