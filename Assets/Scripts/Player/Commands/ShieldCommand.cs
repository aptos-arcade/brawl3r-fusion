using UnityEngine;

namespace Player.Commands
{
    public class ShieldCommand : Command
    {
        private readonly PlayerController player;
        
        public ShieldCommand(PlayerController player, KeyCode key) : base(key, InputButtons.Shield)
        {
            this.player = player;
        }

        public override void WasHeld()
        {
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                if (player.PlayerState.ShieldEnergy < 0.1 || !player.PlayerUtilities.IsGrounded) return;
                player.PlayerActions.TryShield();
            }
            else
            {
                if (player.PlayerUtilities.IsDodging || !player.PlayerState.CanDodge) return;
                player.PlayerState.Direction = Input.GetAxisRaw("Horizontal") * Vector2.right;
                player.PlayerActions.TryDodge();
            }
            
        }
        
        public override void WasReleased()
        {
            if (!player.PlayerAnimations.IsCurrentBodyAnimation("Shield")) return;
            player.PlayerAnimations.OnAnimationDone("Shield");
        }
    }
}
