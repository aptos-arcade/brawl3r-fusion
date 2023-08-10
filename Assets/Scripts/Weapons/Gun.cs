using Fusion;
using Player;
using UnityEngine;

namespace Weapons
{
    public class Gun : NetworkBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Projectile projectilePrefab;
        public Projectile ProjectilePrefab => projectilePrefab;
        
        [SerializeField] private Transform barrelTransform;
        
        public void Shoot()
        {
            // get the quaternion of based on the player's local scale
            var rotation = Quaternion.Euler(0, 0, playerController.transform.localScale.x > 0 ? 0 : 180);
            Runner.Spawn(projectilePrefab, barrelTransform.position, rotation);
        }
    }
}