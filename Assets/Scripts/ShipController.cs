using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float engineForce = 5f;
    public float maxVelocity = 10f;
    public float maxAngularVelocity = 10f;
    public Transform leftEngine;
    public Transform rightEngine;

    [SerializeField]
    bool _leftEngineOn;
    [SerializeField] 
    bool _rightEngineOn;

    Rigidbody2D _rb;
    Vector2 _spawn;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spawn = transform.position;

        StartCoroutine(EngineRoutine());
    }

    void Update()
    {    
        if(Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

        Debug.Log(_rb.velocity.sqrMagnitude + " " + _rb.angularVelocity);
    }

    public void ToggleLeftEngine(bool newState)
    {
        Debug.LogError("Left engine " + newState);
        _leftEngineOn = newState;
    }

    public void ToggleRightEngine(bool newState)
    {
        Debug.LogError("Right engine " + newState);
        _rightEngineOn = newState;
    }

    public void Reset()
    {
        transform.position = _spawn;
        transform.rotation = Quaternion.identity;
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0;
    }

    IEnumerator EngineRoutine()
    {
        while(true)
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                ToggleLeftEngine(true);
            }
            else if(Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                ToggleLeftEngine(false);
            }

            if(_leftEngineOn)
            {
                if(_rb.angularVelocity > -maxAngularVelocity /*&& _rb.velocity.sqrMagnitude < maxVelocity*/)
                {
                    _rb.AddForceAtPosition(leftEngine.up * engineForce, leftEngine.position);
                    Debug.DrawLine(leftEngine.position, leftEngine.position + leftEngine.up * engineForce, Color.red);
                }
            }
     

            if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                ToggleRightEngine(true);
            }
            else if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                ToggleRightEngine(false);
            }

            if(_rightEngineOn)
            {
                if(_rb.angularVelocity < maxAngularVelocity /*&& _rb.velocity.sqrMagnitude < maxVelocity*/)
                {
                    _rb.AddForceAtPosition(rightEngine.up * engineForce, rightEngine.position);
                    Debug.DrawLine(rightEngine.position, rightEngine.position + rightEngine.up * engineForce, Color.green);
                }
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_rb.velocity);
    }
}