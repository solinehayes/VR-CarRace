using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class LocomotionTechnique : MonoBehaviour
{
    // Please implement your locomotion technique in this script. 
    public OVRInput.Controller leftController;
    public OVRInput.Controller rightController;
    [Range(0, 10)] public float translationGain = 0.5f;
    public GameObject hmd;
    [SerializeField] private float leftTriggerValue;    
    [SerializeField] private float rightTriggerValue;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool isIndexTriggerDown;

    public GameObject ObjectToMove;
    private Rigidbody objectRb;


    public GameObject steering_wheel;
    public GameObject throttle_lever;


    /////////////////////////////////////////////////////////
    // These are for the game mechanism.
    public ParkourCounter parkourCounter;
    public string stage;

    private float previousWheelAngle;
    
    void Start()
    {
        objectRb = ObjectToMove.GetComponent<Rigidbody>();
    }

    void Update()
    {
        float wheelAngle= steering_wheel.transform.localEulerAngles.z;
        float leverAngle = throttle_lever.transform.localEulerAngles.x;

        if (leverAngle >=340.0f){
            leverAngle = leverAngle -360.0f ;
        }
        if (wheelAngle >180.0f){
            wheelAngle = wheelAngle -360.0f ;
        }
        

        //Define speed
        float speed = speedFromAngle(leverAngle);
        
        //Add Rotation
        if(speed !=0){
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, - wheelAngle/50.0f, 0));
            objectRb.MoveRotation(objectRb.rotation * deltaRotation);
        }

        //Add moving force
        Vector3 forward = Vector3.Scale(new Vector3(1,0,1), ObjectToMove.transform.forward);
        objectRb.AddForce(forward * speed, ForceMode.Force);

        
        

        ////////////////////////////////////////////////////////////////////////////////
        // These are for the game mechanism.
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (parkourCounter.parkourStart)
            {
                this.transform.position = parkourCounter.currentRespawnPos;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        // These are for the game mechanism.
        if (other.CompareTag("banner"))
        {
            stage = other.gameObject.name;
            parkourCounter.isStageChange = true;
        }
        else if (other.CompareTag("coin"))
        {
            parkourCounter.coinCount += 1;
            this.GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);
        }
        // These are for the game mechanism.

    }
    float speedFromAngle(float angle){
        if(angle <=-10){
            return (angle+10.0f)*1000.0f;
        }
        else if (angle <=0){
            return (angle+10.0f)*5.0f;
        }
        return (15.0f*angle +1.0f)*50.0f;
        
    }
    
    Vector3 computeForce(float angle,float speed){
        //Computes the force to apply to the boat from the rotation angle around y 
        float angleInRadian = angle *3.14f /180.0f;
        return speed* new Vector3(Mathf.Abs(Mathf.Sin(angleInRadian)),0.0f,Mathf.Abs(Mathf.Cos(angleInRadian)));
    }
}