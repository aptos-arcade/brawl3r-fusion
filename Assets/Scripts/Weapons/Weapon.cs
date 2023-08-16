using UnityEngine;

namespace Weapons
{
    public class Weapon: MonoBehaviour
    {
        // components
        [SerializeField] private AudioSource audioSource;

        public void PlaySound(AudioClip audioClip)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(audioClip);
        }
    }
}
