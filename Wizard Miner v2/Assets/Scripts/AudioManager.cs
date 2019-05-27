using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Sounds made using the 8-bit sound generator
 * http://www.superflashbros.net/as3sfxr/
*/

public enum AudioNames
{
    BOULDER_LAND,
    DEATH,
    DIAMOND_FALL,
    EXPLOSION,
    DIAMOND,
    DIAMOND_LAND,
    POWERUP,
    SHOOT,
    SOMETHING,
    DIG,
    BOULDER_FALL,
    FIREBALL,
    POTION,
    EXIT_OPEN,
    BUTTERFLY,
    BOULDREND,
    FRAGILE,
    ORE,
    PUSH,
    EPHORSU
}

public class AudioManager : MonoBehaviour {

    //Audio clips
    public AudioClip[] audioClips;

    //private AudioSource aSource;
    private List<AudioSource> aSources = new List<AudioSource>();

	// Use this for initialization
	void Start () 
    {
        TileObject.amManager = GetComponent<AudioManager>();
        //aSource = GetComponent<AudioSource>();
        for (int i = 0; i < System.Enum.GetValues(typeof(AudioNames)).Length; i++)
        {
            aSources.Add(gameObject.AddComponent(typeof(AudioSource)) as AudioSource);
            aSources[i].clip = audioClips[i];
            aSources[i].playOnAwake = false;
        }
	}

    //Play a sound from the sound library
    public void PlaySound(AudioNames soundId)
    {
        aSources[(int)soundId].Play();
    }
   
}
