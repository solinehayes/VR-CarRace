using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GearController : MonoBehaviour
{

    [SerializeField] private bool isInCollider;
    [SerializeField] private bool isSelected;

    public TMP_Text debugText;

    public OVRInput.Controller controller;
    public GameObject gearObject;
    public GameObject boat;
    private Transform bt;


    private Vector3 initialControllerPosition;
    private float triggerButtonValue;


    private float leverSize = 0.275f;

    private Vector3 controllerInBoat;

    // Start is called before the first frame update
    void Start()
    {
        bt = boat.transform;
    }

    // Update is called once per frame
    void Update()
    {
        controllerInBoat = bt.InverseTransformPoint(this.transform.position);
        triggerButtonValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);


        if (isInCollider && !isSelected && triggerButtonValue >0.90){
            isSelected = true;
            initialControllerPosition = controllerInBoat;
            StartCoroutine(vibrate(0.1f,1.0f, controller));
        }
        else if (isInCollider && isSelected && triggerButtonValue <0.90){
            isSelected = false;
            StartCoroutine(vibrate(0.1f,1.0f,controller));
        }

        else if(isInCollider && isSelected && Mathf.Abs(controllerInBoat.y-initialControllerPosition.y)>=0.01){
            float h = controllerInBoat.y-initialControllerPosition.y;
            float rot_x = Mathf.Asin(h/leverSize)*180.0f/3.14f*0.9f;
            Vector3 newRotation = gearObject.transform.localEulerAngles + new Vector3(rot_x,0.0f,0.0f);

            //Transform angles from -180° -> 180° to angles to 0°-> 360°
            if (newRotation.x < 0.0f){
                newRotation.x = 360.0f + newRotation.x ;
            }
            else if (newRotation.x>360.0f){
                newRotation.x = newRotation.x - 360;
            }

            //Handle lever physical boundries
            if(newRotation.x < 340.0f && newRotation.x> 60.0f){
                StartCoroutine(vibrate(0.3f,1.0f, controller));
                if( 2* newRotation.x - 400 >0){
                    newRotation = new Vector3(340.0f,gearObject.transform.localEulerAngles.y,gearObject.transform.localEulerAngles.z );
                }
                else {
                    newRotation = new Vector3(60.0f,gearObject.transform.localEulerAngles.y,gearObject.transform.localEulerAngles.z );
                }
            }
            gearObject.transform.localEulerAngles = newRotation;
            initialControllerPosition = controllerInBoat;
        }


    }
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Throttle")){
            StartCoroutine(vibrate(0.1f,0.5f, controller));
            isInCollider = true;
        }
    }
    void OnTriggerExit(Collider other){
        if(other.CompareTag("Throttle")){
            StartCoroutine(vibrate(0.1f,0.5f,controller));
            isInCollider = false;
            isSelected = false;
        }
    }

    IEnumerator vibrate(float seconds, float amplitude, OVRInput.Controller controller){
        OVRInput.SetControllerVibration(amplitude, amplitude, controller);
        yield return new WaitForSeconds(seconds);
        OVRInput.SetControllerVibration(0, 0, controller);

    }
}
