using Fusion;
using UnityEngine;

namespace Weapons
{
    public class HitEffect : NetworkBehaviour
    {

        [SerializeField] private float destroyTime;
        
        [SerializeField] private AudioSource audioSource;
        
        [Networked] private TickTimer LifeTimer { get; set; }
    
        public override void Spawned()
        {
            audioSource.Play();
            LifeTimer = TickTimer.CreateFromSeconds(Runner, destroyTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (LifeTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }
    }
}
