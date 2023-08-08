using Brawler;
using ScriptableObjects;
using UnityEngine;

namespace Weapons
{
    public class GunSprite : MonoBehaviour
    {
        
        [SerializeField] private GunImages gunImages; 
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetGun(Guns gun)
        {
            spriteRenderer.sprite = gunImages.GetGunImage((int)gun);
        }
    }
}