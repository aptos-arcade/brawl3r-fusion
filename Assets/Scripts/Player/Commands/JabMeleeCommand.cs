using Global;
using UnityEngine;

namespace Player.Commands
{
    public class JabMeleeCommand : Command
    {
        private readonly PlayerController player;
        
        public JabMeleeCommand(PlayerController player, KeyCode key) : base(key)
        {
            this.player = player;
        }

        public override void GetKeyDown()
        {
            if (player.PlayerProperties.IsGrounded)
            {
                player.PlayerAttacks.Melee(player.PlayerStats.JabMeleeAttack, Directions.Neutral);
            }
            else
            {
                player.PlayerAttacks.Melee(player.PlayerStats.DownMeleeAttack, Directions.Down);
            }
        }
    }
}
