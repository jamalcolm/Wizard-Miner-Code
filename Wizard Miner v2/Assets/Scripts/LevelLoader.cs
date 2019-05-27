using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The LevelLoader script loads levels from a list.
 * (An example of how these levels are formatted is below)
*/

public class LevelLoader : MonoBehaviour {

    public Dictionary<string, string> levelData = new Dictionary<string,string>();
    public List<TextAsset> levelsList;
    public int levelNo;

    public void LoadLevel()
    {
        if (levelNo < levelsList.Count)
        {
            //Get text
            string text = levelsList[levelNo].text;

            //Split by new-line
            string[] lines = text.Split('\n');

            foreach (string line in lines)
            {
                //Get property/value of each line by splitting by '=' sign
                string property = "";
                string value = "";
                string[] split = line.Split('=');

                if (split.Length >= 2)
                {
                    property = split[0];
                    value = split[1];

                    levelData[property] = value;
                    //Debug.Log("levelData[" + property + "]=" + value);
                }
            }
        }
    }         
}

//Example level: Level properties (width, height etc.) are defined. Adding extra level properties is relatively straightforward.
//'tiles' contains an ID for the starting objects in each tile.

/*
 * 
width=30
height=20
time=100
quota=18
bordered=0
tiles=5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,14,0,0,0,0,0,0,5,5,5,14,5,0,0,0,0,0,0,
5,0,0,0,0,0,0,0,0,0,5,5,0,0,0,0,0,0,0,5,5,5,0,5,0,0,0,0,0,0,0,0,0,0,5,0,0,0,5,0,5,5,0,0,0,0,0,0,0,5,5,5,0,
5,0,0,0,0,0,0,5,0,5,0,5,0,5,0,5,0,5,5,0,0,0,3,0,0,0,6,6,6,0,5,0,0,0,0,0,0,5,0,5,0,5,0,5,0,5,0,5,5,0,0,0,0,
0,0,0,5,5,5,0,5,0,0,0,0,0,0,5,0,5,0,14,0,5,14,5,0,5,5,0,0,0,0,0,0,0,5,5,5,0,5,0,0,0,0,0,0,5,0,5,0,5,0,5,0,
5,0,5,5,0,0,0,0,0,0,0,5,5,5,0,5,0,0,0,0,0,0,5,0,5,0,5,0,5,0,5,0,5,5,5,5,5,5,5,5,5,5,5,5,0,5,0,0,0,0,0,0,5,
0,0,0,5,0,0,0,5,0,5,5,0,0,0,0,0,0,0,0,0,5,0,5,5,5,5,0,5,5,5,0,0,0,0,0,0,0,0,0,5,5,5,5,0,5,5,5,0,0,0,5,6,5,
1,1,1,1,1,1,5,5,5,5,5,5,6,5,5,5,5,5,0,0,0,0,0,5,0,0,0,0,6,5,0,0,0,0,0,0,5,0,0,0,0,0,6,0,0,0,5,5,0,0,0,0,0,
5,0,0,0,5,6,5,0,0,0,0,0,0,5,0,0,0,0,5,6,5,0,0,5,5,0,0,0,0,0,5,6,5,6,5,5,5,0,0,0,0,0,0,5,0,0,0,0,5,5,5,0,0,
5,5,0,0,0,0,0,5,6,5,6,5,5,5,0,0,0,0,0,0,5,0,0,0,0,0,0,0,0,0,5,5,0,0,0,0,0,5,6,5,6,5,5,5,14,0,0,0,0,0,5,5,0,
5,5,5,5,5,5,5,5,5,0,0,0,0,14,5,14,5,14,5,5,5,14,0,0,0,0,0,5,0,0,0,0,0,0,0,0,14,5,5,5,5,0,5,5,5,5,5,5,5,5,5,
1,1,1,1,1,1,5,5,5,5,0,5,5,5,5,5,5,5,0,0,0,0,0,0,6,6,6,0,0,0,0,0,0,0,0,0,5,0,0,0,0,0,0,0,0,14,11,5,5,5,5,5,5,
5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
 * */