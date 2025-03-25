using System;
using System.Collections.Generic;
using Main;
using UnityEngine;

namespace AssetBundleRes
{
    public class AssetLoadManager : SingleManager<AssetLoadManager>, IManager
    {
        private Dictionary<string, Asset> m_assets = new();

        public void Init()
        {
        }

        public void LoadAssetAsync<T>(string bundleName, string assetName, Action<Asset> callback)
        {
            string fullName = bundleName + assetName;
            if (m_assets.ContainsKey(fullName))
            {
                Asset asset = m_assets[fullName];
                if (asset.asset == null)
                {
                    asset.LoadAssetAsync<T>(bundleName, assetName);
                }

                asset.onComplete += callback;
            }
            else
            {
                Asset asset = new Asset();
                asset.LoadAssetAsync<T>(bundleName, assetName);
                asset.onComplete += callback;
                m_assets.Add(fullName, asset);
            }
        }

        public void Update()
        {
            foreach (var pair in m_assets)
            {
                pair.Value.Update();
            }
        }
    }
}