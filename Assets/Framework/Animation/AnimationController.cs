using UnityEngine;

namespace Framework.Animation
{
    public class AnimationController : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Play(AnimationRequest request)
        {
            switch (request.Type)
            {
                case AnimationType.Idle:
                    _animator.SetFloat("Speed", 0f);
                    _animator.ResetTrigger("Attack");
                    break;

                case AnimationType.Move:
                    _animator.SetFloat("Speed", 1f);
                    break;

                case AnimationType.Attack:
                    _animator.SetTrigger("Attack");
                    break;
            }
        }
    }
}