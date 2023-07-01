using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.transform.parent.gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = Camera.main.transform.position - transform.position;
    }
}
