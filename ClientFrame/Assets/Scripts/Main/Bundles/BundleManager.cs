using System;
using System.Collections.Generic;
using System.Linq;
using Main;
using UnityEngine;

namespace AssetBundleRes
{
    public class LoadedAssetBundle
    {
        private string m_bundleName;
        
        public AssetBundle Bundle => m_bundle;
        private AssetBundle m_bundle;
        private int m_refCount;

        public event Action OnUnload;

        public LoadedAssetBundle(string bundleName, AssetBundle bundle)
        {
            m_bundleName = bundleName;
            m_bundle = bundle;
            m_refCount = 1;
        }
        
        public void Retain()
        {
            m_refCount++;
        }

        private void unload()
        {
            m_bundle.Unload(true);
            OnUnload?.Invoke();
            OnUnload = null;
        }

        public void Release()
        {
            m_refCount--;
            // 当引用计数为0时，必定已无其他bundle对此bundle有依赖，可以安全卸载
            if (m_refCount == 0)
            {
                unload();
                BundleManager.Instance.UnloadBundleDependencies(m_bundleName);
            }
        }
    }

    public class BundleManager : SingleManager<BundleManager>, IManager
    {
        private Dictionary<string, LoadedAssetBundle> m_loadedAssetBundles = new();
        private List<BundleRequest> m_loadingRequest = new();
        private List<AssetLoadOperation> m_InProcessOperations = new();
        private Dictionary<string, string[]> m_bundleDependencies = new();

        private AssetBundle m_manifestBundle;
        private AssetBundleManifest m_manifest;

        private static string m_manifestBundlePath = Application.streamingAssetsPath + "/assets/assets";
        private static string m_assetBundlePath = Application.streamingAssetsPath + "/assets/";

        public void Init()
        {
            m_manifestBundle = AssetBundle.LoadFromFile(m_manifestBundlePath);
            m_manifest = m_manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        #region LoadAsset

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
                LoadedAssetBundle loadedBundle = new LoadedAssetBundle(bundleName, bundle);
                loadedBundle.OnUnload += () => m_loadedAssetBundles.Remove(bundleName);
                m_loadedAssetBundles.Add(bundleName, loadedBundle);
            }
        }

        public void GetAssetBundle(string bundleName, out LoadedAssetBundle bundle)
        {
            bundle = m_loadedAssetBundles.ContainsKey(bundleName) ? m_loadedAssetBundles[bundleName] : null;
        }

        public bool CheckBundleDependenciesLoaded(string bundleName)
        {
            foreach (var dependency in m_bundleDependencies[bundleName])
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
            if (!m_bundleDependencies.ContainsKey(bundleName))
            {
                m_bundleDependencies[bundleName] = m_manifest.GetAllDependencies(bundleName);
            }
            foreach (var dependency in m_bundleDependencies[bundleName])
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

        public void UnloadBundleDependencies(string bundleName)
        {
            string[] dependencies = m_bundleDependencies[bundleName];
            foreach (var dependency in dependencies)
            {
                if (!m_loadedAssetBundles.ContainsKey(dependency))
                {
                    Debug.LogError($"bundle {dependency} has been unloaded before unloading bundle {bundleName}");
                    continue;
                }
                
                m_loadedAssetBundles[dependency].Release();
            }
        }
        
        #endregion

        #region UnloadAsset

        public void UnloadBundle(string bundleName)
        {
            if(!m_loadedAssetBundles.ContainsKey(bundleName))
                return;

            m_loadedAssetBundles[bundleName].Release();
        }

        #endregion

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
        
    }
}