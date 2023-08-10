using Gameplay;
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
            if (player.PlayerState.MeleeEnergy >= player.PlayerStats.SideMeleeAttack.Energy)
            {
                if (player.PlayerComponents.Animator.CurrentAnimationBody == "Body_Attack") return;
                player.PlayerReferences.Sword.strikerData = player.PlayerStats.SideMeleeAttack;

                player.gameObject.transform.localScale = new Vector3(xScale, 1, 1);
                player.PlayerReferences.PlayerCanvas.transform.localScale = new Vector3(xScale, 1, 1);
                
                player.PlayerComponents.Animator.SetAttackDirection(Directions.Side);
                
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
