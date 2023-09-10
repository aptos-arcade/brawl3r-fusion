using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class RegionSelectUi : MonoBehaviour
    {
        [SerializeField] private RegionButton regionButtonPrefab;
        
        [SerializeField] private GameObject loadingText;
        [SerializeField] private GameObject regionList;
        
        [SerializeField] private Transform topRegionsParent;
        
        [SerializeField] private GameObject allRegionsGameObject;
        [SerializeField] private Transform allRegionsParent;
        
        [SerializeField] private Button showMoreButton;
        

        private void Start()
        {
            showMoreButton.onClick.AddListener(OnShowMore);
            loadingText.SetActive(true);
            regionList.SetActive(false);
            topRegionsParent.gameObject.SetActive(true);
            allRegionsGameObject.SetActive(false);
            showMoreButton.gameObject.SetActive(false);
            RegionManager.Instance.PingRegions(UpdatePingDisplays);
        }

        private void UpdatePingDisplays(List<RegionPing> regionPings)
        {
            foreach (GameObject pingButton in topRegionsParent)
            {
                Destroy(pingButton);
            }
            foreach (GameObject pingButton in allRegionsParent)
            {
                Destroy(pingButton);
            }
            
            // add the first three regions to the top regions
            for (var i = 0; i < 3; i++)
            {
                var regionPing = regionPings[i];
                var regionButton = Instantiate(regionButtonPrefab, topRegionsParent);
                regionButton.Initialize(regionPing);
            }
            
            // add all regions to the all regions
            foreach (var regionPing in regionPings)
            {
                var regionButton = Instantiate(regionButtonPrefab, allRegionsParent);
                regionButton.Initialize(regionPing);
            }
            
            loadingText.SetActive(false);
            regionList.SetActive(true);
            showMoreButton.gameObject.SetActive(true);
        }

        private void OnShowMore()
        {
            topRegionsParent.gameObject.SetActive(false);
            allRegionsGameObject.SetActive(true);
            showMoreButton.gameObject.SetActive(false);
        }
    }
}