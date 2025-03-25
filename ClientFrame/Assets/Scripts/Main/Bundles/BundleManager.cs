using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Main;
using UnityEditor;
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

    public class BundleManager : SingleMono<BundleManager>
    {
        private Dictionary<string, LoadedAssetBundle> m_loadedAssetBundles = new();
        private Dictionary<string, BundleRequest> m_loadingRequest = new();
        private List<AssetLoadOperation> m_InProcessOperations = new();

        private AssetBundle m_manifestBundle;
        private AssetBundleManifest m_manifest;

        private static string m_manifestBundlePath = Application.streamingAssetsPath + "/assets/assets";
        private static string m_manifestPath = m_manifestBundlePath + "/AssetBundleManifest";

        public void Init()
        {
            Debug.LogError($"bundleManager init");
            // StartCoroutine(init());
        }

        private void Awake()
        {
            StartCoroutine(init());
        }

        public void Test()
        {
            Debug.LogError("Test");
        }

        public IEnumerator init()
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(m_manifestBundlePath);
            yield return bundleRequest;
            m_manifestBundle = bundleRequest.assetBundle;
            m_manifest = m_manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        public AssetLoadOperation LoadAssetAsync<T>(string bundleName, string assetName)
        {
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

                if (m_loadingRequest.ContainsKey(bundleName))
                    m_loadingRequest.Remove(bundleName);
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
            if (m_loadedAssetBundles.ContainsKey(bundleName) || m_loadingRequest.ContainsKey(bundleName))
                return;

            var request = AssetBundle.LoadFromFileAsync(bundleName);
            BundleRequest bundleRequest = new BundleRequest(request, bundleName);
            m_loadingRequest.Add(bundleName, bundleRequest);
        }

        public void Update()
        {
            foreach (var pair in m_loadingRequest)
            {
                pair.Value.Update();
            }

            foreach (var option in m_InProcessOperations)
            {
                option.Update();
            }
        }
    }
}