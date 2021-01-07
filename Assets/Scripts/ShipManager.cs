using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipController))]
public class ShipManager : MonoBehaviour
{

    public ShipVariables shipVariables;
    private ShipController _shipController;

    float health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        health = shipVariables.startHealth;
        _shipController = GetComponent<ShipController>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        //E = 0.5mv2
        float damage = 0.01f * 0.5f * _shipController.RigidBody.mass * Mathf.Pow(_shipController.RigidBody.velocity.magnitude, 2);
        health -= damage;
        Debug.Log("damage: " + damage + ", health: " + health);
    }

}
