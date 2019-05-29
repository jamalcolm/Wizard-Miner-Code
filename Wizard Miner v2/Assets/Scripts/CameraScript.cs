using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The CameraScript manipulates how the tiles are represented on screen.
 * 
 * This is not as simple as taking each tile and rendering it.
 * The script instantiates tiles and figures out which tile objects correspond
 * to which space, then assigns that tile a sprite.
 * 
*/

public class CameraScript : MonoBehaviour {

    //Public variables
    public GameObject tilePrefab;
    public bool smooth;
    public bool wrap;
    public Vector2Int viewportSize;
    public Vector2 camPosition;
    public Sprite noneSprite;

    //Private variables
    private Transform tileParent;
    private TileManager tmManager;
    private GameObject[,] tiles;
    private float screenWidth, screenHeight;
    private Vector2 tileSize;
    private float progress;

    // Use this for initialization
    void Start () 
    {
        //Initialise camera
        tileParent = GameObject.Find ("Tiles").transform;
        tmManager = GetComponent<TileManager> ();

        screenHeight = Camera.main.orthographicSize * 2f;
        screenWidth = screenHeight / Screen.height * Screen.width;
        tileSize = new Vector2 (screenWidth / viewportSize.x, screenHeight / viewportSize.y);

        //Instantiate tiles
        tiles = new GameObject[viewportSize.x+2, viewportSize.y+2];
        for (int y = 0; y < viewportSize.y+2; y++)
            for (int x = 0; x < viewportSize.x+2; x++) 
            {
                tiles[x, y] = Instantiate(tilePrefab, tileParent);
                tiles[x, y].transform.position = new Vector3(ViewX(x), ViewY(y));
                tiles[x, y].transform.localScale = tileSize;
            }	
    }

    //Get tile position according to screen
    float ViewX(float x)
    {
        return (-screenWidth / 2f + (x-1f) * tileSize.x);
    }

    float ViewY(float y)
    {
        return (-screenWidth / 2f + (y+1f) * tileSize.y);
    }
	
	// Update is called once per frame
    void Update () 
    {
        //Progress = amount of time between current and previous frame
        if (smooth)
            progress = tmManager.progress;
        else
            progress = 1f;

        //Follow player
        camPosition = new Vector2(
            viewportSize.x*-0.5f+tmManager.playerPosition.x,
            viewportSize.y*-0.5f+tmManager.playerPosition.y
        );

        //Update tiles
        int s, t;
        int camx = (int)camPosition.x - 1;
        int camy = (int)camPosition.y - 1;
        Level level = LevelGenerator.level;

        //Loop through all tiles
        for (int y = 0; y < viewportSize.y + 2; y++)
            for (int x = 0; x < viewportSize.x + 2; x++) 
            {
                TileObject tObject = new TileObject();

                //Show level wrapping
                if (wrap) 
                {
                    s = Functions.nfmod(x + level.size.x + camx, level.size.x);
                    t = Functions.nfmod(y + level.size.y + camy, level.size.y);
                    tObject = level.tiles[s, t];
                    SetTile(tiles[x,y], x,y, tObject.currentSprite, tObject.direction);
                }
                //Don't show level wrapping
                else 
                {
                    s = (x + camx);
                    t = (y + camy);
                    if (s >= 0 && s < level.size.x && t >= 0 && t < level.size.y)
                    {
                        tObject = level.tiles[s, t];
                        SetTile(tiles[x, y], x,y, tObject.currentSprite, tObject.direction);
                    }
                    else
                        SetTile(tiles[x,y], x,y, noneSprite, Vector2Int.zero);
                }
            }
    }

    //Set tile TileId 
    void SetTile(GameObject tileId, int x, int y, Sprite sprite, Vector2Int direction)
    {
        tileId.GetComponent<SpriteRenderer>().sprite = sprite;
        tileId.transform.position = new Vector3(
            ViewX(x+(direction.x-tmManager.playerDir.x)*(progress-1)),
            ViewY(y+(direction.y-tmManager.playerDir.y)*(progress-1))
        );

    }
}
