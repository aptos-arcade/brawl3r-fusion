using Gameplay;
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
            if (player.PlayerState.MeleeEnergy >= player.PlayerStats.UpMeleeAttack.Energy)
            {
                if (player.PlayerComponents.Animator.CurrentAnimationBody == "Body_Attack") return;
                player.PlayerReferences.Sword.strikerData = player.PlayerStats.UpMeleeAttack;
                
                player.PlayerComponents.Animator.SetAttackDirection(Directions.Up);
                
                player.PlayerActions.TrySwapWeapon(Global.Weapons.Sword);
                player.PlayerActions.Attack();
            }
            else
            {
                GameManager.Instance.NoEnergy(EnergyManager.EnergyType.Sword);
            }
        }
    }
}
