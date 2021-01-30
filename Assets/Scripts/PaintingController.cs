using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingController : MonoBehaviour
{

    public GameObject[] PaintingArray;

    //public GameObject Painting2;
    //public GameObject Painting3;
    //public GameObject Painting4;

    private int _currentPainting;

    // Start is called before the first frame update
    void Start()
    {
        _currentPainting = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetOpacity(float opacity)
    {
        Color tmp = PaintingArray[_currentPainting].GetComponent<SpriteRenderer>().color;
        tmp.a = opacity;
        PaintingArray[_currentPainting].GetComponent<SpriteRenderer>().color = tmp;

    }

}
