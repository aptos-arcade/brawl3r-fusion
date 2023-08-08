using Fusion;
using UnityEngine;

namespace Weapons
{
    public class Projectile: Striker
    {
        [SerializeField] private float speed;
        [SerializeField] private float destroyTime;

        private Vector2 direction;
        
        [Networked] private TickTimer LifeTimer { get; set; }

        public override void Spawned()
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, destroyTime);
        }


        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if (!LifeTimer.ExpiredOrNotRunning(Runner))
            {
                transform.Translate(transform.right * speed * Runner.DeltaTime, Space.World);
            }

            if (LifeTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }

        private void Destroy()
        {
            Runner.Despawn(Object);
        }

        protected override void OnPlayerStrike(Vector2 position)
        {
            Destroy();
        }
    }
}
