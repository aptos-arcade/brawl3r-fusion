using System.Collections;
using ApiServices.Models.Fetch;
using AptosIntegration;
using Brawler;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu.RankedMenu
{
    public class YourBrawlerManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text loadingText;
        
        [SerializeField] private BrawlerDisplay brawlerDisplayPrefab;
        
        [SerializeField] private Button continueToRoomSelectButton;
        
        [Header("Managers")]
        [SerializeField] private BrawlerItemsManager brawlerItemsManager;
        [SerializeField] private PlayerEloRatingManager playerEloRatingManager;

        private BrawlerDisplay curBrawlerDisplay;
        
        private void OnEnable()
        {
            StartCoroutine(LoadCoroutine());
            TransactionHandler.OnTransactionResult += OnTransactionResult;
        }
        
        private void OnDisable()
        {
            TransactionHandler.OnTransactionResult -= OnTransactionResult;
            if (curBrawlerDisplay != null) Destroy(curBrawlerDisplay.gameObject);
        }
        
        private void ShowLoading()
        {
            if(curBrawlerDisplay != null) Destroy(curBrawlerDisplay.gameObject);
            brawlerItemsManager.ShowItemsPanel(false);
            playerEloRatingManager.ShowEloRating(false);
            continueToRoomSelectButton.gameObject.SetActive(false);
            loadingText.text = "Loading...";
            loadingText.gameObject.SetActive(true);
        }

        private IEnumerator LoadCoroutine()
        {
            ShowLoading();
            yield return StartCoroutine(
                ApiServices.FetchServices.FetchBrawlerData(HandleFetchBrawlerData, WalletManager.Instance.Address));
        }

        private void HandleFetchBrawlerData(BrawlerData brawlerData)
        {
            if (brawlerData == null) return;
            var hasCharacter = brawlerData.character.collection != string.Empty;
            if (hasCharacter)
            {
                BrawlerManager.Instance.SetBrawlerCharacter(
                    Characters.Characters.GetCharacterEnum(brawlerData.character.collection));
            }
            BrawlerManager.Instance.SetBrawlerGun(brawlerData.meleeWeapon.type);
            BrawlerManager.Instance.SetBrawlerSword(brawlerData.rangedWeapon.type);

            // remove loading text
            loadingText.gameObject.SetActive(false);
            
            // create character display
            curBrawlerDisplay = Instantiate(brawlerDisplayPrefab, new Vector3(0f, -0.5f, 0), Quaternion.identity);
            curBrawlerDisplay.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            
            // update items and elo
            brawlerItemsManager.UpdateItemsDisplay();
            playerEloRatingManager.ShowEloRating(true);
            
            // show continue button
            continueToRoomSelectButton.gameObject.SetActive(true);
            continueToRoomSelectButton.interactable = hasCharacter;
        }

        private void OnTransactionResult(bool successful)
        {
            if (!successful) return;
            StartCoroutine(LoadCoroutine());
        }
    }
}