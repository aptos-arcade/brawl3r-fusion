using Characters;
using UnityEngine;
using Weapons;

namespace Brawler
{
    public class BrawlerDisplay : MonoBehaviour
    {
        [SerializeField] private CharacterDisplay character;
        [SerializeField] private GunSprite gunSprite;
        [SerializeField] private SwordSprite swordSprite;

        private void OnEnable()
        {
            BrawlerManager.OnBrawlerChanged += UpdateBrawlerDisplay;
            UpdateBrawlerDisplay();
        }
        
        private void OnDisable()
        {
            BrawlerManager.OnBrawlerChanged -= UpdateBrawlerDisplay;
        }

        private void UpdateBrawlerDisplay()
        {
            character.SetCharacter(BrawlerManager.Instance.Brawler.Character);
            gunSprite.SetGun(BrawlerManager.Instance.Brawler.Gun);
            swordSprite.SetSword(BrawlerManager.Instance.Brawler.Sword);
        }
    }
}