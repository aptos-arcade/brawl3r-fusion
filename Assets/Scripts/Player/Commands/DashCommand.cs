using System;
using Fusion;
using UnityEngine;

namespace Player.Commands
{
    public class DashCommand : Command
    {

        private readonly PlayerController player;

        private TickTimer dashTimer;

        private readonly float direction;

        public DashCommand(PlayerController player, KeyCode key, float direction) : base(key, direction > 0 
            ? InputButtons.RightDash : InputButtons.LeftDash)
        {
            this.player = player;
            this.direction = direction;
        }

        public override void WasPressed()
        {
            if (player.PlayerProperties.IsDodging) return;
            if (player.PlayerProperties.IsDashing && Math.Abs(player.transform.localScale.x - direction) > 0.01)
            {
                player.PlayerAnimations.OnAnimationDone(Animations.Animations.BodyDash, Animations.Animations.LegsDash);
            }
            
            if (!dashTimer.ExpiredOrNotRunning(player.Runner))
            {
                player.PlayerAnimations.TryDash();
                dashTimer = TickTimer.None;
            }
            else
            {
                dashTimer = TickTimer.CreateFromSeconds(player.Runner, 0.2f);
            }
        }
    }
}
