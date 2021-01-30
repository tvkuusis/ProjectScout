using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    public int collectablesPerCanves = 10;
    public GameObject collectablePrefab;
    public static CollectableManager Instance;
    [SerializeField]
    List<Collectable> _currentCollectables = new List<Collectable>();
    public Transform minPosition;
    public Transform maxPosition;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SpawnNewCollectables();
    }

    public void SpawnNewCollectables()
    {
        for(int i = 0; i < collectablesPerCanves; ++i)
        {
            Collectable c = Instantiate(collectablePrefab, new Vector3(Random.Range(minPosition.position.x, maxPosition.position.x), Random.Range(minPosition.position.y, maxPosition.position.y), 1), Quaternion.identity).GetComponent<Collectable>();
            _currentCollectables.Add(c);
        }
    }

    public void OnCollectableGained(Collectable c)
    {
        if(_currentCollectables.Contains(c))
        {
            _currentCollectables.Remove(c);
            Destroy(c.gameObject);
            Debug.Log("Collectable gained. Left: " + _currentCollectables.Count + " / " + collectablesPerCanves);
        }

        float opacity = (float)_currentCollectables.Count / (float)collectablesPerCanves;
        PaintingController.Instance.SetOpacity(opacity);

        if(_currentCollectables.Count == 0)
        {
            SpawnNewCollectables();
        }
    }
}
