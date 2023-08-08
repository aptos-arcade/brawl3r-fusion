using Fusion;
using UnityEngine;

namespace Weapons
{
    public class Gun : NetworkBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        public Projectile ProjectilePrefab => projectilePrefab;
        
        [SerializeField] private Transform barrelTransform;
        
        public void Shoot()
        {
            Runner.Spawn(projectilePrefab, barrelTransform.position, Quaternion.identity);
        }
    }
}