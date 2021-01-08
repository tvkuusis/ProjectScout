using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{

    public enum ControlType { Original, BoostedThrusters, BoostedThrustersSpeedScale, RigidBodyRotation }
    public ControlType currentControlType;
    public Transform leftEngine;
    public Transform rightEngine;
    public Rigidbody2D RigidBody { get; private set; }
    public Text controlText; // Debug text
    public ShipVariables shipVariables;
    public float dualTapTimeWindow = 0.2f;
    public float tapTimeWindow = 0.2f;
    [Tooltip("multiplier for thruster forces at minimum speed")]
    public float minForceSpeedRatio = 0.2f; //Min. ratio between max and min speed thrusting, based on ship speed
    [Tooltip("multiplier for thruster forces at maximum speed")]
    public float maxForceSpeedRatio = 1.0f; //Min. ratio between max and min speed thrusting, based on ship speed

    [SerializeField]
    bool _leftEngineOn;
    [SerializeField]
    bool _rightEngineOn;
    float _engineForce = 5f;
    float _engineDashForce = 50f;
    float _momentaryForceSpeedRatio = 0f;
    float _dashMaxTime = 0.2f;
    float _sideThrusterToMainThrusterRatio = 1;
    float _maxVelocity = 100f;
    float _maxDashVelocity = 50f;
    float _maxAngularVelocity = 10f;
    [SerializeField]
    bool _isDashing;
    Vector2 _spawn;
    
    float _dashTimeStart = 0.0f;
    float _momentaryDashForce = 0f;
    bool _leftDoubleTapped = false;
    bool _rightDoubleTapped = false;
    float _tapTimeLeft = 0.0f;
    float _tapTimeRight = 0.0f;
    float _previousTapTimeLeft = 0.0f;
    float _previousTapTimeRight = 0.0f;
    bool _leftButtonPressed = false;
    bool _rightButtonPressed = false;
    bool _doubleTapped = false;
    float _maxTurnRatioVelocity = 30.0f; //Speed of the ship where maximum turning speed is reached

    void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        _spawn = transform.position;

        _tapTimeLeft = Time.time;
        _tapTimeRight = Time.time;
        _previousTapTimeLeft = -10000f; //This value is arbitrary just to make sure that when game starts, double-tapping isn't possible with first, quick, single tap
        _previousTapTimeRight = -10000f; //This value is arbitrary just to make sure that when game starts, double-tapping isn't possible with first, quick, single tap

        _engineForce = shipVariables.engineForce;
        _engineDashForce = shipVariables.engineDashForce;
        _dashMaxTime = shipVariables.dashMaxTime;
        _sideThrusterToMainThrusterRatio = shipVariables.sideThrusterToMainThrusterRatio;
        _maxVelocity = shipVariables.maxVelocity;
        _maxDashVelocity = shipVariables.maxDashVelocity;
        _maxAngularVelocity = shipVariables.maxAngularVelocity;
        _isDashing = false;

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

        //Start of double tapping with L+R buttons
        if(_leftDoubleTapped && _rightDoubleTapped)
        {
            _doubleTapped = true;
            _leftDoubleTapped = false;
            _rightDoubleTapped = false;
        }

        //Check for dashing
        if(_doubleTapped && !_isDashing)
        {
            _isDashing = true;
            _dashTimeStart = Time.time;
            _momentaryDashForce = _engineDashForce;
            _maxVelocity = _maxDashVelocity;
            Debug.Log("Dashing starting time: " + Time.time);
        }

        //Check when to stop dashing (time-based or button release)
        if(_isDashing)
        {
            if(Time.time - _dashTimeStart >= _dashMaxTime || !_leftButtonPressed || !_rightButtonPressed)
            {
                _isDashing = false;
                _maxVelocity = shipVariables.maxVelocity;
                _doubleTapped = false;
                _momentaryDashForce = 0.0f;
                Debug.Log("Dash ending time: " + Time.time);
            }

        }

        //Setting the turning ratio based on ship's speed
        _momentaryForceSpeedRatio = RigidBody.velocity.magnitude / _maxTurnRatioVelocity;
        if(_momentaryForceSpeedRatio < minForceSpeedRatio) _momentaryForceSpeedRatio = minForceSpeedRatio;
        if(_momentaryForceSpeedRatio > maxForceSpeedRatio) _momentaryForceSpeedRatio = maxForceSpeedRatio;
  
        Debug.Log("Rigidbody.velocity.y: " + RigidBody.velocity.y);
        Debug.Log("Rigidbody.velocity.x: " + RigidBody.velocity.x);
        Debug.Log("Rigidbody.velocity.sqrmagnitude: " + RigidBody.velocity.sqrMagnitude);
    }

    public void ToggleLeftEngine(bool newState)
    {
        _leftEngineOn = newState;

        //Check for double-tap
        if(newState == true)
        {
            _leftButtonPressed = true;
            _tapTimeLeft = Time.time;

            if(_tapTimeLeft - _previousTapTimeLeft <= tapTimeWindow)
            {
                _leftDoubleTapped = true;
                Debug.Log("Left doubletapped!");
            }
            else
            {
                _leftDoubleTapped = false;
                _previousTapTimeLeft = _tapTimeLeft;
            }
        }
        else
        {
            _leftButtonPressed = false;
            _leftDoubleTapped = false;
        }
    }

    public void ToggleRightEngine(bool newState)
    {
        _rightEngineOn = newState;

        //Check for double-tap
        if(newState == true)
        {
            _rightButtonPressed = true;
            _tapTimeRight = Time.time;

            if(_tapTimeRight - _previousTapTimeRight <= tapTimeWindow)
            {
                _rightDoubleTapped = true;
                Debug.Log("Right doubletapped!");
            }
            else
            {
                _rightDoubleTapped = false;
                _previousTapTimeRight = _tapTimeRight;
            }
        }
        else
        {
            _rightButtonPressed = false;
            _rightDoubleTapped = false;
        }
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
                currentControlType = ControlType.BoostedThrustersSpeedScale;
                break;
            case ControlType.BoostedThrustersSpeedScale:
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
                        if(RigidBody.angularVelocity > -_maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * (_engineForce + _momentaryDashForce), leftEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * _engineForce, Color.red);
                        }
                    }

                    if(_rightEngineOn)
                    {
                        if(RigidBody.angularVelocity < _maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(rightEngine.up * (_engineForce + _momentaryDashForce), rightEngine.position);
                            Debug.DrawLine(rightEngine.position, rightEngine.position + rightEngine.up * _engineForce, Color.green);
                        }
                    }
                    break;
                case ControlType.BoostedThrusters:
                    if(_leftEngineOn && !_rightEngineOn)
                    {
                        if(RigidBody.angularVelocity > -_maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * _engineForce * _sideThrusterToMainThrusterRatio, leftEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * _engineForce * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, Color.red);
                        }
                    }
                    else if(_rightEngineOn && !_leftEngineOn)
                    {
                        if(RigidBody.angularVelocity < _maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(rightEngine.up * _engineForce * _sideThrusterToMainThrusterRatio, rightEngine.position);
                            Debug.DrawLine(rightEngine.position, rightEngine.position + rightEngine.up * _engineForce * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, Color.green);
                        }
                    }
                    else if(_leftEngineOn && _rightEngineOn)
                    {
                        if(RigidBody.velocity.sqrMagnitude < _maxVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * (_engineForce + _momentaryDashForce), leftEngine.position);
                            RigidBody.AddForceAtPosition(rightEngine.up * (_engineForce + _momentaryDashForce), rightEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * (_engineForce + _momentaryDashForce), Color.red);
                        }
                    }
                    break;
                case ControlType.BoostedThrustersSpeedScale:
                    if(_leftEngineOn && !_rightEngineOn)
                    {
                        if(RigidBody.angularVelocity > -_maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * _engineForce * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, leftEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * _engineForce * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, Color.red);
                        }
                    }
                    else if(_rightEngineOn && !_leftEngineOn)
                    {
                        if(RigidBody.angularVelocity < _maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(rightEngine.up * _engineForce * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, rightEngine.position);
                            Debug.DrawLine(rightEngine.position, rightEngine.position + rightEngine.up * _engineForce * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, Color.green);
                        }
                    }
                    else if(_leftEngineOn && _rightEngineOn)
                    {
                        if(RigidBody.velocity.sqrMagnitude < _maxVelocity || RigidBody.velocity.y < 0) // && Mathf.Pow(RigidBody.velocity.x, 2) < _maxVelocity)) this was trying to limit superfast flying on x-axis facing just a bit downwards. Needs further stydy/testing
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * (_engineForce + _momentaryDashForce), leftEngine.position);
                            RigidBody.AddForceAtPosition(rightEngine.up * (_engineForce + _momentaryDashForce), rightEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * (_engineForce + _momentaryDashForce), Color.red);
                        }
                    }
                    break;
                case ControlType.RigidBodyRotation:
                    if(_leftEngineOn && !_rightEngineOn)
                    {
                        RigidBody.angularVelocity = 0;
                        RigidBody.MoveRotation(RigidBody.rotation - _maxAngularVelocity * Time.deltaTime);
                    }

                    if(_rightEngineOn && !_leftEngineOn)
                    {
                        RigidBody.angularVelocity = 0;
                        RigidBody.MoveRotation(RigidBody.rotation + _maxAngularVelocity * Time.deltaTime);
                    }

                    if(_leftEngineOn && _rightEngineOn)
                    {
                        if(RigidBody.velocity.sqrMagnitude < _maxVelocity)
                        {
                            RigidBody.AddForceAtPosition(leftEngine.up * (_engineForce + _momentaryDashForce), leftEngine.position);
                            RigidBody.AddForceAtPosition(rightEngine.up * (_engineForce + _momentaryDashForce), rightEngine.position);
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * _engineForce, Color.red);
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