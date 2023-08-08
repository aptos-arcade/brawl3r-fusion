using Brawler;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class CharacterCard : MonoBehaviour
    {
        [SerializeField] private CharactersEnum characterEnum;

        private Button selectButton;

        private void Start()
        {
            selectButton = GetComponent<Button>();
            selectButton.onClick.AddListener(SelectCharacter);
        }

        private void SelectCharacter()
        {
            BrawlerManager.Instance.SetBrawlerCharacter(characterEnum);
        }
    }
}