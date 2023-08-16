using UnityEngine;

namespace Player.Commands
{
    public class ShootCommand : Command
    {

        private readonly PlayerController player;
        
        public ShootCommand(PlayerController player, KeyCode key) : base(key, InputButtons.Shoot)
        {
            this.player = player;
        }

        public override void WasPressed()
        {
            player.PlayerAttacks.Shoot();
        }
    }
}
