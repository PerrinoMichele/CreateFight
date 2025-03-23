using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    public GameObject replacement;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
        Transform replacementTransform = transform.Find("Back");
        replacement = replacementTransform.gameObject;
        replacement.SetActive(true);

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        replacement.SetActive(false);
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        replacement.SetActive(true);
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}