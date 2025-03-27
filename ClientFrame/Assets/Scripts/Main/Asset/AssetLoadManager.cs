using System;
using System.Collections.Generic;
using Main;
using UnityEditor;
using Object = UnityEngine.Object;

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
                asset.OnComplete += callback;
                asset.LoadAssetAsync<T>(bundleName, assetName);
            }
            else
            {
                Asset asset = new Asset();
                asset.OnComplete += callback;
                m_assets.Add(fullName, asset);
                asset.LoadAssetAsync<T>(bundleName, assetName);
            }
        }

        public void RemoveAsset(string assetName)
        {
            m_assets.Remove(assetName);
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