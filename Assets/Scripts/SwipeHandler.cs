using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float swipeMinDistance = 40f;
    public UnityEvent onSwipeDown;
    public UnityEvent onSwipeUp;

    Vector2 _touchPosStart;
    Vector2 _touchPosEnd;

    void Start()
    {
        if(onSwipeDown == null)
            onSwipeDown = new UnityEvent();

        if(onSwipeUp == null)
            onSwipeUp = new UnityEvent();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        _touchPosStart = eventData.position;

        Debug.Log("beginning swipe at: " + _touchPosStart);

    }

    public void OnDrag(PointerEventData eventData)
    {
/*
        _touchPos = eventData.position;

        Debug.Log("ondrag at: " + _touchPos);
*/
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        _touchPosEnd = eventData.position;

        Debug.Log("end swipe at: " + _touchPosEnd);

        if(_touchPosEnd.y > _touchPosStart.y && Vector2.Distance(_touchPosStart, _touchPosEnd) > swipeMinDistance)
        {
            onSwipeUp.Invoke();
        }

        else if(_touchPosEnd.y < _touchPosStart.y && Vector2.Distance(_touchPosStart, _touchPosEnd) > swipeMinDistance)
        {
            onSwipeDown.Invoke();
        }
    }
}
