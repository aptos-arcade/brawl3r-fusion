using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ping = System.Net.NetworkInformation.Ping;

namespace Photon
{
    public class RegionManager : MonoBehaviour
    {
        public static RegionManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField] private List<Region> regions;

        public Region SelectedRegion { get; private set; }

        public void PingRegions(Action<List<RegionPing>> onComplete)
        {
            Debug.Log("Pinging regions...");
            var pings = regions.Select(region =>
            {
                var ping = new Ping();
                var pingReply = ping.Send(region.address, 2000);
                return pingReply?.RoundtripTime ?? -1;
            }).ToList();
            var regionPings = pings
                .Select((t, i) => new RegionPing(regions[i], (int)t))
                .Where(region => region.ping != -1)
                .ToList();
            regionPings.Sort((a, b) => a.ping.CompareTo(b.ping));
            onComplete(regionPings);
        }
        
        public void SelectRegion(Region region)
        {
            SelectedRegion = region;
            SceneManager.LoadScene("UsernameScene");
        }

        public void ClearRegion()
        {
            SelectedRegion = null;
            SceneManager.LoadScene("RegionSelectScene");
        }
    }

    [Serializable]
    public class Region
    {
        public string name;
        public string code;
        public string address;
    }
    
    public class RegionPing: Region
    {
        public RegionPing(Region region, int ping)
        {
            name = region.name;
            code = region.code;
            address = region.address;
            this.ping = ping;
        }
        
        public int ping;
    }
}