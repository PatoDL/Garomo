using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyButtonBehaviour : EventTrigger
{
    Vector3 originalSize;

    private void Start()
    {
        originalSize = transform.localScale;
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        transform.localScale *= 1.2f;
        Debug.Log("entra");
    }

    public override void OnPointerExit(PointerEventData data)
    {
        transform.localScale /= 1.2f;
        Debug.Log("sale");
    }

    private void OnDisable()
    {
        transform.localScale = originalSize;
    }
}
