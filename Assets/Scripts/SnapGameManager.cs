
using UnityEngine;

public class SnapGameManager : MonoBehaviour
{

    public MySnapExperimentRunner myrunner;
    public GameObject TrialInstructions;
    // Update is called once per frame
    public void ButtonPress()
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
