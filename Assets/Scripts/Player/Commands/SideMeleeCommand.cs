using Global;
using UnityEngine;

namespace Player.Commands
{
    public class SideMeleeCommand : Command
    {
        private readonly PlayerController player;

        private readonly int xScale;
        
        public SideMeleeCommand(PlayerController player, KeyCode key, int xScale) : base(key, xScale == 1 
            ? InputButtons.RightSideMelee : InputButtons.LeftSideMelee)
        {
            this.player = player;
            this.xScale = xScale;
        }

        public override void WasPressed()
        {
            if (player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyAttack)) return;
            player.gameObject.transform.localScale = new Vector3(xScale, 1, 1);
            player.PlayerReferences.PlayerCanvas.transform.localScale = new Vector3(xScale, 1, 1);
            player.PlayerAttacks.Melee(player.PlayerStats.SideMeleeAttack, Directions.Side);
        }
    }
}
