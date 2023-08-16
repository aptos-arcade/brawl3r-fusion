using UnityEngine;

namespace Player.PlayerModules
{
    public class PlayerAudioController
    {
        private readonly PlayerController player;
        
        public PlayerAudioController(PlayerController player)
        {
            this.player = player;
        }
        
        private AudioSource RunAudioSource => player.PlayerComponents.RunAudioSource;
        
        private AudioSource OneShotAudioSource => player.PlayerComponents.OneShotAudioSource;

        public void HandleAudio()
        {
            if (player.PlayerComponents.RigidBody.velocity.magnitude > 0.1f && player.PlayerProperties.IsGrounded &&
                !player.PlayerProperties.IsDashing && !player.PlayerProperties.IsDodging)
            {
                if(!RunAudioSource.isPlaying) RunAudioSource.Play();
            }
            else
            {
                if(RunAudioSource.isPlaying) RunAudioSource.Stop();
            }
        }
        
        public void PlayOneShotAudio(AudioClip clip)
        {
            OneShotAudioSource.Stop();
            OneShotAudioSource.PlayOneShot(clip);
        }
    }
}