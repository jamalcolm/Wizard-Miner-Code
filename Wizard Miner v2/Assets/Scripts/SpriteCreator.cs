using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * To give a bit of style to each level, The Dirt and Wall sprites are procedurally generated.
 * The colours chosen are random, but care has been taken to make sure it is not too dark to see.
*/

public class SpriteCreator : MonoBehaviour {

    public int size;
    public int seed;
    public Color primaryColour;
    public Color secondaryColour;
    private Color[] spriteColours;

    private int wallWidth;
    private int wallHeight;
    private int wallXoff;
    private int wallYoff;

    public Sprite[] gemSprites;
    public Sprite gemSprite1;
    public Sprite gemSprite2;

    // Use this for initialization
    void Start() 
    {
        Init();
    }	

    public void Init()
    {
        wallWidth = size/Random.Range(1,4);
        wallHeight = size/Random.Range(1,4);
        wallXoff = size/Random.Range(1,4);
        wallYoff = size/Random.Range(1,4);
        spriteColours = new Color[8];
        float v = 0.2f;
              
        //Set colour for dirt/wall, check that it is not too dark
        do
        {
            ColourSet();
        }
        while (
            (GetValue(spriteColours[0])<v && GetValue(spriteColours[1])<v && GetValue(spriteColours[2])<v && GetValue(spriteColours[3])<v)
            || (GetValue(spriteColours[4])<v && GetValue(spriteColours[6])<v)
        );

        int x = Random.Range(0, gemSprites.Length/2)*2;
        gemSprite1 = gemSprites[x];
        gemSprite2 = gemSprites[x+1];

    }

    public void ColourSet()
    {
        primaryColour = Random.ColorHSV();
        secondaryColour = Random.ColorHSV();
        float amount = Random.Range(-2f,2f);
        spriteColours[0] = primaryColour;
        spriteColours[1] = primaryColour + secondaryColour*amount;
        spriteColours[2] = primaryColour - secondaryColour*amount/2;
        spriteColours[3] = primaryColour + (primaryColour*secondaryColour)*amount/4;
        spriteColours[4] = secondaryColour;
        spriteColours[5] = secondaryColour + primaryColour*amount;
        spriteColours[6] = secondaryColour - primaryColour*amount/2;
        spriteColours[7] = secondaryColour - (primaryColour*secondaryColour)*amount/4;
    }

    //Generate new sprite of type 'type', used to give a bit of style to levels
    public Sprite GenerateSprite(string type)
    {
        Sprite mySprite;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colours = new Color[size*size];
        for (int i = 0; i < colours.Length; i++)
        {
            int x;
            switch (type)
            {
                case "dirt":
                    {
                        x = GetDirtColour(i);
                        break;
                    }
                case "wall":
                    {
                        x = GetWallColour(i);
                        break; 
                    }
                default:
                    {x = 0;break;}
            }
            colours[i] = spriteColours[x];
        }

        tex.SetPixels(colours);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        mySprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 0f), size);
        return mySprite;
    }

    int GetDirtColour(int n)
    {
        return 3-(int)Mathf.Sqrt(Random.Range(0, 16));
    }

    int GetWallColour(int n)
    {
        int r = 0;


        bool isCement = (n % wallWidth == ((n/(size*wallHeight))*wallXoff)%wallWidth || (n / size) % wallHeight == (n*wallYoff/size)%wallHeight);
        if (isCement)
        {
            r = 6;
        }
        else
            r = 4;
        return r;
    }

    float GetValue(Color c)
    {
        return (c.r%1 + c.g%1 + c.b%1) / 3f;
    }
       
}
