using UnityEngine;
using Pospec.Common;

namespace Pospec.Movement.Examples.SideScrolling
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SideScrollMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private ISideScrollController controller;
        private StateMachine<IState> machine = new StateMachine<IState>();
        [SerializeField] private IdleSideScrollState idleState;
        [SerializeField] private WalkingSideScrollState walkState;
        [SerializeField] private JumpSideScrollState jumpState;
        [SerializeField] private FallingSideScrollState fallState;

        private void Start()
        {
            walkState.Initialize(this, machine, controller);
        }
    }
}
