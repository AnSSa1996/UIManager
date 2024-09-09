using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class UIToggleCheck : Toggle
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        InternalToggle();
    }

    private void InternalToggle()
    {
        if (!IsActive() || !IsInteractable())
            return;
        isOn = !isOn;
    }
}