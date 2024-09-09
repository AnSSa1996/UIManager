using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class UIToggleSwitch : Button
{
    private bool _isOn = false;
    [SerializeField] public Color32 EnabledColor = new Color32(0, 255, 0, 255);
    [SerializeField] public Color32 DisabledColor = new Color32(125, 125, 125, 255);
    [SerializeField] public float EnabledPosX = 70f;
    [SerializeField] public float DisabledPosX = -70f;
    
    [SerializeField] public RectTransform Handle = null;
    [SerializeField] public Image BackgroundImage = null;

    private Sequence _enabledTweenSequence = null;
    private Sequence _disabledTweenSequence = null;

    public bool isOn
    {
        get => _isOn;
        set
        {
            if (_isOn == value) return;
            _isOn = value;
            PlayAnimationToggleSwitch();
        }
    }
    
    public void InitSet(bool ison)
    {
        _isOn = ison;
        if (_isOn) SetEnable();
        else SetDisable();
    }

    public override void OnPointerClick(PointerEventData eventData)
    { 
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        InternalToggle();
        
        base.OnPointerClick(eventData);
    }
    
    private void InternalToggle()
    {
        if (!IsActive() || !IsInteractable())
            return;

        isOn = !isOn;
    }

    private void PlayAnimationToggleSwitch()
    {
        KillAllTween();
        
        if (isOn) PlayEnableAnim();
        else PlayDisableAnim();
    }

    private void PlayEnableAnim()
    {
        SetDisable();
        _enabledTweenSequence = DOTween.Sequence();
        _enabledTweenSequence.Join(Handle.DOAnchorPosX(EnabledPosX, 0.5f));
        _enabledTweenSequence.Join(BackgroundImage.DOColor(EnabledColor, 0.5f));
    }

    private void PlayDisableAnim()
    {
        SetEnable();
        _disabledTweenSequence = DOTween.Sequence();
        _disabledTweenSequence.Join(Handle.DOAnchorPosX(DisabledPosX, 0.5f));
        _disabledTweenSequence.Join(BackgroundImage.DOColor(DisabledColor, 0.5f));
    }

    private void SetEnable()
    {
        Handle.anchoredPosition = new Vector2(EnabledPosX, 0f);
        BackgroundImage.color = EnabledColor;
    }

    private void SetDisable()
    {
        Handle.anchoredPosition = new Vector2(DisabledPosX, 0f);
        BackgroundImage.color = DisabledColor;
    }

    private void KillAllTween()
    {
        if (_enabledTweenSequence.IsUnityNull() == false) _enabledTweenSequence.Kill(true);
        if (_disabledTweenSequence.IsUnityNull() == false) _disabledTweenSequence.Kill(true);
    }
}
