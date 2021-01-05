using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    public float engineForce = 5f;
    [Tooltip("EXPERIMENTAL: for non-classic control that handles single thruster usage separately from L+R thruster combo")]
    public float sideThrusterToMainThrusterRatio = 1;
    public float maxVelocity = 10f;
    public enum ControlType { Original, BoostedThrusters, RigidBodyRotation }
    [Tooltip("EXPERIMENTAL: Non-classic control handles single thruster usage separately from L+R thruster combo. RigidBody mode rotates rigidbody instead of adding force to thrusters")]
    public ControlType currentControlType;
    public float maxAngularVelocity = 10f;
    public Transform leftEngine;
    public Transform rightEngine;
    public Rigidbody2D RigidBody { get; private set; }
    public Text controlText; // Debug text

    [SerializeField]
    bool _leftEngineOn;
    [SerializeField]
    bool _rightEngineOn;
    Vector2 _spawn;

    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        _spawn = transform.position;

        StartCoroutine(EngineRoutine());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ToggleLeftEngine(true);
        }
        else if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            ToggleLeftEngine(false);
        }

        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ToggleRightEngine(true);
        }
        else if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            ToggleRightEngine(false);
        }
    }

    public void ToggleLeftEngine(bool newState)
    {
        _leftEngineOn = newState;
    }

    public void ToggleRightEngine(bool newState)
    {
        _rightEngineOn = newState;
    }

    public void Reset()
    {
        transform.position = _spawn;
        transform.rotation = Quaternion.identity;
        RigidBody.velocity = Vector2.zero;
        RigidBody.angularVelocity = 0;
    }

    public void SwitchControlScheme()
    {
        switch(currentControlType)
        {
            case ControlType.Original:
                currentControlType = ControlType.BoostedThrusters;
                break;
            case ControlType.BoostedThrusters:
                currentControlType = ControlType.RigidBodyRotation;
                break;
            case ControlType.RigidBodyRotation:
                currentControlType = ControlType.Original;
                break;
        }

        controlText.text = currentControlType.ToString();
    }

    IEnumerator EngineRoutine()
    {
        YieldInstruction yi = new WaitForFixedUpdate();
        while(true)
        {
            switch(currentControlType)
            {
                case ControlType.Original:
                    if(_leftEngineOn)
                    {
                        if(RigidBody.angularVelocity > -maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * engineForce, leftEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * engineForce, Color.red);
                        }
                    }

                    if(_rightEngineOn)
                    {
                        if(RigidBody.angularVelocity < maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(rightEngine.up * engineForce, rightEngine.position);
                            Debug.DrawLine(rightEngine.position, rightEngine.position + rightEngine.up * engineForce, Color.green);
                        }
                    }
                    break;
                case ControlType.BoostedThrusters:
                    if(_leftEngineOn && !_rightEngineOn)
                    {
                        if(RigidBody.angularVelocity > -maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * engineForce, leftEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * engineForce * sideThrusterToMainThrusterRatio, Color.red);
                        }
                    }
                    else if(_rightEngineOn && !_leftEngineOn)
                    {
                        if(RigidBody.angularVelocity < maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(rightEngine.up * engineForce, rightEngine.position);
                            Debug.DrawLine(rightEngine.position, rightEngine.position + rightEngine.up * engineForce * sideThrusterToMainThrusterRatio, Color.green);
                        }
                    }
                    else if(_leftEngineOn && _rightEngineOn)
                    {
                        if(RigidBody.velocity.sqrMagnitude < maxVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * engineForce, leftEngine.position);
                            RigidBody.AddForceAtPosition(rightEngine.up * engineForce, rightEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * engineForce, Color.red);
                        }
                    }
                    break;
                case ControlType.RigidBodyRotation:
                    if(_leftEngineOn && !_rightEngineOn)
                    {
                        RigidBody.angularVelocity = 0;
                        RigidBody.MoveRotation(RigidBody.rotation - maxAngularVelocity * Time.deltaTime);
                    }

                    if(_rightEngineOn && !_leftEngineOn)
                    {
                        RigidBody.angularVelocity = 0;
                        RigidBody.MoveRotation(RigidBody.rotation + maxAngularVelocity * Time.deltaTime);
                    }

                    if(_leftEngineOn && _rightEngineOn)
                    {
                        if(RigidBody.velocity.sqrMagnitude < maxVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * engineForce, leftEngine.position);
                            RigidBody.AddForceAtPosition(rightEngine.up * engineForce, rightEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * engineForce, Color.red);
                        }
                    }
                    break;
            }

            yield return yi;
        }
    }

    private void OnDrawGizmos()
    {
        if(RigidBody)
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)RigidBody.velocity);
        }
    }
}