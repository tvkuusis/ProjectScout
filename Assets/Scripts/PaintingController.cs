using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingController : MonoBehaviour
{

    public GameObject[] PaintingArray;

    //public GameObject Painting2;
    //public GameObject Painting3;
    //public GameObject Painting4;

    public static PaintingController Instance;

    private int _currentPainting;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _currentPainting = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetOpacity(float opacity)
    {
        Color tmp = PaintingArray[_currentPainting].GetComponent<SpriteRenderer>().color;
        tmp.a = opacity * 255;
        PaintingArray[_currentPainting].GetComponent<SpriteRenderer>().color = tmp;

        if(opacity <= 0)
        {
            _currentPainting++;
            if(_currentPainting > PaintingArray.Length) _currentPainting = PaintingArray.Length;
        }

    }

}
