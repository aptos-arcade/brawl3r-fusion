using Com.LuisPedroFonseca.ProCamera2D;
using Gameplay;
using UnityEngine;

namespace Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        private ProCamera2D proCamera2D;
        
        private ProCamera2DShake cameraShake;

        private void Awake()
        {
            proCamera2D = GameManager.Instance.SceneCamera;
            cameraShake = GameManager.Instance.SceneCamera.GetComponent<ProCamera2DShake>();
        }

        public void AddPlayer(Transform playerTransform)
        {
            proCamera2D.AddCameraTarget(playerTransform);
        }
        
        public void RemovePlayer(Transform playerTransform)
        {
            proCamera2D.RemoveCameraTarget(playerTransform);
        }

        public void ShakeCamera(float duration, Vector2 strength)
        {
            cameraShake.Shake(duration, strength);
        }
    }
}