using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    Animator _ac;

    void Start()
    {
        _ac = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.LogError(gameObject.name + " collided with " + other.gameObject.name);
        _ac.SetTrigger("Collect");
    }

    public void OnCollect()
    {
        gameObject.SetActive(false);
    }
}
