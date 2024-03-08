using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Inworld;
using System.Runtime.CompilerServices;

public class MyExperimentRunner : MonoBehaviour
{

    public Transform[] spawnPoints;

    public CharacterSet[] FemalecharacterSets;
    public CharacterSet[] MalecharacterSets;
    public GameObject blockStartIndicator;  // GameObject to display at the start of each block
    public string[] AIGender = { "Female", "Male" };
    public string[] Locomotion = { "Walking", "Teleportation" };
    public int TrialRepetitions;

    private GameObject thisTrialsAI;

    public RawImage PaintingImage;

    public GameObject PreTrialPainting;//painting stuff
    public GameObject TrialInstructions;//instructions for candidiate
    public GameObject OptionToStartNextTrial;//allow button to appear

    public GameObject EntireTrialCanvas;
    public GameObject Finished;



    private int currentTrial=0;
    private int currentBlock=0;
    private List<ExpTrial> experimentDesign = new List<ExpTrial>();


    private int LastTrial;

      public SendPosition sendPositionScript;

    void Start()
    {
     //   InworldController.CurrentCharacter = null;
        LastTrial = TrialRepetitions * Locomotion.Length*AIGender.Length;
        Debug.Log("Number of Trials: "+LastTrial);


        GenerateExpDesign();
        DebugExpDesign();

        Debug.Log("Starting Experiment");

    }


    void DebugCharacterSets(CharacterSet[] sets, string gender)
    {
        Debug.Log($"{gender} Character Sets:");
        for (int i = 0; i < sets.Length; i++)
        {
            Debug.Log($"{i}: {sets[i]?.characterPrefab?.name} - {sets[i]?.characterTexture?.name}");
        }
    }


    void GenerateExpDesign()
    {
        List<CharacterSet> availableFemaleSets = new List<CharacterSet>(FemalecharacterSets);
        List<CharacterSet> availableMaleSets = new List<CharacterSet>(MalecharacterSets);

        foreach (string locomotion in Locomotion)
        {
            List<string> genders = new List<string>(AIGender);

            // Duplicate each element in the genders list
            genders = genders.SelectMany(g => Enumerable.Repeat(g, TrialRepetitions)).ToList();

            // Randomize the order of genders
            System.Random rng = new System.Random();
            genders = genders.OrderBy(x => rng.Next()).ToList();

            foreach (string gender in genders)
            {
                CharacterSet selectedSet = null;

                if (gender == "Female" && availableFemaleSets.Count > 0)
                {
                    // Randomly select a Female character set
                    int randomIndex = rng.Next(availableFemaleSets.Count);
                    selectedSet = availableFemaleSets[randomIndex];

                    // Remove the selected set from the available sets
                    availableFemaleSets.RemoveAt(randomIndex);
                }
                else if (gender == "Male" && availableMaleSets.Count > 0)
                {
                    // Randomly select a Male character set
                    int randomIndex = rng.Next(availableMaleSets.Count);
                    selectedSet = availableMaleSets[randomIndex];

                    // Remove the selected set from the available sets
                    availableMaleSets.RemoveAt(randomIndex);
                }

                // Add the trial with the selected character set
                experimentDesign.Add(new ExpTrial(gender, locomotion, selectedSet));
            }
        }
    }



    //run when use hits "ready on block instructions"
    public void ContinueExperimentAfterBlockInstructions() {
          sendPositionScript.BeginRecord();
        PreTrialPainting.SetActive(true);
        TrialInstructions.SetActive(false);
        OptionToStartNextTrial.SetActive(false);
        if (currentTrial != LastTrial) {
            ExpTrial thisTrial = experimentDesign[currentTrial];
            CharacterSet thisSet = thisTrial.TrialSet;
            Texture thisPainting = thisSet.characterTexture;
            PaintingImage.texture = thisPainting;
        }
     
    }
 
    private bool firstTeleport = true;
    //run when the player hits "next"
    public void SetUpNextTrial()
    {
        sendPositionScript.EndRecord();
        currentTrial++;
        if (currentTrial != 0)
        {
            experimentDesign[currentTrial - 1].TrialSet.characterPrefab.SetActive(false);
        }
        if (currentTrial == LastTrial) {
            Debug.Log("Ending Experiment");
            EndExperiment();
            return;
        }
            Debug.Log("Trial Now Set to:"+currentTrial);
       // Debug.Log(experimentDesign[currentTrial].Locomotion);

        if (experimentDesign[currentTrial].Locomotion== "Teleportation" && firstTeleport) {
            Debug.Log("block");
            StartNewBlock() ;
        }
        else {
        
            PreTrialPainting.SetActive(true);
            TrialInstructions.SetActive(false);
            OptionToStartNextTrial.SetActive(false);
            if (currentTrial < LastTrial)
            {
                ExpTrial TrialData = experimentDesign[currentTrial];
                Texture NextPainting = TrialData.TrialSet.characterTexture;
                PaintingImage.texture = NextPainting;
            }



        }
   
      
    }



    private GameObject thisAI;
    //when the player is ready (viewed the painting and about to go speak to AI)
    public void GetAIReady() {
          sendPositionScript.BeginRecord();
        PreTrialPainting.SetActive(false);
        TrialInstructions.SetActive(true);
        OptionToStartNextTrial.SetActive(false);
         thisAI = experimentDesign[currentTrial].TrialSet.characterPrefab;
        thisAI.SetActive(true);
        DisableCanvas();
        matchTransform(thisAI, spawnPoints[currentTrial%2]);
        setCurrentCharacter(thisAI);
    }



    //run when begin speaking to character
    public void EnableNextTrialOption()
    {
        OptionToStartNextTrial.SetActive(true);

    }




    //helper functions
    public void DisableCanvas()
    {
        GameObject canvasObject = thisAI.transform.Find("Canvas").gameObject;
        if (canvasObject != null)
        {
            Canvas canvasComponent = canvasObject.GetComponent<Canvas>();
            canvasComponent.enabled = false;
        }
    }
    // Optional Pre-Trial code. Useful for waiting for the participant to
    // do something before each trial (multiple frames). Also might be useful for fixation points etc.

    private void EndExperiment() {
        EntireTrialCanvas.SetActive(false);
        Finished.SetActive(true);

    }
    private void StartNewBlock()
    {
        componentSet(true);//allow teleportation components
        firstTeleport = false;
        TrialInstructions.SetActive(false);
        OptionToStartNextTrial.SetActive(false);
        currentBlock++;
        Debug.Log($"Starting Block {currentBlock}");

        // Display the blockStartIndicator GameObject
        if (blockStartIndicator != null)
        {
            blockStartIndicator.SetActive(true);
        }

        // Additional logic or actions for the start of a new block can be added here
    }





    public void matchTransform(GameObject toChange, Transform toMatch)
    {
        toChange.transform.position = toMatch.position;
        toChange.transform.rotation = toMatch.rotation;
    }

    public void setCurrentCharacter(GameObject CharacterObj)
    {

        Inworld.InworldCharacter inworldCharacterComponent = CharacterObj.GetComponent<Inworld.InworldCharacter>();
        if (inworldCharacterComponent != null)
        {
            InworldController.CurrentCharacter = inworldCharacterComponent;
        }
        else
        {
            Debug.LogError("InworldCharacter component not found on the GameObject.");
        }
        Debug.Log("Inworld current character set to:" + InworldController.CurrentCharacter);
    }
    void DebugExpDesign()
    {
        Debug.Log("Experimental Design:");

        foreach (ExpTrial trial in experimentDesign)
        {
            string characterInfo = trial.TrialSet != null
                ? $"Character: {trial.TrialSet.characterPrefab.name}, Texture: {trial.TrialSet.characterTexture.name}"
                : "Character: null, Texture: null";

            Debug.Log($"Gender: {trial.Gender}, Locomotion: {trial.Locomotion}, {characterInfo}");
        }
    }
    public GameObject floor;
public string[] components;
   public void componentSet(bool enabled)
    {
        if (floor != null)
        {
            foreach (string componentName in components)
            {
                // Try to find the component by name
                MonoBehaviour component = floor.GetComponent(componentName) as MonoBehaviour;

                // If the component is found, disable it
                if (component != null)
                {
                    component.enabled = enabled;
                    Debug.Log($"Component: {componentName}. is enabled:{enabled}");
                }
                else
                {
                    Debug.LogWarning($"Component not found: {componentName}");
                }
            }
        }
    }


    // Helper class to represent a single experimental trial
    private class ExpTrial
    {
        public string Gender { get; }
        public string Locomotion { get; }

        public CharacterSet TrialSet { get; }

        public ExpTrial(string gender, string locomotion, CharacterSet trialSet)
        {
            Gender = gender;
            Locomotion = locomotion;
            TrialSet = trialSet;
        }
    }
}

