using Player;
using Player.NetworkBehaviours;
using UnityEngine;

namespace Weapons
{
    public class Projectile: Striker
    {
        [SerializeField] private float speed;

        public override void FixedUpdateNetwork()
        {
            transform.Translate(transform.right * speed * Runner.DeltaTime, Space.World);
            Debug.Log(Object);
        }

        protected override void OnPlayerStrike(Vector2 position, PlayerController player)
        {
            base.OnPlayerStrike(position, player);
            Runner.Despawn(Object);
        }
        
        protected override void OnShieldStrike(Vector2 position, PlayerShield shield)
        {
            base.OnShieldStrike(position, shield);
            Runner.Despawn(Object);
        }
        
        protected override int GetDirection()
        {
            return transform.rotation.z > 0 ? -1 : 1;
        }
    }
}
