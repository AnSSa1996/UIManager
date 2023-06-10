using UnityEngine;
using System;
using UnityEngine.Events;

namespace UIFramework
{
    public abstract class ATransitionComponent : MonoBehaviour
    {
        public abstract void Animate(Transform target, bool fadeIn, UnityAction callWhenFinished);
    }
}