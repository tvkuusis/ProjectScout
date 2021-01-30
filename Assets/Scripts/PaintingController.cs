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
        Debug.Log("SetOpacity to: " + opacity);

        StartCoroutine(LerpOpacityCoroutine(opacity));

    }

    private IEnumerator LerpOpacityCoroutine(float opacity)
    {

        Color tmp = PaintingArray[_currentPainting].GetComponent<SpriteRenderer>().color;
        //tmp.a = opacity;
        //PaintingArray[_currentPainting].GetComponent<SpriteRenderer>().color = tmp;

        float timeNow = Time.time;

        while(Time.time - timeNow <= 1f)
        {
            tmp.a = Mathf.Lerp(tmp.a, opacity, Time.time - timeNow);
            PaintingArray[_currentPainting].GetComponent<SpriteRenderer>().color = tmp;
            yield return null;

        }

        if(opacity <= 0)
        {
            _currentPainting++;
            if(_currentPainting > PaintingArray.Length) _currentPainting = PaintingArray.Length;
        }
    }


}
