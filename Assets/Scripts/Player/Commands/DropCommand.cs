using UnityEngine;

namespace Player.Commands
{
    public class DropCommand : Command
    {

        private readonly PlayerController player;

        public DropCommand(PlayerController player, KeyCode key) : base(key, InputButtons.Drop)
        {
            this.player = player;
        }

        public override void WasPressed()
        {
            if (player.PlayerUtilities.IsGrounded)
            {
                if (player.PlayerUtilities.IsOnPlatform && !player.PlayerUtilities.IsOnGround)
                {
                    player.PlayerActions.Drop();
                }
            }
            else
            {
                player.PlayerComponents.Animator.TryPlayAnimation("Body_FastFall");
                player.PlayerComponents.Animator.TryPlayAnimation("Legs_FastFall");
            }
            
            
        }
    }
}
