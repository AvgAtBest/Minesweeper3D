using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float zoomSpeed = 10f; //speed at which the camera zooms
    public float xSpeed = 120f;
    public float ySpeed = 120f;
    public float yMin = -80f;
    public float yMax = 80f;
    public float distanceMin = 10f;
    public float distanceMax = 15f;
    private float x = 0f;
    private float y = 0f;
    private float distance;
	// Use this for initialization
	void Start ()
    {
        distance = distanceMax;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        //if right mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            x += mouseX * xSpeed * Time.deltaTime;
            y -= mouseY * ySpeed * Time.deltaTime;
            y = Mathf.Clamp(y, yMin, yMax);

        }
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollWheel * zoomSpeed;
        distance = Mathf.Clamp(distance, distanceMin, distanceMax);
        transform.rotation = Quaternion.Euler(y, x, 0);
        transform.position = -transform.forward * distance;
	}
}
