using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TileObject script contains all functions necessary for the objects in
 * the game to function.
 * 
 * TileObject is the base class which child classes
 * (boulder, dirt, player etc.) inherit from.
 * 
 * TileObject contains basic functions such as moving or checking
 * what is next to it.
 * */

//Animation types
public enum AnimationType
{
    ANIM_NONE,
    ANIM_PLAY,
    ANIM_RANDOM,
    ANIM_RANDOMONCE,
    ANIM_RANDOMPLAY,
    ANIM_PLAYER,
    ANIM_POTION
}
    
[System.Serializable]
public class TileObject
{
    
    public string name;
    public string type;
    public bool solid = false;
    public bool rounded = false;
    public bool explosive = false;
    public List<Sprite> sprites;
    public AnimationType animType;
    public int animSpeed;
    public int animOffset;
    public List<string> travels;

    [HideInInspector]
    public string state = "default";
    [HideInInspector]
    public Vector2Int position = Vector2Int.zero;
    [HideInInspector]
    public Vector2Int direction = Vector2Int.zero;
    [HideInInspector]
    public bool updated = false;
    [HideInInspector]
    public Sprite currentSprite;
    [HideInInspector]
    public string exData = "";
    [HideInInspector]
    public bool setToExplode = false;

    public static LevelGenerator lgScript;
    public static AudioManager amManager;
    public static GameManager gmManager;
    public static TileManager tmManager;
    public static Controls controls;
    public static Level level;

    private int spriteIndex = 0;
    private Vector2Int lastDir;
    private float animTime = 0f;
    private int count;

    //Initialise from template (see LevelGenerator)
    public void Init(int templateId)
    {
        TileObject template = lgScript.tileTemplates[templateId];
        name = template.name;
        solid = template.solid;
        rounded = template.rounded;
        sprites = template.sprites;
        travels = template.travels;
        animType = template.animType;
        animSpeed = template.animSpeed;
        animOffset = template.animOffset;
        explosive = template.explosive;
        animTime = 0f;
        count = 0;
        updated = true;
    }

    #region update

    //Update tile object
    public virtual void Update()
    {
        updated = true;
        count++;
        switch (name)
        {
            case "space":
                {direction=Vector2Int.zero;break;}
            case "boulder":
            case "diamond":
            case "potion":
            case "ore":
            case "fragile":
            case "bomb":
                {BoulderUpdate();break;}
            case "player":
                {PlayerUpdate();break;}
            case "explosion":
                {ExplosionUpdate();break;}
            case "enemy":
            case "bouldrend":
                {EnemyUpdate(0);break;}
            case "bat":
            case "butterfly":
                {EnemyUpdate(2);break;}
            case "robot":
                {RobotUpdate();break;}
            case "spider":
                {SpiderUpdate();break;}
            case "fireball":
                {FireballUpdate();break;}
            case "exit":
                {ExitUpdate();break;}
            case "ephorsu":
                {EnemyUpdate(0); break;}
            case "beetle":
                {BeetleUpdate();break;}
            default:
                break;
        }      

        if (setToExplode)
            ExplodeObject();
    }

    #endregion

    #region animation

    //Update sprite
    public virtual void SpriteUpdate()
    {       
        animTime += Time.deltaTime;
        int len = sprites.Count;
        if (len > 0)
        {
            
            switch (animType)
            {
                case AnimationType.ANIM_PLAY:
                    {                        
                        spriteIndex = AnimLoop(animSpeed, len, animOffset);
                        break;}
                case AnimationType.ANIM_RANDOM:
                    {
                        spriteIndex = Random.Range (0, len);
                        break;
                    }
                case AnimationType.ANIM_RANDOMONCE:
                    {
                        spriteIndex = Random.Range (0, len);
                        animType = AnimationType.ANIM_NONE;
                        break;
                    }
                case AnimationType.ANIM_RANDOMPLAY:
                    {
                        animOffset = Random.Range (0, len);
                        animType = AnimationType.ANIM_PLAY;
                        break;
                    }
                case AnimationType.ANIM_PLAYER:
                    {
                        
                        PlayerAnimate();
                        break;
                    }
                case AnimationType.ANIM_POTION:
                    {
                        spriteIndex = (exData == "default") ? 0 : int.Parse(exData);
                        break;
                    }
                default:
                    break;
            }
            currentSprite = sprites[spriteIndex % len];
        }

    }

    //Simple loop animation script
    int AnimLoop(float speed, int size, int offset = 0)
    {
        return (int)(animTime * speed % size + offset);
    }

    #endregion

    #region explosion
    void ExplodeObject()
    {
        switch (name)
        {
            case "player":
            case "enemy":
            case "robot":
            case "bat":
            case "spider":
            case "bomb":
            case "beetle":
                {
                    amManager.PlaySound(AudioNames.EXPLOSION);
                    Explode(1, 0, 0);break;}
            case "butterfly":
                {
                    amManager.PlaySound(AudioNames.BUTTERFLY);
                    Explode(1, 1, 6);break;}
            case "bouldrend":
                {
                    amManager.PlaySound(AudioNames.BOULDREND);
                    Explode(1, 2, 2);break;}
            case "ephorsu":
                {
                    amManager.PlaySound(AudioNames.EPHORSU);
                    if (exData == "default" || exData == "")
                        Explode(1, 3, 0);
                    else
                        Explode(1, 3, int.Parse(exData));
                    break;
                }
            case "fragile":
                {
                    Init(0);
                    amManager.PlaySound(AudioNames.FRAGILE);
                    break;
                }
            case "ore":
                {
                    //Can only be broken by boulder
                    if (At(Rotate(level.gravityDir, 2)).name == "boulder")
                    {
                        Init(6);
                        amManager.PlaySound(AudioNames.ORE);
                    }
                    break;
                }
            default:
                break;
        }
    }

    void Explode(int size, int type, int explodeInto)
    {
        TileObject obj;
        for (int x = -size; x <= size; x++)
            for (int y = -size; y <= size; y++)
            {                
                obj = level.tiles[LevelX(position.x + x),LevelY(position.y + y)];
                if (!obj.solid)
                {
                    if (obj.explosive && obj != this)
                    {
                        obj.setToExplode = true;
                        obj.updated = true;
                    }
                    else
                    {
                        obj.Init(7);
                        obj.exData = explodeInto.ToString();
                        obj.direction = Vector2Int.zero;
                    }
                }
            }
    }
    #endregion

    #region collected
    void Collected(string collector)
    {
        switch (name)
        {
            case "dirt":
                {
                    amManager.PlaySound(AudioNames.DIG);
                    break;}
            case "diamond":
            case "fragile":
                {
                    if (collector == "player")
                    {
                        gmManager.collected++;
                        amManager.PlaySound(AudioNames.DIAMOND);
                    }
                    break;
                }
            case "potion":
                {
                    int id = (exData == "default") ? 0 : int.Parse(exData); 
                    lgScript.GetMagic(id);
                    amManager.PlaySound(AudioNames.POTION);
                    break;
                }
            case "exitopen":
                {
                    if (collector == "player")
                    {
                        gmManager.NextLevel();
                    }
                    break;
                }
        }
    }
    #endregion

    #region pushed
    void Pushed(string pusher)
    {
    }
    #endregion

    #region functions

    int LevelX(int x){return Functions.nfmod(x, level.size.x);}
    int LevelY(int y){return Functions.nfmod(y, level.size.y);}
    Vector2Int RotateLeft(Vector2Int vec){return new Vector2Int(vec.y, -vec.x);}
    Vector2Int RotateRight(Vector2Int vec){return new Vector2Int(-vec.y, vec.x);}
    Vector2Int Rotate(Vector2Int vec, int amount){while (amount-- > 0)
        {
            vec = RotateLeft(vec);
        }
        return vec;
    }
    Vector2Int LevelIn(Vector2Int vec){return new Vector2Int(LevelX(vec.x), LevelY(vec.y));}
    TileObject At(Vector2Int vec){return level.tiles[LevelX(position.x + vec.x), LevelY(position.y + vec.y)];}

    //Check adjacent tiles for tileobject of name
    bool Adj(string name)
    {
        foreach (Vector2Int vec in new List<Vector2Int> {Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down})
        {
            if (At(vec).name == name)
                return true;
        }
        return false;
    }

    //Check if can move relative to position
    bool CanTravel(Vector2Int vec)
    {
        TileObject obj = At(vec);
        return ((obj.name == "space" && obj.direction == Vector2Int.zero) || travels.Contains(obj.name));
    }

    //Simplified version
    bool CanTravel2(Vector2Int vec)
    {
        TileObject obj = At(vec);
        return ((obj.name == "space") || travels.Contains(obj.name));
    }

    //Move object relative to position
    void Move(Vector2Int vec)
    {
        //Set new position
        Vector2Int lastPos = position;
        position = LevelIn(position + vec);
        direction = vec;
        lastDir = direction;
  
        //Re-initialise the object in that position as space
        TileObject obj = level.tiles[position.x, position.y];
        obj.position = lastPos;
        obj.Collected(name);
        obj.Init(0);
        obj.updated = true;
        obj.direction = direction;

        //Swap the objects
        level.tiles[lastPos.x, lastPos.y] = obj;
        level.tiles[position.x, position.y] = this;
        updated = true;
    }

    //Push an object in direction, moving with it
    void Push(Vector2Int vec)
    {
        TileObject obj = At(vec);
        if (!obj.updated)
            obj.Update();
        if (At(vec) == obj)
        {
            obj.Move(vec);
            Move(vec);
        }
    }

    bool canPush(Vector2Int vec)
    {
        return (At(vec * 2).name == "space" && At(vec * 2).direction == Vector2Int.zero);
    }

    #endregion

    #region boulder
    void BoulderUpdate()
    {
        //Sounds
        AudioNames fallSound;
        AudioNames landSound;

        if (name == "diamond" || name== "fragile")
        {
            fallSound = AudioNames.DIAMOND_FALL;
            landSound = AudioNames.DIAMOND_LAND;
        }
        else
        {
            fallSound = AudioNames.BOULDER_FALL;
            landSound = AudioNames.BOULDER_LAND;
        }  

        //Fall if possible
        if (CanTravel(level.gravityDir))
        {
            Move(level.gravityDir);
            if (state != "falling")
                amManager.PlaySound(fallSound);
            state = "falling";
        }
        else
        {
           
            if ((name == "fragile" || name == "bomb") && state=="falling")
                ExplodeObject();
            //Fall right/left
            TileObject obj = At(level.gravityDir);
            if (obj.rounded)
            {
                Vector2Int right = RotateRight(level.gravityDir);
                if (CanTravel(right) && CanTravel(level.gravityDir + right))
                {
                    Move(right);
                    state = "falling2";
                }
                else
                {
                    Vector2Int left = RotateLeft(level.gravityDir);
                    if (CanTravel(left) && CanTravel(level.gravityDir + left))
                    {
                        Move(left);
                        state = "falling2";
                    }
                }
            }

            if (state == "falling" || state=="falling2")
            {
                state = "default";
                amManager.PlaySound(landSound);
                obj.ExplodeObject();
            }
            else if (state == "falling2")
                state = "falling";

        }
    }
    #endregion

    #region player
    void PlayerUpdate()
    {
        bool moved = false;
        Vector2Int wantDir = controls.direction;
        List<string> pushables = new List<string>{ "boulder", "ore", "bomb", "tnt" };
        lgScript.magicCoolDown--;

        //Magic direction
        Vector2Int magicDir = wantDir;
        if (magicDir == Vector2Int.zero)
            magicDir = lastDir;

        //Not using magic
        if (!controls.useMagic)
        {
            if (wantDir != Vector2Int.zero)
            {
                if (!controls.useAction)
                {
                    //Move in direction
                    if (CanTravel(wantDir))
                        Move(wantDir);

                //Push object in direction (left/right only)
                else
                    {
                        At(wantDir).Pushed(name);
                        if (pushables.Contains(At(wantDir).name) && canPush(wantDir) && wantDir.y == 0)
                        {
                            Push(wantDir);
                            amManager.PlaySound(AudioNames.PUSH);
                        }
                        
                    }
                    moved = true;
                }
            //Collect adjacent object
            //In future a particle effect will appear here
            else
                {
                    if (CanTravel(wantDir))
                    {
                        At(wantDir).Collected(name);
                        At(wantDir).Init(0);
                    }
                    lastDir = wantDir;
                }
            }
        }

        //Using magic
        else
        {
            if (lgScript.magicCoolDown < 0)
            {
                

                if (lgScript.magicName == "fireball")
                {
                    //Shoot a fireball
                    if (lgScript.tileTemplates[10].CanTravel2(magicDir+position))
                    {
                        TileObject obj = At(magicDir);
                        obj.Init(10);
                        obj.direction = magicDir; 
                        obj.lastDir = obj.direction;
                        lgScript.UsedMagic(1);
                        amManager.PlaySound(AudioNames.FIREBALL);
                    }
                }
            }
        }

        //States for animation
        if (moved)
        {
            //Can't use switch statement on Vector2Int
            if (wantDir == Vector2Int.right)
                state = "w_right";
            else if (wantDir == Vector2Int.up)
                state = "w_up";
            else if (wantDir == Vector2Int.left)
                state = "w_left";
            else
                state = "w_down";
        }
        else
        {
            //Can't use switch statement on Vector2Int
            if (lastDir == Vector2Int.right)
                state = "s_right";
            else if (lastDir == Vector2Int.up)
                state = "s_up";
            else if (lastDir == Vector2Int.left)
                state = "s_left";
            else
                state = "s_down";
        }
    }

    void PlayerAnimate()
    {
        switch (state)
        {
            case "s_right":
                {spriteIndex = 5;break;}
            case "s_down":
                {spriteIndex = 0;break;}
            case "s_left":
                {spriteIndex = 15;break;}
            case "s_up":
                {spriteIndex = 10;break;}
            case "w_right":
                {spriteIndex = 6 + AnimLoop(10f,4);break;}
            case "w_down":
                {spriteIndex = 1 + AnimLoop(10f,4);break;}
            case "w_left":
                {spriteIndex = 16 + AnimLoop(10f,4);break;}
            case "w_up":
                {spriteIndex = 11 + AnimLoop(10f,4);break;}
        }
    }
    #endregion

    #region explosion

    void ExplosionUpdate()
    {
        if (count > 4)
        {
            if (exData!="default")
                Init(int.Parse(exData));
        }
    }

    #endregion

    #region enemy

    void EnemyUpdate(int rotateDir = 0)
    {
        //Explode next to player or amoeba
        if (Adj("player") || Adj("amoeba"))
            ExplodeObject();

        //Go right if possible
        Vector2Int wantDir = Rotate(lastDir, 1 + rotateDir);
        if (wantDir == Vector2Int.zero)
            wantDir = Vector2Int.right;
        int tries = 3;
        while (tries-- > 0)
        {
            if (CanTravel(wantDir))
            {
                Move(wantDir);
                break;
            }
            else if (At(wantDir).direction != Rotate(wantDir, 2)) //The 'Don't BS me Boulder' rule
            {
                wantDir = Rotate(wantDir, 3 + rotateDir);
            }
        }
        lastDir = wantDir;

    }

    void RobotUpdate()
    {
        //Explode next to player or amoeba
        if (Adj("player") || Adj("amoeba"))
            ExplodeObject();

        //Move in a straight line
        if (CanTravel(lastDir))
        {
            Move(lastDir);
        }
        //Change direction if obstructed
        else
        {
            lastDir = new Vector2Int[] {Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down}[Random.Range(0,4)];
        }
    }

    void SpiderUpdate()
    {
        //Explode next to player or amoeba
        if (Adj("player") || Adj("amoeba"))
            ExplodeObject();

        //Not bordered
        Vector2Int playerPos = tmManager.playerPosition;
        if (!level.wrap)
        {
            if (position.y < playerPos.y && CanTravel(Vector2Int.up))
                Move(Vector2Int.up);
            else if (position.y > playerPos.y && CanTravel(Vector2Int.down))
                Move(Vector2Int.down);
            else if (position.x < playerPos.x && CanTravel(Vector2Int.right))
                Move(Vector2Int.right);
            else if (position.x > playerPos.x && CanTravel(Vector2Int.left))
                Move(Vector2Int.left);
        }

        //Bordered
        else
        {
            int y1 = (int)Functions.nfmod(playerPos.y - position.y, level.size.y);
            int y2 = (int)Functions.nfmod(position.y + level.size.y - playerPos.y, level.size.y);

            if (y1 < y2 && CanTravel(Vector2Int.up))
                Move(Vector2Int.up);
            else if (y1 > y2 && CanTravel(Vector2Int.down))
                Move(Vector2Int.down);
            else
            {
                int x1 = (int)Functions.nfmod(playerPos.x - position.x, level.size.x);
                int x2 = (int)Functions.nfmod(position.x + level.size.x - playerPos.x, level.size.x);
                if (x1 < x2 && CanTravel(Vector2Int.right))
                    Move(Vector2Int.right);
                else if (x1 > x2 && CanTravel(Vector2Int.left))
                    Move(Vector2Int.left);
            }
        }
            

    }

    void BeetleUpdate()
    {
        if (state == "default")
            EnemyUpdate(0);
        else
            EnemyUpdate(2);

        int dir=0;
        if (direction == Vector2Int.right)
            dir = 0;
        else if (direction == Vector2Int.up)
            dir = 1;
        else if (direction == Vector2Int.left)
            dir = 2;
        else if (direction == Vector2Int.down)
            dir = 3;

        if (dir == count % 3)
        {
            if (state == "default")
                state = "alternate";
            else
                state = "default";
        }
    }

    #endregion
   
    #region exit
    void ExitUpdate()
    {
        if (gmManager.collected >= gmManager.quota)
        {
            amManager.PlaySound(AudioNames.EXIT_OPEN);
            Init(12);
        }
    }

    #endregion

    #region magic

    //Fireball
    void FireballUpdate()
    {
        //Move in given direction
        if (CanTravel2(lastDir))
        {          
            string objName = At(lastDir).name;
            Move(lastDir);
            //Delete if moved through anything other than space
            if (objName != "space")
                FireballDestroy();
        }
        else
            FireballDestroy();
    }

    void FireballDestroy()
    {
        amManager.PlaySound(AudioNames.EXPLOSION);
        Explode(0, 0, 0);
    }

    #endregion
     
}