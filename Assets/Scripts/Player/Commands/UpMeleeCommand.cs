using Global;
using UnityEngine;

namespace Player.Commands
{
    public class UpMeleeCommand : Command
    {
        private readonly PlayerController player;
        

        public UpMeleeCommand(PlayerController player, KeyCode key) : base(key, InputButtons.UpMelee)
        {
            this.player = player;
        }

        public override void WasPressed()
        {
            player.PlayerAttacks.Melee(player.PlayerStats.UpMeleeAttack, Directions.Up);
        }
    }
}
