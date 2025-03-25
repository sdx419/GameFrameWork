using System;
using UnityEngine;
// using Object = UnityEngine.Object;

namespace AssetBundleRes
{
    public class Asset
    {
        private string m_bundleName;
        private string m_assetName;
        public UnityEngine.Object asset;
        public AssetLoadOperation m_Option;

        public Action<Asset> onComplete;

        private bool m_loading = false;

        public void LoadAssetAsync<T>(string bundleName, string assetName)
        {
            m_bundleName = bundleName;
            m_assetName = assetName;
            m_Option = BundleManager.Instance.LoadAssetAsync<T>(bundleName, assetName);
            m_loading = true;
        }

        public void LoadAssetAsyncInternal()
        {
            if(m_loading)
                return;

            m_Option = BundleManager.Instance.LoadAssetAsync<UnityEngine.Object>(m_bundleName, m_assetName);
        }

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            return asset as T;
        }
        
        public void Update()
        {
            if(!m_loading)
                return;
            
            if (m_Option is { IsDone: true })
            {
                asset = m_Option.GetAsset();
                onComplete?.Invoke(this);
                m_loading = false;
            }
        }
    }
}