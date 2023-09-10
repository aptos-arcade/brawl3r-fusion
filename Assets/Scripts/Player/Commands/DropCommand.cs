using UnityEngine;

namespace Player.Commands
{
    public class DropCommand : Command
    {

        private readonly PlayerController player;

        public DropCommand(PlayerController player, KeyCode key) : base(key)
        {
            this.player = player;
        }

        public override void GetKeyDown()
        {
            if (player.PlayerProperties.IsGrounded)
            {
                if (player.PlayerProperties.IsOnPlatform && !player.PlayerProperties.IsOnGround)
                {
                    player.PlayerMovementController.Drop();
                }
            }
            else
            {
                player.PlayerAnimations.TryFastFall();
            }
            
            
        }
    }
}
