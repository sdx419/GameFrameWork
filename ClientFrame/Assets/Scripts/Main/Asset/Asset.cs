using System;

namespace AssetBundleRes
{
    public class Asset
    {
        private string m_bundleName;
        private string m_assetName;
        private UnityEngine.Object m_asset;
        private AssetLoadOperation m_option;

        private int m_refCount = 0;
        private bool m_loading = false;

        public event Action<Asset> OnComplete;

        public void LoadAssetAsync<T>(string bundleName, string assetName)
        {
            m_refCount++;
            
            m_bundleName = bundleName;
            m_assetName = assetName;
            m_option ??= BundleManager.Instance.LoadAssetAsync<T>(bundleName, assetName);
            m_loading = true;
        }
        
        public T GetAsset<T>() where T : UnityEngine.Object
        {
            return m_asset as T;
        }

        public void Release()
        {
            m_refCount--;
            if (m_refCount <= 0)
            {
                m_asset = null;
                m_option = null;
                AssetLoadManager.Instance.RemoveAsset(m_bundleName + m_assetName);
                BundleManager.Instance.UnloadBundle(m_bundleName);
                //BundleManager.Instance.GetAssetBundle(m_bundleName, out var bundle);
                //bundle.Release();
            }
        }
        
        public void Update()
        {
            if(!m_loading)
                return;
            
            if (m_option is { IsDone: true })
            {
                m_asset = m_option.GetAsset();
                OnComplete?.Invoke(this);
                OnComplete = null;
                m_loading = false;
            }
        }
    }
}