using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{

    public AudioSource audio;
    public MyWalkingExperimentRunner myrunner;
    public GameObject TrialInstructions;

    public void Play()
    {
        if (toDisable.activeSelf == false)
        {
            if (TrialInstructions.activeSelf == false)
            {
                SetAI();
            }
            else
            {
                myrunner.SetUpNextTrial();
            }

        }
        else
        {
            myrunner.setUpFirstTrial();
            Disable();
            Debug.Log("playing");
            audio.Play();


        }


    }

    public void SetAI()
    {
        Debug.Log("SETTING AI");
        myrunner.GetAIReady();
    }



    public GameObject toDisable;
    public void Disable()
    {
        toDisable.SetActive(false);
    }



}
