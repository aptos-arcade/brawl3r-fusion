using Brawler;
using ScriptableObjects;
using UnityEngine;

namespace Weapons
{
    public class SwordSprite : MonoBehaviour
    {
        [SerializeField] private SwordImages swordImages;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void SetSword(Swords sword)
        {
            spriteRenderer.sprite = swordImages.GetSwordImage((int)sword);
        }
    }
}