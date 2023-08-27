using Fusion;
using Player;
using UnityEngine;

namespace Weapons
{
    public class Gun : SimulationBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform barrelTransform;
        
        public float Energy => projectilePrefab.strikerData.Energy;
        
        private Weapon GunWeapon => GetComponent<Weapon>();

        public void Shoot()
        {
            if (!HasStateAuthority) return;
            var rotation = Quaternion.Euler(0, 0, playerController.transform.localScale.x > 0 ? 0 : 180);
            Runner.Spawn(projectilePrefab, barrelTransform.position, rotation, Object.InputAuthority);
            GunWeapon.PlaySound(projectilePrefab.strikerData.AudioClip);
            playerController.PlayerNetworkState.RangedEnergy -= projectilePrefab.strikerData.Energy;
        }
    }
}