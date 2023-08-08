using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class ServerFooter : MonoBehaviour
    {
        [SerializeField] private TMP_Text regionText;

        [SerializeField] private Button disconnectButton;

        private void Start()
        {
            disconnectButton.onClick.AddListener(RegionManager.Instance.ClearRegion);
            regionText.text = $"Region: {RegionManager.Instance.SelectedRegion.name}";
        }
    }
}