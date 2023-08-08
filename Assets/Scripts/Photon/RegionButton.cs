using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Photon
{
    public class RegionButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;
        
        public void Initialize(RegionPing regionPing)
        {
            text.text = $"{regionPing.name} (Ping: {regionPing.ping})";
            button.onClick.AddListener(() => RegionManager.Instance.SelectRegion(regionPing));
        }
    }
}