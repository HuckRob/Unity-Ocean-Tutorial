using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WaterFloat : MonoBehaviour
{
    //public properties
    public float AirDrag = 1;
    public float WaterDrag = 10;
    public bool AffectDirection = true;
    public bool AttachToSurface = false;
    public Transform[] FloatPoints;

    //used components
    protected Rigidbody rigidBody;
    protected Waves Waves;
    protected float WaterLine;
    protected Vector3[] WaterLinePoints;

    //help Vectors 
    protected Vector3 centerOffset;
    protected Vector3 TargetUp;
    protected Vector3 smoothVectorRotation;


    public Vector3 Center { get { return transform.position + centerOffset; } }

   
    void Awake()
    {
        //get components
        Waves = FindObjectOfType<Waves>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;

        //compte center
        WaterLinePoints = new Vector3[FloatPoints.Length];
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            WaterLinePoints[i] = FloatPoints[i].position;
        }
        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Default water surface
        var newWaterLine = 0f;
        var pointUnderWater = false;

        //set WaterLinePoints and WaterLine
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            //height
            WaterLinePoints[i] = FloatPoints[i].position;
            WaterLinePoints[i].y = Waves.GetHeight(FloatPoints[i].position);
            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length;
            if (WaterLinePoints[i].y > FloatPoints[i].position.y)
            {
                pointUnderWater = true;
            }
        }

        var waterLineDelta = newWaterLine - WaterLine;
        WaterLine = newWaterLine;

        //compute up vector
        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        //gravity
        var gravity = Physics.gravity;
        rigidBody.drag = AirDrag;
        if (WaterLine > Center.y)
        {
            rigidBody.drag = WaterDrag;
            //under water
            if (AttachToSurface)
            {
                //attach to water surface
                rigidBody.position = new Vector3(rigidBody.position.x, WaterLine - centerOffset.y, rigidBody.position.z);
            }
            else
            {
                //go up
                gravity = AffectDirection ? TargetUp * -Physics.gravity.y : -Physics.gravity;
                transform.Translate(Vector3.up * waterLineDelta * 0.9f);
            }
        }
        rigidBody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0, 1));

        //compute up vector
        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        //Rotation Hanlder
        if (pointUnderWater)
        {
            //attach to water surface
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotation, 0.2f);
            rigidBody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * rigidBody.rotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (FloatPoints == null){
            return;
        }

        for (int i = 0; i < FloatPoints.Length; i++)
        {
            if (FloatPoints[i] == null)
            {
                continue;
            }

            if(Waves != null)
            {
                //Draw Cube
                Gizmos.color = Color.red;
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }

            //draw Sphere
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);
        }

        //Draw Center
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 0.3f);
            Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
        }
        
    }
}
