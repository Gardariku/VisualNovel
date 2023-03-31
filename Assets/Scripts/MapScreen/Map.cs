using System;
using System.Collections.Generic;
using DialogueScreen;
using UnityEngine;

namespace MapScreen
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private DialogueSystem _dialogue;
        [SerializeField] private MapSite[] _sites;
        public Dictionary<string, MapSite> Sites { get; private set; }

        private void Start()
        {
            Sites = new Dictionary<string, MapSite>();
            foreach (var site in _sites)
                Sites.Add(site.Name, site);

            _dialogue.SitesOpened += OnSitesOpened;
            _dialogue.SitesClosed += OnSitesClosed;

            gameObject.SetActive(false);
        }

        private void OnSitesClosed(string[] closedSites)
        {
            foreach (var closedSite in closedSites)
                Sites[closedSite].Status = false;
        }

        private void OnSitesOpened(string[] openedSites, string[] paths)
        {
            for (int i = 0; i < openedSites.Length; i++)
            {
                Sites[openedSites[i]].Status = true;
                Sites[openedSites[i]].Path = paths[i];
            }
        }
    }
}
