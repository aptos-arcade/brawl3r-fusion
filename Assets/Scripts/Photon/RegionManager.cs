using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public IEnumerator PingRegions(Action<List<RegionPing>> onComplete)
        {
            var pings = regions.Select(region => new Ping(region.address)).ToList();
            while (pings.Any(ping => !ping.isDone)) yield return null;
            var regionPings = pings
                .Select((t, i) => new RegionPing(regions[i], t.time))
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