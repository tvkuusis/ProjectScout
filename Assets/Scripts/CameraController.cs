using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class CameraController : MonoBehaviour
{
    [Tooltip("Orthographic camera size at ship's maximum speed")]
    public float camSizeMaxSpeed;
    [Tooltip("Orthographic camera size at ship's zero speed")]
    public float camSizeZeroSpeed;
    [Tooltip("Orthographic camera size at scene start, begins zooming into the ship at start. If zero, uses Main Camera's default setting")]
    public float camSizeStart;

    Camera _cam;
    ShipController _shipController;
    float _camSizeDelta;
    float _targetSize;
    float _camMinZoomVelocity = 50.0f; //Speed of the ship where maximum zoom-out is reached

    public void Start()
    {
        _cam = Camera.main; //Get reference to Main Camera

        //If not set, default to Main Camera's default size
        if(camSizeStart == 0)
        {
            camSizeStart = _cam.orthographicSize; 
        } else _cam.orthographicSize = camSizeStart;

        _shipController = GetComponent<ShipController>();
        _camSizeDelta = camSizeMaxSpeed - camSizeZeroSpeed;
    }

    public void Update()
    {

        //Set the camera zoom based on ship velocity
        _targetSize = camSizeZeroSpeed + (_shipController.RigidBody.velocity.magnitude / _camMinZoomVelocity) * _camSizeDelta;
        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetSize, Time.deltaTime);
        //Set the camera position based on ship position
        _cam.transform.position = new Vector2(_shipController.transform.position.x, _shipController.transform.position.y);


    }
}
