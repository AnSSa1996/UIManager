using DG.Tweening;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Sequence = DG.Tweening.Sequence;

namespace UIFramework
{
    public class SimpleTransition : ATransitionComponent
    {
        private float _fadeDuration = 0.5f;
        private UnityAction _currentAction;
        private Transform _currentTarget;
        private float _startValue;
        private float _endValue;
        private Sequence _sequence = null;

        public override void Animate(Transform target, bool fadeIn, UnityAction callWhenFinished)
        {
            _currentAction = null;
            if (fadeIn) FadeInAnimation();
            else FadeOutAnimation();
            _currentAction = callWhenFinished;
        }

        private void FadeInAnimation()
        {
            if (UnityObjectUtility.IsUnityNull(_sequence) == false) _sequence.Kill(true);
            transform.localScale = Vector3.zero;
            _sequence = DOTween.Sequence();
            _sequence.SetUpdate(true);
            _sequence.Append(transform.DOScale(Vector3.one, _fadeDuration).SetEase(Ease.OutBack));
            _sequence.onComplete += () => { _currentAction?.Invoke(); };
            _sequence.Play();
        }

        private void FadeOutAnimation()
        {
            if (UnityObjectUtility.IsUnityNull(_sequence) == false) _sequence.Kill(true);
            transform.localScale = Vector3.one;
            _sequence.SetUpdate(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOScale(Vector3.zero, _fadeDuration).SetEase(Ease.InBack));
            _sequence.onComplete += () => { _currentAction?.Invoke(); };
            _sequence.Play();
        }
    }
}