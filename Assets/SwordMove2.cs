using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordMove2 : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform meshes;

    [SerializeField] float throttle;
    [SerializeField] float accel = 10;
    [SerializeField] float maxSpd = 10;

    //[SerializeField] float turnResponse;
    [SerializeField] float yaw;
    [SerializeField] float pitch;

    //controls
    string RIGHT = "d";
    string LEFT = "a";
    string UP = "w";
    string DOWN = "s";

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //update axis of movement
        //Throttle/Acceleration
        if (Input.GetMouseButton(0))
            throttle += Time.deltaTime;
        if (Input.GetMouseButton(1))
            throttle -= Time.deltaTime;

        //yaw
        if (Input.GetKey(RIGHT))
            yaw += 1f * Time.fixedDeltaTime;
        if (Input.GetKey(LEFT))
            yaw -= 1f * Time.fixedDeltaTime;

        //pitch
        if (Input.GetKey(UP))
            pitch += 1f * Time.fixedDeltaTime;
        if (Input.GetKey(DOWN))
            pitch -= 1f * Time.fixedDeltaTime;

        throttle = Mathf.Clamp(throttle, -1, 1);
        if (Mathf.Abs(throttle) < .2f)
            throttle = Mathf.Lerp(throttle, 0, Time.deltaTime);

        yaw = Mathf.Clamp(yaw, -1, 1);
        yaw = Mathf.Lerp(yaw, 0, Time.deltaTime * 5);

        pitch = Mathf.Clamp(pitch, -1, 1);
        pitch = Mathf.Lerp(pitch, 0, Time.deltaTime * 5);


        //aesthetics
        //sword look at
        Vector3 moveVec = new Vector3(yaw, pitch, throttle + 1);
        moveVec *= accel;
        moveVec += rb.velocity;
        moveVec = Vector3.Lerp(meshes.forward, moveVec, Time.deltaTime/2);
        meshes.LookAt(transform.position+moveVec);
    }

    private void FixedUpdate()
    {
        //apply movement
        rb.AddForce(transform.forward * throttle * accel);
        rb.AddForce(transform.right * yaw * accel);
        rb.AddForce(transform.up * pitch * accel);

        //clamp
        if (rb.velocity.magnitude > maxSpd)
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpd);
    }
}
