using Com.LuisPedroFonseca.ProCamera2D;
using Gameplay;
using UnityEngine;

namespace Player.PlayerModules
{
    public static class PlayerCameraController
    {
        private static ProCamera2D ProCamera2D => GameManager.Instance.SceneCamera;
        
        private static ProCamera2DShake CameraShake => ProCamera2D.GetComponent<ProCamera2DShake>();

        public static void AddPlayer(Transform playerTransform)
        {
            ProCamera2D.AddCameraTarget(playerTransform);
        }
        
        public static void RemovePlayer(Transform playerTransform)
        {
            ProCamera2D.RemoveCameraTarget(playerTransform);
        }

        public static void ShakeCamera(float duration, Vector2 strength)
        {
            CameraShake.Shake(duration, strength);
        }
    }
}