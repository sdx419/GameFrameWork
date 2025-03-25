using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetBundleRes
{
    public class AssetLoadOperation
    {
        public string m_bundleName;
        private string m_assetName;
        private Type m_type;
        private LoadedAssetBundle m_loadedBundle;
        private AssetBundleRequest m_request;
        private Object m_asset;
        private bool m_isDone = false;

        public bool IsDone => m_isDone;
        
        public AssetLoadOperation(string bundleName, string assetName, Type type)
        {
            m_bundleName = bundleName;
            m_assetName = assetName;
            m_type = type;
        }

        public Object GetAsset()
        {
            return m_asset;
        }
        
        public void Update()
        {
            if(m_isDone)
                return;

            BundleManager.Instance.GetAssetBundle(m_bundleName, out m_loadedBundle);
            if(m_loadedBundle == null)
                return;
            
            if (m_request == null)
            {
                m_request = m_loadedBundle.Bundle.LoadAssetAsync(m_assetName, m_type);
            }
            else if (m_request.isDone)
            {
                m_asset = m_request.asset;
                m_isDone = true;
            }

        }
    }
}