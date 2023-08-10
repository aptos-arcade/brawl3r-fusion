using Fusion;
using UnityEngine;

namespace Utilities
{
    public class DestroyAfterSeconds : NetworkBehaviour
    {

        [SerializeField] private float destroySeconds;
        
        [Networked] private TickTimer DestroyTimer { get; set; }

        public override void Spawned()
        {
            DestroyTimer = TickTimer.CreateFromSeconds(Runner, destroySeconds);
        }

        public override void FixedUpdateNetwork()
        {
            if (!DestroyTimer.Expired(Runner)) return;
            Runner.Despawn(Object);
        }
    }
}
