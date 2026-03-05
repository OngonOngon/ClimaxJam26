namespace Pospec.Movement.Examples.SideScrolling
{
    public interface ISideScrollController
    {
        float GetHorizontalMovement();
        bool IsJumping();
        bool JumpStarted();
        bool IsGrounded();
    }
}
