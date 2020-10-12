using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour { 


    public static AudioClip enemyEatSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        enemyEatSound = Resources.Load<AudioClip>("enemyEat");

        audioSrc = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip) {
        
                audioSrc.PlayOneShot(enemyEatSound);
        
    }
}
