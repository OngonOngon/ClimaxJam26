using UnityEngine;
using Pospec.Common;

namespace Pospec.Movement.Examples.SideScrolling
{
    public class WalkingSideScrollState : MonoBehaviour, IState
    {
        [Tooltip("Horizontal speed")]
        [SerializeField] private float moveSpeed = 5f;
        [Tooltip("Rate of change for move speed")]
        [SerializeField] private float acceleration = 10f;
        
        private SideScrollMovement movement;
        private StateMachine<IState> machine;
        private ISideScrollController controller;

        public void Initialize(SideScrollMovement movement, StateMachine<IState> machine, ISideScrollController controller)
        {
            this.movement = movement;
        }

        public void Enter()
        {
            throw new System.NotImplementedException();
        }

        public void Execute()
        {
            //if (!controller.IsGrounded())
            //    machine.TransitionTo(movement.FallState);
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}
