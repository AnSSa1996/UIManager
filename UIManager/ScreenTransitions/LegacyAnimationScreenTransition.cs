using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace UIFramework.Examples
{
    public class LegacyAnimationScreenTransition : ATransitionComponent
    {
        [SerializeField] private AnimationClip clip = null;

        private UnityAction _previousCallbackWhenFinished;
        private bool _fadeIn = false;
        
        public override void Animate(Transform target, bool fadeIn, UnityAction action) {
            FinishPrevious();
            var targetAnimation = target.GetOrAddComponent<Animation>();
            if (targetAnimation == null) {
                Debug.LogError("[LegacyAnimationScreenTransition] No Animation component in " + target);
                if (action != null) {
                    action();
                }

                return;
            }

            _fadeIn = fadeIn;
            targetAnimation.clip = clip;
            StartCoroutine(PlayAnimationRoutine(targetAnimation, action));
        }

        private IEnumerator PlayAnimationRoutine(Animation targetAnimation, UnityAction callWhenFinished) {
            _previousCallbackWhenFinished = callWhenFinished;
            foreach (AnimationState state in targetAnimation) {
                state.time = _fadeIn ? 0f : state.clip.length;
                state.speed = _fadeIn ? 1f : -1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
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
