using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyButtonBehaviour : EventTrigger
{
    Vector3 originalSize;
    Button b;

    private void Start()
    {
        originalSize = transform.localScale;

        b = GetComponent<Button>();

        b.onClick.AddListener(ClickSound);
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        transform.localScale *= 1.2f;
        if (GameManager.Instance.soundOn)
            AkSoundEngine.PostEvent("Menu_Shade_Button", gameObject);
    }

    public override void OnPointerExit(PointerEventData data)
    {
        transform.localScale /= 1.2f;
    }

    private void OnDisable()
    {
        transform.localScale = originalSize;
    }

    private void ClickSound()
    {
        if (GameManager.Instance.soundOn)
            AkSoundEngine.PostEvent("Menu_Press_button", gameObject);
    }
}
