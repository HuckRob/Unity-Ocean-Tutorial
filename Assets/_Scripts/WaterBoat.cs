using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBoat : MonoBehaviour
{
    //Public Properties
    public Transform Motor;
    public float StearPower = 200f;
    public float Power = 20f;
    public float MaxSpeed = 15f;
    public float Drag = 0.1f;
    public  Rigidbody rigidbody;
    //Used Protected Componets
    
    protected Quaternion StartRotation;


    public void awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        StartRotation = Motor.localRotation;
    }

    void FixedUpdate()
    {
        //default direction
        var forceDirection = transform.forward;
        var stear = 0;

        //steer direction [-1,0,1]
        if (Input.GetKey(KeyCode.A))
        {
            stear = 1;
        }
        if(Input.GetKey(KeyCode.D))
        {
            stear = - 1;
        }

        if(Motor == null)
        {
            Debug.Log("Cant Find Motor");
        }

        if(rigidbody == null)
        {
            Debug.Log("Cant Find Rigid Body");
        }
        //Rotational Force
        rigidbody.AddForceAtPosition(stear * transform.right * StearPower / 100f, Motor.position);

        //compute vectors for moving forward
        var forward = Vector3.Scale(new Vector3(1, 0 , 1), transform.forward);
        var targetVel = Vector3.zero;

        //forward / backward boat power
        if (Input.GetKey(KeyCode.W))
        {
            PhysicsHelper.ApplyForceToReachVelocity(rigidbody, forward * MaxSpeed, Power);
        }
        if (Input.GetKey(KeyCode.S))
        {
            PhysicsHelper.ApplyForceToReachVelocity(rigidbody,forward * -MaxSpeed, Power);
        }
    }
}
