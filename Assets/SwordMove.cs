using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMove : MonoBehaviour
{
    [SerializeField] Rigidbody rbChild;
    Rigidbody rb;

    [SerializeField] float accel = 10;
    [SerializeField] float deccel = .5f;
    [SerializeField] float maxSpd = 10;

    [SerializeField] float turnResponse;
    [SerializeField] float yaw, pitch, roll;

    [SerializeField] float stability = .3f;

    [SerializeField] GameObject smackObj;

    bool accelarating;

    [SerializeField] Transform backHand, frontHand;

    Queue<Vector3> pastVels = new Queue<Vector3>();

    //controls
    string RIGHT = "d";
    string LEFT = "a";
    string UP = "s";
    string DOWN = "w";
    string rollLEFT = "q";
    string rollRIGHT = "e";

    float ricochetCD = .5f;
    float useRicochetCD;

    Vector3 handsDir;
    float handsDistance = 5;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        handsDir = (frontHand.localPosition - backHand.localPosition).normalized;

        //pitch = handsDir.y;
        //yaw = handsDir.x;

        //forward and back
        if (Input.GetKey("space"))
            accelarating = true;
        //Accelerate();
        else accelarating = false;

        if (Input.GetMouseButton(1) && rb.velocity.magnitude > .2f)
            rb.AddForce(transform.forward *-deccel * Time.deltaTime);

        
        //rotation controls
        //yaw
        if (Input.GetKey(RIGHT))
            yaw += 1f * Time.fixedDeltaTime;
        if (Input.GetKey(LEFT))
            yaw -= 1f * Time.fixedDeltaTime;

        //pitch
        if(Input.GetKey(UP))
            pitch += 1f * Time.fixedDeltaTime;
        if (Input.GetKey(DOWN))
            pitch -= 1f * Time.fixedDeltaTime;

        //roll
        if (Input.GetKey(rollRIGHT))
            roll += 1f * Time.fixedDeltaTime;
        else if (Input.GetKey(rollLEFT))
            roll -= 1f * Time.fixedDeltaTime;
        else roll = Mathf.Lerp(roll, 0, Time.deltaTime * 10);

        yaw = Mathf.Clamp(yaw, -1, 1);
        yaw = Mathf.Lerp(yaw, 0, Time.deltaTime*10);

        pitch = Mathf.Clamp(pitch, -1, 1);
        pitch = Mathf.Lerp(pitch, 0, Time.deltaTime*10);

        roll = Mathf.Clamp(roll, -1, 1);

        //set hands to control vars
        float tempZ = backHand.localPosition.z;
        backHand.localPosition = new Vector3(-yaw, pitch)*handsDistance/2 + new Vector3(0,0,backHand.localPosition.z);
        frontHand.localPosition = new Vector3(-yaw, pitch)*-handsDistance/2 + new Vector3(0, 0, frontHand.localPosition.z);

        useRicochetCD -= Time.deltaTime;

    }

    private void FixedUpdate()
    {
        //track past velocity
        pastVels.Enqueue(rb.velocity);
        if (pastVels.Count > 3) pastVels.Dequeue();

        if (accelarating) Accelerate();

        //apply
        rb.AddRelativeTorque(Vector3.up * yaw * turnResponse/2);
        rb.AddRelativeTorque(-Vector3.forward * yaw);

        Vector3 lookTowards = transform.rotation.eulerAngles + new Vector3(0,10*Time.timeScale,0)*yaw;
        //transform.rotation = Quaternion.Euler(lookTowards);
        rb.AddTorque(transform.right * pitch * turnResponse);
        rb.AddRelativeTorque(-Vector3.forward * roll * turnResponse);

        //aesthetics
        //sword look at
        //if (!(Input.GetKey(DOWN)||Input.GetKey(UP) || Input.GetKey(RIGHT) || Input.GetKey(LEFT))) {
            //Vector3 flatForward = transform.forward;
            //flatForward = new Vector3(flatForward.x, 0, flatForward.z);
            //Vector3 lookDir = Vector3.Lerp(transform.forward, flatForward, Time.timeScale * stability);
            //transform.LookAt(transform.position + lookDir);
        //}
    }

    void Accelerate()
    {
        if (rb.velocity.magnitude > maxSpd) return;
        rb.AddForce(transform.forward * accel);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (useRicochetCD > 0) return;
        useRicochetCD = ricochetCD;

        Vector3 reflectVec = Vector3.Reflect(GetComponent<Rigidbody>().velocity, collision.GetContact(0).normal);
        Vector3 vel = pastVels.Peek();

        Debug.Log("rb velocity: " + vel);
        Debug.Log("reflect vector: " + reflectVec);

        /*
        if (Vector3.Angle(vel, -collision.GetContact(0).normal) < 30)
        {
            Dead();
            GameObject.Instantiate(smackObj, collision.GetContact(0).point, Quaternion.identity);
            return;
        }
        */

        Debug.Log("Richochet");

        GameObject.Instantiate(smackObj, collision.GetContact(0).point, Quaternion.identity);

        transform.Translate(reflectVec.normalized, Space.World);
        transform.Translate(collision.GetContact(0).normal, Space.World);
        //transform.forward = reflectVec;
        rb.velocity = reflectVec.normalized * rb.velocity.magnitude;
        rb.AddForce(reflectVec.normalized*accel/3, ForceMode.Impulse);
    }

    void Dead()
    {
        Debug.Log("dead");
        rb.velocity = Vector3.zero;
        transform.Translate(pastVels.Peek().normalized / 2, Space.World);
        rb.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(backHand.position, frontHand.position);
    }
}
