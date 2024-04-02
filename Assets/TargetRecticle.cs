using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TargetRecticle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //https://developer.oculus.com/documentation/unity/unity-ovrinput/
        float leftValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)[1];
        float rightValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick)[1];
      
    }
}
