using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
     * The game will support 4-directional input and four button controls:
     * action: Dig/collect the object next to you.
     * magic: Use the current magic item.
     * change1: Increment magic id
     * change2: Decrement magic id
     * 
     * TODO: Make controls more responsive
     * TODO: Implement joystick controls
     * 
    */

public class Controls : MonoBehaviour {   

    public Vector2Int direction;
    public string but_action, but_magic, but_change1, but_change2;
    public float progressResponse; //How far into the update until start accepting input

    [HideInInspector]
    public bool useMagic = false;

    [HideInInspector]
    public bool useAction = false;

    [HideInInspector]
    public string magicItem = "none";

    private TileManager tmManager;
    private LevelGenerator lgScript;

    //Start
    void Start()
    {
        TileObject.controls = GetComponent<Controls>();
        tmManager = GetComponent<TileManager>();
        lgScript = GetComponent<LevelGenerator>();
    }

    //Reset (ie when the level starts)
    public void Reset()
    {
        direction = Vector2Int.zero;
        useMagic = false;
        useAction = false;
    }
                
	// Update is called once per frame
	void Update () 
    {
        //Set direction
        if (tmManager.progress >= progressResponse)
        {
            //Calculate new input direction
            int horizontal = (int)Input.GetAxisRaw("Horizontal");
            int vertical = (int)Input.GetAxisRaw("Vertical");
            if (vertical != 0)
                horizontal = 0;

            Vector2Int newDirection = new Vector2Int(horizontal, vertical);
            if (newDirection != Vector2Int.zero)
                direction = newDirection;

            //Button inputs
            if (Input.GetKey(but_magic))
                useMagic = true;
            if (Input.GetKey(but_action))
                useAction = true;

        }
        //Switch magic ID
        if (Input.GetKeyDown(but_change1))
            lgScript.ChangeMagic(1);
        if (Input.GetKeyDown(but_change2))
            lgScript.ChangeMagic(-1);
	}
}
