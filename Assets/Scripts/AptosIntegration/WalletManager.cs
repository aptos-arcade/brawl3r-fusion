using System.Runtime.InteropServices;
using UnityEngine;
using Utilities;

namespace AptosIntegration
{
    public class WalletManager : MonoBehaviour
    {

        public static WalletManager Instance;
    
        private void Awake()
        {
            Instance = this;
        }

        public string Address { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(Address);
        public string AddressEllipsized => StringUtils.Ellipsize(Address);
        
        [DllImport("__Internal")]
        private static extern void SetConnectModalOpen(int isOpen);
    
        public delegate void WalletConnectedAction();
        public static event WalletConnectedAction OnConnect;
    
        public void SetAccountAddress(string accountAddress)
        {
            Address = accountAddress;
            OnConnect?.Invoke();
        }
        
        
        public static void OpenConnectWalletModal()
        {
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                SetConnectModalOpen(1);
            #endif
        }

    }
}
