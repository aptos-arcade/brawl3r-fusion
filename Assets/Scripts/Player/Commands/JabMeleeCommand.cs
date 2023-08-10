using Gameplay;
using Global;
using UnityEngine;

namespace Player.Commands
{
    public class JabMeleeCommand : Command
    {
        private readonly PlayerController player;
        
        public JabMeleeCommand(PlayerController player, KeyCode key) : base(key, InputButtons.JabMelee)
        {
            this.player = player;
        }

        public override void WasPressed()
        {
            if (player.PlayerAnimations.IsCurrentBodyAnimation("Attack")) return;
            if (player.PlayerUtilities.IsGrounded)
            {
                if (player.PlayerState.MeleeEnergy >= player.PlayerStats.JabMeleeAttack.Energy)
                {
                    player.PlayerReferences.Sword.strikerData = player.PlayerStats.JabMeleeAttack;
                    player.PlayerAnimations.SetAttackDirection(Directions.Neutral);
                }
                else
                {
                    GameManager.Instance.NoEnergy(EnergyManager.EnergyType.Sword);
                    return;
                }
            }
            else
            {
                if (player.PlayerState.MeleeEnergy >= player.PlayerStats.DownMeleeAttack.Energy)
                {
                    player.PlayerReferences.Sword.strikerData = player.PlayerStats.DownMeleeAttack;
                    player.PlayerAnimations.SetAttackDirection(Directions.Down);
                }
                else
                {
                    GameManager.Instance.NoEnergy(EnergyManager.EnergyType.Sword);
                    return;
                }
            }
            player.PlayerActions.TrySwapWeapon(Global.Weapons.Sword);
            player.PlayerActions.Attack();
        }
    }
}
