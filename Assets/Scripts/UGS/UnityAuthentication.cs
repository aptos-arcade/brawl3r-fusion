using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UGS
{
    public class UnityAuthentication : MonoBehaviour
    {
        private async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();
                SetupEvents();
                await SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException ex)
            {
                
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
            }
        }


        private static void SetupEvents()
        {
            AuthenticationService.Instance.SignedIn += OnSignedIn;
            AuthenticationService.Instance.SignInFailed += OnSignInFailed;
            AuthenticationService.Instance.SignedOut += OnSignedOut;
            AuthenticationService.Instance.Expired += OnExpired;
        }

        private static void OnSignedIn()
        {
            SceneManager.LoadScene("RegionSelectScene");
        }

        private static void OnSignInFailed(RequestFailedException err)
        {
        }
        
        private static void OnSignedOut()
        {
        }
        
        private static void OnExpired()
        {
        }
    }
}
