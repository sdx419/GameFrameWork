using System.Collections.Generic;
using System.Linq;
using Main;
using UnityEngine;

namespace AssetBundleRes
{
    public class LoadedAssetBundle
    {
        public AssetBundle Bundle => m_bundle;
        
        private AssetBundle m_bundle;
        private int m_refCount;

        public LoadedAssetBundle(AssetBundle bundle)
        {
            m_bundle = bundle;
        }
        
        public void Retain()
        {
        }

        private void unload()
        {
        }

        public void Release()
        {
        }
    }

    public class BundleManager : SingleManager<BundleManager>, IManager
    {
        private Dictionary<string, LoadedAssetBundle> m_loadedAssetBundles = new();
        private List<BundleRequest> m_loadingRequest = new();
        private List<AssetLoadOperation> m_InProcessOperations = new();

        private AssetBundle m_manifestBundle;
        private AssetBundleManifest m_manifest;

        private static string m_manifestBundlePath = Application.streamingAssetsPath + "/assets/assets";
        private static string m_manifestPath = m_manifestBundlePath + "/AssetBundleManifest";
        private static string m_assetBundlePath = Application.streamingAssetsPath + "/assets/";

        public void Init()
        {
            Debug.LogError($"bundleManager init");
            m_manifestBundle = AssetBundle.LoadFromFile(m_manifestBundlePath);
            m_manifest = m_manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        
        public AssetLoadOperation LoadAssetAsync<T>(string bundleName, string assetName)
        {
            bundleName = m_assetBundlePath + bundleName;
            LoadAssetBundleAsync(bundleName);
            var operation = new AssetLoadOperation(bundleName, assetName, typeof(T));
            m_InProcessOperations.Add(operation);
            return operation;
        }

        public void OnBundleLoaded(string bundleName, AssetBundle bundle)
        {
            if (m_loadedAssetBundles.ContainsKey(bundleName))
            {
                m_loadedAssetBundles[bundleName].Retain();
            }
            else
            {
                LoadedAssetBundle _bundle = new LoadedAssetBundle(bundle);
                m_loadedAssetBundles.Add(bundleName, _bundle);
            }
        }

        public void GetAssetBundle(string bundleName, out LoadedAssetBundle bundle)
        {
            bundle = m_loadedAssetBundles.ContainsKey(bundleName) ? m_loadedAssetBundles[bundleName] : null;
        }

        public bool CheckBundleDependenciesLoaded(string bundleName)
        {
            string[] dependencies = m_manifest.GetAllDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                if (!m_loadedAssetBundles.ContainsKey(dependency))
                    return false;
            }

            return true;
        }

        public void LoadAssetBundleAsync(string bundleName)
        {
            LoadAssetBundleAsyncInternal(bundleName);
            LoadBundleDependenciesAsync(bundleName);
        }

        public void LoadBundleDependenciesAsync(string bundleName)
        {
            string[] dependencies = m_manifest.GetAllDependencies(bundleName);
            foreach (var dependency in dependencies)
            {
                LoadAssetBundleAsyncInternal(dependency);
            }
        }

        private void LoadAssetBundleAsyncInternal(string bundleName)
        {
            if (m_loadedAssetBundles.ContainsKey(bundleName))
            {
                m_loadedAssetBundles[bundleName].Retain();
                return;
            }
            
            // 注意！
            // 已经请求加载bundle，但是此bundle尚未加载完成，则新增加载bundle的请求
            // if (m_loadingRequest.Any(item => item.BundleName == bundleName))
                // return;

            var request = AssetBundle.LoadFromFileAsync(bundleName);
            BundleRequest bundleRequest = new BundleRequest(request, bundleName);
            m_loadingRequest.Add(bundleRequest);
        }

        public void Update()
        {
            for (var i = m_loadingRequest.Count - 1; i >= 0 ; i--)
            {
                var request = m_loadingRequest[i];
                request.Update();

                if (request.IsDone())
                    m_loadingRequest.RemoveAt(i);
            }

            for (var i = m_InProcessOperations.Count - 1; i >= 0; i--)
            {
                var bundleOption = m_InProcessOperations[i];
                bundleOption.Update();
                
                if (bundleOption.IsDone)
                    m_InProcessOperations.RemoveAt(i);
            }
        }

        public void UnloadBundle(string bundleName)
        {
            if(!m_loadedAssetBundles.ContainsKey(bundleName))
                return;
        }
    }
}