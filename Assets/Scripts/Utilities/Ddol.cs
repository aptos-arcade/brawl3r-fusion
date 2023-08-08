using UnityEngine;

namespace Utilities
{
    public class Ddol : MonoBehaviour
    {
        
        private static Ddol instance;
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}