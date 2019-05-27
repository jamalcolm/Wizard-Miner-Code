using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * GameManager script handles main game functions such as
 * level generation by calling functions from other scripts.
 *
 */

public class GameManager : MonoBehaviour {

    public int score;
    public int time;
    public int collected;
    public int quota;

    private LevelGenerator lgScript;
    private LevelLoader levelLoader;
    private AudioManager amManager;
    private Controls controls;
    private float timer;
    private bool reset;

	// Use this for initialization
	void Start () 
    {
        //Find all game components
        levelLoader = GetComponent<LevelLoader>();
        TileObject.gmManager = GetComponent<GameManager>();
        lgScript = GetComponent<LevelGenerator>();
        lgScript.Init();
        amManager = GetComponent<AudioManager>();
        controls = GetComponent<Controls>();

        //Apply options
        GetComponent<CameraScript>().smooth = !GameOptionsState.fixedCamera;
        GetComponent<TileManager>().updateRate = Mathf.Lerp(0.28f, 0.08f, GameOptionsState.gameSpeed / 10f);

        //Generate the level
        GenerateLevel();
	}
	
	// Update is called once per frame
	void Update () {

        //Decrement game time 
        //(TODO: Relate this to game speed so it is fairer)
        //(TODO: Add a time-trial mode where time increases instead of decreases)
        //(TODO: Do only if player is alive, have a 'playerAlive' variable)
        if (time > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                time--;
                timer = 0f;
                if (time < 10)
                {
                    amManager.PlaySound(AudioNames.SOMETHING);
                }
            }
        }

        if (Input.GetKeyDown("return"))
            reset = true;
        if (Input.GetKeyDown("1"))
            NextLevel();
        if (Input.GetKeyDown("r"))
            amManager.PlaySound(AudioNames.BOULDER_LAND);

        if (reset)
        {
            reset = false;
            GenerateLevel();
        }
	}

    #region game_functions
    public void GenerateLevel()
    {        
        levelLoader.LoadLevel();
        lgScript.CreateLevel();
        collected = 0;
        time = LevelGenerator.level.time;
        quota = LevelGenerator.level.quota;
        controls.Reset();
    }

    //Go to next level
    public void NextLevel()
    {
        levelLoader.levelNo++;
        reset = true;
    }

    #endregion
}
