using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemindPlayerAudio : MonoBehaviour
{
       public AudioSource audio;
   void Update(){
       // Debug.Log(InworldController.CurrentCharacter);
        if(Input.GetKey("a")){
            Play();
        }
  
    }

    public void Play(){
audio.Play();
    }
}
