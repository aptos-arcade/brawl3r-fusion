using Gameplay;
using Global;
using Weapons;

namespace Player.PlayerModules
{
    public class PlayerAttacks
    {

        private readonly PlayerController player;
        
        public PlayerAttacks(PlayerController player)
        {
            this.player = player;
        }

        public void Shoot()
        {
            if(player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyAttack)) return;
            if (player.PlayerNetworkState.RangedEnergy >= player.PlayerReferences.Gun.Energy)
            {
                player.PlayerAttacks.Attack(Global.Weapons.Gun);
            }
            else
            {
                player.PlayerReferences.PlayerCanvasManager.NoEnergy(EnergyManager.EnergyType.Gun);
            }
        }

        public void Melee(StrikerData attackData, Directions direction)
        {
            if (player.PlayerAnimations.IsCurrentBodyAnimation(Animations.Animations.BodyAttack)) return;
            if (player.PlayerNetworkState.MeleeEnergy >= attackData.Energy)
            {
                player.PlayerReferences.Sword.strikerData = attackData;
                player.PlayerAnimations.SetAttackDirection(direction);
                Attack(Global.Weapons.Sword);
            }
            else
            {
                player.PlayerReferences.PlayerCanvasManager.NoEnergy(EnergyManager.EnergyType.Sword);
            }
        }
        
        private void Attack(Global.Weapons weapon)
        {
            player.PlayerUtilities.TriggerInvincibility(false);
            if (weapon != player.PlayerNetworkState.Weapon)
            {
                player.PlayerNetworkState.Weapon = weapon;
            }
            player.PlayerAnimations.TryAttack();
        }

        public void SwapWeapon()
        {
            player.PlayerAnimations.SetWeapon(player.PlayerNetworkState.Weapon);
            for(var i = 0; i < player.PlayerReferences.WeaponObjects.Length; i++)
            {
                player.PlayerReferences.WeaponObjects[i].gameObject.SetActive(
                    i == (int)player.PlayerNetworkState.Weapon);
            }
        }
    }
}
