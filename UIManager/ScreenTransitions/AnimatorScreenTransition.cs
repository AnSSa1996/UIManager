using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace UIFramework.Examples
{
    public class AnimatorScreenTransition : ATransitionComponent
    {
        [SerializeField] private Animator animator = null;

        private UnityAction _previousCallbackWhenFinished;
        private bool _fadeIn = false;
        
        public override void Animate(Transform target, bool fadeIn, UnityAction action) {
            FinishPrevious();
            
            _fadeIn = fadeIn;
            StartCoroutine(PlayAnimationRoutine(action));
        }

        private IEnumerator PlayAnimationRoutine(UnityAction callWhenFinished) {
            _previousCallbackWhenFinished = callWhenFinished;

            if (_fadeIn) animator.SetTrigger("FadeIn");
            else animator.SetTrigger("FadeOut");
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
            FinishPrevious();
        }
        
        private void FinishPrevious() {
            if (_previousCallbackWhenFinished != null) {
                _previousCallbackWhenFinished();
                _previousCallbackWhenFinished = null;
            }
            
            StopAllCoroutines();
        }
    }
}
