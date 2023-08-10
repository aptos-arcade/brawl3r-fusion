using System;
using UnityEngine;

namespace Player.Commands
{
    public class DashCommand : Command
    {

        private readonly PlayerController player;

        private float lastPressTime;

        private readonly float direction;

        public DashCommand(PlayerController player, KeyCode key, float direction) : base(key, direction > 0 
            ? InputButtons.RightDash : InputButtons.LeftDash)
        {
            this.player = player;
            this.direction = direction;
        }

        public override void WasPressed()
        {
            if (player.PlayerUtilities.IsDodging) return;
            if (player.PlayerUtilities.IsDashing && Math.Abs(player.transform.localScale.x - direction) > 0.01)
            {
                player.PlayerAnimations.OnAnimationDone("Dash");
            }
            var elapsedTime = Time.time - lastPressTime;
            lastPressTime = Time.time;
            if (elapsedTime > 0.2f) return;
            player.PlayerActions.TryDash();
        }
    }
}
