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

    private void OnTriggerEnter2D(Collider2D other)
    {
        _ac.SetTrigger("Collect");
    }

    public void OnCollect()
    {
        CollectableManager.Instance.OnCollectableGained(this);
        gameObject.SetActive(false);
    }
}
