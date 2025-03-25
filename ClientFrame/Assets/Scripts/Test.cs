using System;
using System.Collections;
using System.Collections.Generic;
using AssetBundleRes;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [ContextMenu("testLoad")]
    public void testLoad()
    {
        AssetLoadManager.Instance.LoadAssetAsync<GameObject>("prefab", "Prefab1", (asset) =>
        {
            GameObject prefab = asset.GetAsset<GameObject>();
            GameObject go = Instantiate(prefab);
            go.transform.SetParent(transform);
            go.name = "prefab1";
        });
    }
    
    IEnumerator LoadBundleAsync()
    {
        string bundlePath = Application.streamingAssetsPath + "/AssetBundles/Windows/prefab";
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundle bundle = request.assetBundle;
        if (bundle == null)
        {
            Debug.LogError("加载失败");
            yield break;
        }

        AssetBundleRequest prefabRequest = bundle.LoadAssetAsync<GameObject>("prefab1");
        yield return prefabRequest;

        GameObject hero = prefabRequest.asset as GameObject;
        Instantiate(hero, transform, true);
        bundle.Unload(false);
    }
}
