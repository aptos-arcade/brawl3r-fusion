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
                if (player.PlayerNetworkState.ShieldEnergy < 0.1 || !player.PlayerProperties.IsGrounded) return;
                player.PlayerAnimations.TryShield();
            }
            else
            {
                if (player.PlayerProperties.IsDodging || !player.PlayerProperties.CanDodge) return;
                player.PlayerNetworkState.Direction = Input.GetAxisRaw("Horizontal") * Vector2.right;
                player.PlayerAnimations.TryDodge();
            }
            
        }
        
        public override void WasReleased()
        {
            if (!player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyShield)) return;
            player.PlayerAnimations.OnAnimationDone(Animations.Animations.BodyShield, Animations.Animations.LegsShield);
        }
    }
}
