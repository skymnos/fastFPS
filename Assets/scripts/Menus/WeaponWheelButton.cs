using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelButton : MonoBehaviour
{
    private GameObject slot;
    [SerializeField] private Image secondaryBGImage;

    public GunSO gun;

    private Color initalSecondaryBGColor;
    private Vector3 initialPos;

    private void Start()
    {
        slot = transform.parent.gameObject;
        initialPos = transform.localPosition;
        initalSecondaryBGColor = secondaryBGImage.color;
    }

    public void Selected()
    {
        slot.transform.localPosition += slot.transform.up * 10;
        secondaryBGImage.color = Color.red;
    }

    public void Unselected()
    {
        slot.transform.localPosition = initialPos;
        secondaryBGImage.color = initalSecondaryBGColor;
    }
}
