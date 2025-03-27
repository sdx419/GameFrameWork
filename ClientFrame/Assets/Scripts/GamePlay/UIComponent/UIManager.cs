using System;
using System.Collections.Generic;
using AssetBundleRes;
using Main;
using UnityEngine;

public class UIManager : SingleMono<UIManager>
{
    [SerializeField] private Transform m_singleRoot;
    [SerializeField] private Transform m_mainRoot;
    [SerializeField] private Transform m_popRoot;
    [SerializeField] private Transform m_coverRoot;

    private Dictionary<string, Asset> m_uiAssets = new();
    
    public void Init()
    {
    }
    
    public void ShowUI(string uiName, Action callback)
    {
        AssetLoadManager.Instance.LoadAssetAsync<GameObject>("uiprefab", uiName, (asset) =>
        {
            GameObject uiRes = asset.GetAsset<GameObject>();
            GameObject uiPrefab = UnityEngine.Object.Instantiate(uiRes);
            uiPrefab.transform.parent = m_mainRoot;
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
    
    [ContextMenu("testShow")]
    public void test()
    {
        ShowUI("UIMainView", ()=>{Debug.LogError("load main finish");});
    }

    [ContextMenu("testHide")]
    public void testHideUI()
    {
        HideUI("UIMainView");
    }

    public void Update()
    {
    }
}