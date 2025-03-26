using UnityEngine;

namespace AssetBundleRes
{
    public class BundleRequest
    {
        private AssetBundleCreateRequest m_request;
        
        public string BundleName=> m_bundleName;
        
        private string m_bundleName;

        public BundleRequest(AssetBundleCreateRequest request, string bundleName)
        {
            m_request = request;
            m_bundleName = bundleName;
        }

        public bool IsDone()
        {
            return m_request.isDone && BundleManager.Instance.CheckBundleDependenciesLoaded(m_bundleName);
        }

        public void Update()
        {
            if (IsDone())
            {
                BundleManager.Instance.OnBundleLoaded(m_bundleName, m_request.assetBundle);
            }
        }
    }
}