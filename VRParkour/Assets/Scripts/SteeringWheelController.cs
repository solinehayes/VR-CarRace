using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SteeringWheelController : MonoBehaviour
{
    [SerializeField] private bool isInCollider;
    [SerializeField] private bool isSelected;

    public OVRInput.Controller controller;
    public GameObject wheel;
    public GameObject boat;
    private Transform bt;
    private float wheelSize = 0.4f;
    private Vector3 previousControllerPosition;
    private float triggerButtonValue;

    private Vector3 controllerInBoat;
    private Vector3 wheelInBoat;

    // Start is called before the first frame update
    void Start()
    {
        bt = boat.transform;

        //Get the position of the wheel in the coordonate system of the boat
        wheelInBoat = bt.InverseTransformPoint(wheel.transform.position);

    }

    // Update is called once per frame
    void Update()
    {        
        //Get the position of the controller in the coordinate system of the boat
        controllerInBoat = bt.InverseTransformPoint(this.transform.position);

        triggerButtonValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);

        if (isInCollider && !isSelected && triggerButtonValue >0.90){
            isSelected = true;
            previousControllerPosition = controllerInBoat;
            StartCoroutine(vibrate(0.1f,1.0f, controller));
        }
        else if (isInCollider && isSelected && triggerButtonValue <0.90){
            isSelected = false;
            StartCoroutine(vibrate(0.1f,1.0f,controller));
        }

        if(isInCollider && isSelected && (Mathf.Abs(controllerInBoat.y-previousControllerPosition.y)>=0.01 || Mathf.Abs(controllerInBoat.x-previousControllerPosition.x)>=0.01)){
            float distanceToWheelCenter = Vector3.Distance(previousControllerPosition,wheelInBoat);
            float distanceFromInitital = Vector3.Distance(previousControllerPosition,controllerInBoat);
            float rot_z = 2.0f * Mathf.Asin(distanceFromInitital/(2* distanceToWheelCenter))*180.0f/3.14f;
        
            Vector3 newRotation = wheel.transform.localEulerAngles + new Vector3(0.0f,0.0f,rot_z);

            
            //Transform angles from -180° -> 180° to angles to 0°-> 360°
            if (newRotation.z < 0.0f){
                newRotation.z = 360.0f + newRotation.z ;
            }
            else if (newRotation.z>360.0f){
                newRotation.z = newRotation.z - 360;
            }

            //Set the right orientation of the wheel turn
            if(previousControllerPosition.x < controllerInBoat.x ) {
                newRotation = wheel.transform.localEulerAngles - new Vector3(0.0f,0.0f,rot_z);
            } 

            //The wheel is only allowed -90° to 90° 
            if(newRotation.z < 270.0f && newRotation.z> 90.0f){
                StartCoroutine(vibrate(0.3f,1.0f, controller));
                if( newRotation.z  > 180.0f ){
                    newRotation = new Vector3(wheel.transform.localEulerAngles.x,wheel.transform.localEulerAngles.y, 270.0f );
                }
                else {
                    newRotation = new Vector3(wheel.transform.localEulerAngles.x,wheel.transform.localEulerAngles.y , 90.0f);
                }
            }


            //Update the wheel rotation and update the controller position
            wheel.transform.localEulerAngles = newRotation;
            previousControllerPosition = controllerInBoat;
        }
        else if(wheel.transform.localEulerAngles.z != 0 && triggerButtonValue <0.90 ){
            //When the wheel is not in zero and not held by the user it goes back to 0 slowly
            Vector3 newRotation;
            if(wheel.transform.localEulerAngles.z>180.0f){
               newRotation = wheel.transform.localEulerAngles - new Vector3(0.0f,0.0f,(wheel.transform.localEulerAngles.z-360.0f)/100.0f);
            }
            else {
                newRotation= wheel.transform.localEulerAngles - new Vector3(0.0f,0.0f,wheel.transform.localEulerAngles.z/100.0f);
            }
            wheel.transform.localEulerAngles = newRotation;

        }
        
    }
    void OnTriggerEnter(Collider other){
        if(isWheelCollided(other)){
            StartCoroutine(vibrate(0.1f,0.5f,controller));
            isInCollider = true;
        }
    }
    void OnTriggerExit(Collider other){
        if(other.CompareTag("Wheel") && isInCollider){
            StartCoroutine(vibrate(0.1f,0.5f,controller));
            isInCollider = false;
       }
    }

    bool isWheelCollided(Collider other){
        float distanceToWheelCenter = Mathf.Pow(this.transform.position.x-wheel.transform.position.x,2)+Mathf.Pow(this.transform.position.y-wheel.transform.position.y,2);
        if(!other.CompareTag("Wheel")) return false;
        if(distanceToWheelCenter <= wheelSize*wheelSize){
            return true;
        }
        
        return false;
    }

    IEnumerator vibrate(float seconds, float amplitude, OVRInput.Controller controller){
        OVRInput.SetControllerVibration(amplitude, amplitude, controller);
        yield return new WaitForSeconds(seconds);
        OVRInput.SetControllerVibration(0, 0, controller);

    }
    
}
