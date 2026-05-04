using Framework.StateMachine;
using UnityEngine;

namespace Framework.Animation
{
    public class AnimationSystem : MonoBehaviour
    {
        public AnimationController controller;
        public StateContext context;

        private void LateUpdate()
        {
            if (context.AnimationRequest.HasValue)
            {
                controller.Play(context.AnimationRequest.Value);
                
                context.AnimationRequest = null;
            }
        }
    }
}