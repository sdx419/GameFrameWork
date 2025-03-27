using System;
using System.Collections.Generic;
using AssetBundleRes;
using Main;
using UnityEngine;

public class UIManager : SingleMono<UIManager>
{
    private UIRoot m_uiRoot;
    private Dictionary<string, Asset> m_uiAssets = new();
    public void Init()
    {
    }

    public void RegisterUIRoot(UIRoot uiRoot)
    {
        if (m_uiRoot == null)
            m_uiRoot = uiRoot;
    }

    public void ShowUI(string uiName, Action callback)
    {
        AssetLoadManager.Instance.LoadAssetAsync<GameObject>("uiprefab", uiName, (asset) =>
        {
            GameObject uiRes = asset.GetAsset<GameObject>();
            GameObject uiPrefab = UnityEngine.Object.Instantiate(uiRes);
            uiPrefab.transform.parent = m_uiRoot.MainRoot;
            uiPrefab.transform.localScale = Vector3.one;
            uiPrefab.transform.localPosition = Vector3.zero;
            m_uiAssets.Add(uiName, asset);
            callback?.Invoke();
        });
    }

    public void HideUI(string uiName)
    {
        m_uiAssets[uiName].Release();
    }

    public void Update()
    {
    }
}