using Framework.Commands;
using UnityEngine;

namespace Gameplay.Systems.Movement.Commands
{
    public class MoveCommand : ICommand
    {
        private readonly Transform _transform;
        private readonly Vector3 _direction;
        private readonly float _speed;

        public MoveCommand(Transform transform, Vector3 direction, float speed)
        {
            _transform = transform;
            _direction = direction;
            _speed = speed;
        }

        public void Execute()
        {
            _transform.position += _direction * _speed * Time.deltaTime;
        }
    }
}