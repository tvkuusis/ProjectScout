using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShipController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public enum ControlType { Original, BoostedThrusters, BoostedThrustersSpeedScale, RigidBodyRotation }
    public ControlType currentControlType;
    public Transform leftEngine;
    public Transform rightEngine;
    public Rigidbody2D RigidBody { get; private set; }
    public Text controlText; // Debug text
    public ShipVariables shipVariables;
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
    Vector2 _touchPosLeft;
    Vector2 _touchPosRight;
    
    float _dashTimeStart = 0.0f;
    float _momentaryDashForce = 0f;
    bool _leftButtonDoubleTapped = false;
    bool _rightButtonDoubleTapped = false;
    float _tapTimeLeft = 0.0f;
    float _tapTimeRight = 0.0f;
    float _previousTapTimeLeft = 0.0f;
    float _previousTapTimeRight = 0.0f;
    bool _leftButtonPressed = false;
    bool _rightButtonPressed = false;
    bool _leftRightDoubleTapped = false;
    float _maxTurnRatioVelocity = 30.0f; //Speed of the ship where maximum turning speed is reached
    float _momentaryDashForceLeft = 0f;
    float _momentaryDashForceRight = 0f;
    bool _isDashingLeft = false;
    bool _isDashingRight = false;

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

        //Reckognize double tapping with both L and R buttons
        if(_leftButtonDoubleTapped && _rightButtonDoubleTapped)
        {
            _leftRightDoubleTapped = true;
            _leftButtonDoubleTapped = false;
            _rightButtonDoubleTapped = false;
        }

        //Double-thruster dash
        if(_leftRightDoubleTapped && !_isDashing)
        {
            _isDashing = true;
            _dashTimeStart = Time.time;
            _momentaryDashForce = _engineDashForce;
            _maxVelocity = _maxDashVelocity;
            Debug.Log("Dash start time: " + Time.time);
        }

        //Check when to stop dashing (time-based or button release)
        if(_isDashing)
        {
            if(Time.time - _dashTimeStart >= _dashMaxTime || !_leftButtonPressed || !_rightButtonPressed)
            {
                _isDashing = false;
                _maxVelocity = shipVariables.maxVelocity;
                _leftRightDoubleTapped = false;
                _momentaryDashForce = 0.0f;
                Debug.Log("Dash ending time: " + Time.time);
            }
        }

        //Single-thruster dashing, left
        if(_leftButtonDoubleTapped && !_rightButtonDoubleTapped && !_isDashingLeft && !_isDashing)
        {
            _isDashingLeft = true;
            _isDashingRight = false;
            _isDashing = false;
            _dashTimeStart = Time.time;
            _momentaryDashForceLeft = _engineDashForce * _sideThrusterToMainThrusterRatio;
            Debug.Log("Dashing left starting time: " + Time.time);
        }

        if(_isDashingLeft)
        {
            if(Time.time - _dashTimeStart >= _dashMaxTime || !_leftButtonPressed)
            {
                _isDashingLeft = false;
                _leftButtonDoubleTapped = false;
                _momentaryDashForceLeft = 0.0f;
                Debug.Log("Dashing left ending time: " + Time.time);
            }
        }

        //Single-thruster dashing, right
        if(!_leftButtonDoubleTapped && _rightButtonDoubleTapped && !_isDashingRight && !_isDashing)
        {
            _isDashingRight = true;
            _isDashingLeft = false;
            _isDashing = false;
            _dashTimeStart = Time.time;
            _momentaryDashForceRight = _engineDashForce * _sideThrusterToMainThrusterRatio;
            Debug.Log("Dashing right starting time: " + Time.time);
        }

        if(_isDashingRight)
        {
            if(Time.time - _dashTimeStart >= _dashMaxTime || !_rightButtonPressed)
            {
                _isDashingRight = false;
                _rightButtonDoubleTapped = false;
                _momentaryDashForceRight = 0.0f;
                Debug.Log("Dashing right ending time: " + Time.time);
            }
        }
    }

    public void ToggleLeftEngine(bool newState)
    {
        _leftEngineOn = newState;

        //Left button is pressed
        if(newState == true)
        {
            //NOTE: remove the if condition if speed-based turning is wanted to affect to all control types - Lauri
            if(currentControlType == ControlType.BoostedThrustersSpeedScale) UpdateTurningSpeed();

            _leftButtonPressed = true;
            _tapTimeLeft = Time.time;

            //Check for double - tap
            if(_tapTimeLeft - _previousTapTimeLeft <= tapTimeWindow)
            {
                _leftButtonDoubleTapped = true;
                Debug.Log("Left doubletapped!");
            }
            else
            {
                _leftButtonDoubleTapped = false;
                _previousTapTimeLeft = _tapTimeLeft;
            }
        }
        else
        {
            _leftButtonPressed = false;
            _leftButtonDoubleTapped = false;
        }
    }

    public void ToggleRightEngine(bool newState)
    {
        _rightEngineOn = newState;

        //Check for double-tap
        if(newState == true)
        {
            //NOTE: remove the if condition if speed-based turning is wanted to affect to all control types - Lauri
            if(currentControlType == ControlType.BoostedThrustersSpeedScale) UpdateTurningSpeed();

            _rightButtonPressed = true;
            _tapTimeRight = Time.time;

            if(_tapTimeRight - _previousTapTimeRight <= tapTimeWindow)
            {
                _rightButtonDoubleTapped = true;
                Debug.Log("Right doubletapped!");
            }
            else
            {
                _rightButtonDoubleTapped = false;
                _previousTapTimeRight = _tapTimeRight;
            }
        }
        else
        {
            _rightButtonPressed = false;
            _rightButtonDoubleTapped = false;
        }
    }

   

    public void OnBeginDrag(PointerEventData eventData)
    {

        _touchPosLeft = eventData.position;

        Debug.Log("beginning drag at: " + _touchPosLeft);

    }

    public void OnDrag(PointerEventData eventData)
    {

        _touchPosLeft = eventData.position;

        Debug.Log("beginning drag at: " + _touchPosLeft);

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        _touchPosLeft = eventData.position;

        Debug.Log("beginning drag at: " + _touchPosLeft);

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
                            RigidBody.AddForceAtPosition(leftEngine.up * (_engineForce + _momentaryDashForceLeft) * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, leftEngine.position);                     
                            Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * _engineForce * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, Color.red);
                        }
                    }
                    else if(_rightEngineOn && !_leftEngineOn)
                    {
                        if(RigidBody.angularVelocity < _maxAngularVelocity)
                        {
                            RigidBody.AddForceAtPosition(rightEngine.up * (_engineForce + _momentaryDashForceRight) * _sideThrusterToMainThrusterRatio * _momentaryForceSpeedRatio, rightEngine.position);
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

    private void UpdateTurningSpeed()
    {
        //Update the thruster's turning speed based on ship's speed
        _momentaryForceSpeedRatio = RigidBody.velocity.magnitude / _maxTurnRatioVelocity;
        if(_momentaryForceSpeedRatio < minForceSpeedRatio) _momentaryForceSpeedRatio = minForceSpeedRatio;
        if(_momentaryForceSpeedRatio > maxForceSpeedRatio) _momentaryForceSpeedRatio = maxForceSpeedRatio;
    }

    private void OnDrawGizmos()
    {
        if(RigidBody)
        {
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)RigidBody.velocity);
        }
    }
}