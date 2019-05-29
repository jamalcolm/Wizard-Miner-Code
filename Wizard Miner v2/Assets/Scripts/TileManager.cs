using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TileManager script updates all of the tiles periodically.
*/
public class TileManager : MonoBehaviour {

    public float updateRate;

    [HideInInspector]
    public float time = 0.0f;
    public float progress;

    [HideInInspector]
    public Vector2Int playerPosition = Vector2Int.zero;

    [HideInInspector]
    public Vector2Int playerDir = Vector2Int.zero;

    private int updateCount = 0;
    private Level level;
    private Controls controls;   

    // Use this for initialization
    void Start () 
    {
        controls = GetComponent<Controls>();
        TileObject.tmManager = GetComponent<TileManager>();
	}
	
    // Update is called once per frame
    void Update () 
    {
        level = LevelGenerator.level;

        //Update every (updateRate) seconds
        time += Time.deltaTime;
        if (time >= updateRate)
        {
            time = 0.0f;
            TileUpdate();
            controls.Reset();
        }

        //Animation
        for (int y = level.size.y - 1; y > -1; y--)
            for (int x = 0; x < level.size.x; x++)
            {
                TileObject obj = level.tiles[x, y];
                obj.SpriteUpdate();
            }

        progress = (time / Mathf.Max(updateRate, float.Epsilon));
	}

    //Tile update script
    void TileUpdate()
    {
        TileObject obj;

        //First pass before main update
        for (int y = level.size.y - 1; y > -1; y--)
            for (int x = 0; x < level.size.x; x++)
            {
                obj = level.tiles[x, y];
                obj.updated = false;
                obj.direction = Vector2Int.zero;
            }
        playerDir = Vector2Int.zero;

        //Main update
        for (int y = level.size.y - 1; y > -1; y--)
            for (int x = 0; x < level.size.x; x++)
            {
                obj = level.tiles[x, y];
                if (!obj.updated)
                {
                    obj.Update();
                    if (obj.name == "player")
                    {
                        playerPosition = obj.position;
                        playerDir = obj.direction;
                    }
                }
            }
        updateCount++;
    }
}