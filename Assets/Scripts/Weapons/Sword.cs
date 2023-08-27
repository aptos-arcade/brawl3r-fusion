using Player;
using Player.NetworkBehaviours;
using UnityEngine;

namespace Weapons
{
    public class Sword : Striker
    {
        
        [SerializeField] private PlayerController owner;
        
        protected override void OnPlayerStrike(Vector2 position, PlayerController player)
        {
            base.OnPlayerStrike(position, player);
            // StartCoroutine(DisableCoroutine(player));
        }
        
        private Weapon SwordWeapon => GetComponent<Weapon>();
        
        protected override void OnShieldStrike(Vector2 position, PlayerShield shield)
        {
            base.OnShieldStrike(position, shield);
            owner.PlayerUtilities.ShieldHit(shield);
        }

        public void Attack()
        {
            SwordWeapon.PlaySound(strikerData.AudioClip);
            owner.PlayerNetworkState.MeleeEnergy -= strikerData.Energy;
        }
        
        protected override int GetDirection()
        {
            return owner.transform.localScale.x > 0 ? 1 : -1;
        }

        // private static IEnumerator DisableCoroutine(PlayerController player)
        // {
        //     player.PlayerComponents.BodyCollider.enabled = false;
        //     yield return new WaitForSeconds(0.25f);
        //     player.PlayerComponents.BodyCollider.enabled = true;
        // }
    }
}