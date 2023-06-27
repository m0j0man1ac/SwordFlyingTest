using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMove : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float accel = 10;
    [SerializeField] float deccel = .5f;
    [SerializeField] float maxSpd = 10;

    [SerializeField] float turnResponse;
    [SerializeField] float yaw;
    [SerializeField] float pitch;

    [SerializeField] float stability = .3f;

    //controls
    string RIGHT = "d";
    string LEFT = "a";
    string UP = "s";
    string DOWN = "w";

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //forward and back
        if (Input.GetMouseButton(0))
            Accelerate();
        if (Input.GetMouseButton(1) && rb.velocity.magnitude > .2f)
            rb.AddForce(transform.forward *-deccel);

        //yaw
        if (Input.GetKey(RIGHT))
        {
            yaw += 1f * Time.fixedDeltaTime;
        }
        if (Input.GetKey(LEFT))
            yaw -= 1f * Time.fixedDeltaTime;

        //pitch
        if(Input.GetKey(UP))
        {
            pitch += 1f * Time.fixedDeltaTime;
        }
        if (Input.GetKey(DOWN))
        {
            pitch -= 1f * Time.fixedDeltaTime;
        }

        yaw = Mathf.Clamp(yaw, -1, 1);
        yaw = Mathf.Lerp(yaw, 0, Time.deltaTime*10);

        pitch = Mathf.Clamp(pitch, -1, 1);
        pitch = Mathf.Lerp(pitch, 0, Time.deltaTime*10);


        
    }

    private void FixedUpdate()
    {
        //apply
        rb.AddTorque(transform.up * yaw * turnResponse * Time.deltaTime);
        rb.AddTorque(transform.right * pitch * turnResponse * 3 * Time.deltaTime);

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
        rb.AddForce(transform.forward*accel);

        //clamp
        if (rb.velocity.magnitude > maxSpd)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpd);
    }
}
