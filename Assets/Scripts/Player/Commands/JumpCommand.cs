using UnityEngine;

namespace Player.Commands
{
    public class JumpCommand : Command
    {

        private readonly PlayerController player;

        public JumpCommand(PlayerController player, KeyCode key) : base(key)
        {
            this.player = player;
        }

        public override void GetKeyDown()
        {
            if (player.PlayerProperties.IsGrounded)
            {
                player.PlayerAnimations.TryJump();
            }
            else if(player.PlayerNetworkState.CanDoubleJump)
            {
                player.PlayerAnimations.TryDoubleJump();
            }
        }
    }
}
