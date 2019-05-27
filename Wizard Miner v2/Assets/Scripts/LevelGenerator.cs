using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The level generator generates the level and stores level
 * data such as quota. Later it will also have the ability
 * to save/load level.
 * 
 * The level generator stores templates for the tiles
 * 
*/

[System.Serializable]
public class Spell
{
    public string name;
    public Sprite sprite;
    public int amount = 0;
}

public class LevelGenerator : MonoBehaviour {

    //Templates to store object properties
    public List<TileObject> tileTemplates;
    public List<Spell> spells;

    public static Level level;
    public int magicId;
    public string magicName;
    public int magicAmount;
    public int magicCoolDown;

    private LevelLoader levelLoader;
    private SpriteCreator spriteCreator;
    private CameraScript cameraScript;

	// Use this for initialization
    public void Init()
    {
        //Set static tileobject reference
        TileObject.lgScript = GetComponent<LevelGenerator>();
        levelLoader = GetComponent<LevelLoader>();
        spriteCreator = GetComponent<SpriteCreator>();
        cameraScript = GetComponent<CameraScript>();
    }

    //Create level from tiles
    public void CreateLevel()
    {
        //New sprites
        spriteCreator.Init();
        tileTemplates[1].sprites[0] = spriteCreator.GenerateSprite("dirt");
        tileTemplates[1].sprites[1] = spriteCreator.GenerateSprite("dirt");
        tileTemplates[1].sprites[2] = spriteCreator.GenerateSprite("dirt");
        tileTemplates[1].sprites[3] = spriteCreator.GenerateSprite("dirt");
        tileTemplates[4].sprites[0] = spriteCreator.GenerateSprite("wall");
        tileTemplates[6].sprites[0] = spriteCreator.gemSprite1;
        tileTemplates[6].sprites[1] = spriteCreator.gemSprite2;

        //Get level data from dictionary
        int width = int.Parse(levelLoader.levelData["width"]);
        int height = int.Parse(levelLoader.levelData["height"]);
        string[] data = levelLoader.levelData["tiles"].Split(',');
                       
        int i = 0;
        level = new Level();
        level.size = new Vector2Int(width, height);
        level.tiles = new TileObject[level.size.x, level.size.y];

        int bordered = int.Parse(levelLoader.levelData["bordered"]);
        if (bordered > 0)
        {
            cameraScript.wrap = false;
            level.wrap = false;
        }
        else
        {
            cameraScript.wrap = true;
            level.wrap = true;
        }

        //Cave properties
        //TODO: Add a failsafe and default values
        level.time = int.Parse(levelLoader.levelData["time"]);
        level.quota = int.Parse(levelLoader.levelData["quota"]);

        for (int y = level.size.y-1; y > -1; y--)
            for (int x = 0; x < level.size.x; x++)
            {
                //Debug.Log(data[i]);
                string[] split = data[i++].Split('.');
                               
                int type = int.Parse(split[0]);

                string exData = "default";
                if (split.Length > 1)
                {
                    exData = split[1];
                }
                level.tiles[x, y] = Create(type, new Vector2Int(x, y), exData);
                level.tiles[x, y].SpriteUpdate();
            }
        TileObject.level = level;
        InitSpells("none");
    }

    //Create new object and initialise type

    TileObject Create(int type, Vector2Int position, string exData="")
    {
        TileObject obj = new TileObject();
        obj.Init(type);
        obj.position = position;
        obj.exData = exData;
        return obj;
    }


    //Start with 0 spells for all except primary spell
    public void InitSpells(string primarySpell)
    {
        foreach (Spell s in spells)
        {
            if (s.name == primarySpell)
                s.amount = -1;
            else
                s.amount = 0;
        }
        UpdateMagic();
    }

    //Update relevant magic data
    public void UpdateMagic()
    {
        magicName = spells[magicId].name;
        magicAmount = spells[magicId].amount;

    }

    public void ChangeMagic(int n)
    {
        int tries = 500;
        do
        {
            magicId = Functions.nfmod(magicId + n, spells.Count);
        }
        while (spells[magicId].amount == 0 && tries-->0);
        UpdateMagic();
    }

    //Used magic
    public void UsedMagic(int cooldown)
    {
        if (magicId != 0)
        {
            spells[magicId].amount--;
            if (spells[magicId].amount == 0)
                ChangeMagic(1);
            UpdateMagic();
        }
        magicCoolDown = cooldown;
    }

    //Get magic
    public void GetMagic(int id)
    {
        spells[id].amount++;
    }
}

//Level class to contain level data
public class Level
{
    public Vector2Int size;
    public TileObject[,] tiles;
    public Vector2Int gravityDir = Vector2Int.down;
    public int quota;
    public int time;
    public bool wrap;
}

