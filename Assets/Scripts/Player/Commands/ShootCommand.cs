using Gameplay;
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
            if (player.PlayerState.RangedEnergy >= player.PlayerStats.RangedAttack.Energy)
            {
                if(player.PlayerAnimations.IsCurrentBodyAnimation("Attack")) return;
                player.PlayerActions.TrySwapWeapon(Global.Weapons.Gun);
                player.PlayerActions.Attack();
            }
            else
            {
                GameManager.Instance.NoEnergy(EnergyManager.EnergyType.Gun);
            }
            
        }
    }
}
