using Player.Animations;
using UnityEngine;

namespace Player
{
    public class PlayerVisualController : MonoBehaviour
    { 
        [SerializeField] private PlayerController playerController;

        private GameObject PlayerCanvas => playerController.PlayerReferences.PlayerCanvas;
        
        // player visual state
        private bool isFacingRight = true;
        private bool isInitialized;
        
        // original scales
        private Vector3 originalPlayerScale;
        private Vector3 originalCanvasScale;
        
        private void Start()
        {
            originalPlayerScale = transform.localScale;
            originalCanvasScale = PlayerCanvas.transform.localScale;
            isInitialized = true;
        }

        public void UpdateScaleTransforms()
        {
            if (!isInitialized) return;
            isFacingRight = playerController.PlayerState.Direction.x switch
            {
                > 0.1f => true,
                < -0.1f => false,
                _ => isFacingRight
            };
            
            SetObjectLocalScaleBasedOnDir(transform, originalPlayerScale);
            SetObjectLocalScaleBasedOnDir(PlayerCanvas.transform, originalCanvasScale);
        }

        private void SetObjectLocalScaleBasedOnDir(Transform tr, Vector3 originalScale)
        {
            var xValue = isFacingRight ? originalScale.x : -originalScale.x;
            tr.localScale = new Vector3(xValue, originalScale.y, originalScale.z);
        }
        
        
    }
}