using Brawler;
using Characters;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.RankedMenu
{
    public class BrawlerItemsManager : MonoBehaviour
    {
        [Header("Game Objects")] 
        [SerializeField]
        private GameObject itemsPanel;

        [Header("Buttons")] 
        [SerializeField] private Button characterButton;

        [Header("Images")] 
        [SerializeField] private Image characterImage;
        [SerializeField] private Image swordImage;
        [SerializeField] private Image gunImage;

        [Header("Scriptable Objects")] 
        [SerializeField] private CharacterImages characterImages;
        [SerializeField] private SwordImages swordImages;
        [SerializeField] private GunImages gunImages;

        [Header("Modals")] 
        [SerializeField] private ModalManager modalManager;

        private void Start()
        {
            characterButton.onClick.AddListener(() => modalManager.OpenAvailableCharactersModal());
        }

        public void ShowItemsPanel(bool isShown)
        {
            itemsPanel.SetActive(isShown);
        }

        public void UpdateItemsDisplay()
        {
            SetCharacterImage();
            SetSwordImage();
            SetGunImage();
            ShowItemsPanel(true);
        }

        private void SetCharacterImage()
        {
            var noCharacter = BrawlerManager.Instance.Brawler.Character == CharactersEnum.None;
            characterImage.sprite = noCharacter
                ? null
                : characterImages.GetCharacterSprite((int)BrawlerManager.Instance.Brawler.Character - 1);
            var color = characterImage.color;
            color.a = noCharacter ? 0.1f : 1f;
            characterImage.color = color;
        }

        private void SetSwordImage()
        {
            var noSword = BrawlerManager.Instance.Brawler.Sword == Swords.None;
            swordImage.sprite = noSword
                ? null
                : swordImages.GetSwordImage((int)BrawlerManager.Instance.Brawler.Sword - 1);
            var color = swordImage.color;
            color.a = noSword ? 0.1f : 1f;
            swordImage.color = color;
        }
        
        private void SetGunImage()
        {
            var noGun = BrawlerManager.Instance.Brawler.Gun == Guns.None;
            gunImage.sprite = noGun
                ? null
                : gunImages.GetGunImage((int)BrawlerManager.Instance.Brawler.Gun - 1);
            var color = gunImage.color;
            color.a = noGun ? 0.1f : 1f;
            gunImage.color = color;
        }
    }
}