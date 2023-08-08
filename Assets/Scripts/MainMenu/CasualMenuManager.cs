using Brawler;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MainMenu
{
    public class CasualMenuManager : MonoBehaviour
    {

        [Header("Game Objects")]
        [SerializeField] private GameObject characterSelect;
        [SerializeField] private GameObject roomSelect;
        [SerializeField] private GameObject brawlerDisplay;
        
        [Header("Buttons")]
        [SerializeField] private Button continueToRoomsButton;
        [SerializeField] private Button backToModeSelectButton;
        [SerializeField] private Button backButton;
        
        // Start is called before the first frame update
        private void Start()
        {
            characterSelect.SetActive(true);
            brawlerDisplay.SetActive(true);
            roomSelect.SetActive(false);
            
            BrawlerManager.Instance.SetBrawlerCharacter(Characters.Characters.GetRandomCharacter());
            BrawlerManager.Instance.SetBrawlerGun(Random.Range(1, 6));
            BrawlerManager.Instance.SetBrawlerSword(Random.Range(1, 6));
            BrawlerManager.Instance.SetName(AuthenticationService.Instance.PlayerName.Split("#")[0]);
            BrawlerManager.Instance.SetId(AuthenticationService.Instance.PlayerId);
            continueToRoomsButton.onClick.AddListener(OnContinueToRooms);
        }

        private void OnContinueToRooms()
        {
            characterSelect.SetActive(false);
            brawlerDisplay.SetActive(false);
            continueToRoomsButton.gameObject.SetActive(false);
            roomSelect.SetActive(true);
            backToModeSelectButton.gameObject.SetActive(false);
            SetBackButtonHandler(OnBackToCharacterSelect);
        }
        
        private void OnBackToCharacterSelect()
        {
            characterSelect.SetActive(true);
            brawlerDisplay.SetActive(true);
            continueToRoomsButton.gameObject.SetActive(true);
            roomSelect.SetActive(false);
            backToModeSelectButton.gameObject.SetActive(true);
            backButton.gameObject.SetActive(false);
        }

        private void SetBackButtonHandler(UnityAction handler)
        {
            backButton.gameObject.SetActive(true);
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(handler);
        }
    }
}
