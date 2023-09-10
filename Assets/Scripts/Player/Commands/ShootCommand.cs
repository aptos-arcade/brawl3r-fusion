using UnityEngine;

namespace Player.Commands
{
    public class ShootCommand : Command
    {

        private readonly PlayerController player;
        
        public ShootCommand(PlayerController player, KeyCode key) : base(key)
        {
            this.player = player;
        }

        public override void GetKeyDown()
        {
            player.PlayerAttacks.Shoot();
        }
    }
}
