using Framework.AI.Systems;
using Framework.StateMachine;
using UnityEngine;

namespace Gameplay.AI.Group
{
    public class AIGroupSystem : IAISystem
    {
        public AISystemCategory Category => AISystemCategory.Squad;

        private readonly AIGroupManager _manager;

        public AIGroupSystem(AIGroupManager manager)
        {
            _manager = manager;
        }

        public void Update(StateContext context)
        {
            if (_manager == null)
                return;

            _manager.SharedAlertLevel = Mathf.Lerp(
                _manager.SharedAlertLevel,
                0f,
                Time.deltaTime * 0.5f
            );
        }
    }
}