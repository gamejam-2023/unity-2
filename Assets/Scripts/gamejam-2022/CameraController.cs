using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float interpolation = 1.0f * Time.deltaTime;
        
        Vector3 position = this.transform.position;
        position.y = Mathf.Lerp(this.transform.position.y, target.transform.position.y, interpolation);
        position.x = Mathf.Lerp(this.transform.position.x, target.transform.position.x, interpolation);
        
        this.transform.position = position;
    }
}
