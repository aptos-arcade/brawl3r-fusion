using Fusion;
using UnityEngine;

namespace Weapons
{
    public class Weapon: NetworkBehaviour
    {
        // components
        private AudioSource audioSource;
        
        // networked properties
        [Networked] private NetworkBool IsActive { get; set; }

        public override void Spawned()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioClip);
        }

        public override void FixedUpdateNetwork()
        {
            if (IsActive != gameObject.activeSelf)
            {
                gameObject.SetActive(IsActive);
            }
        }
    }
}
