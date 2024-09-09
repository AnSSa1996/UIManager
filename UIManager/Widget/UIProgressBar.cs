using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;

    [SerializeField]
    private float speed = 1.0f;
    public void SetValue(float percent)
    {
        slider.value = percent;
        text.text = $"{percent * 100:00}%";
    }
}
