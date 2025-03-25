using System;
using System.Collections.Generic;
using AssetBundleRes;
using UnityEngine;

namespace Main
{
    public class GlobalManager : SingleMono<GlobalManager>
    {

        private Dictionary<string, IManager> m_singleManagers = new();

        public T GetManagerSingleInstance<T>() where T : class, IManager, new()
        {
            string managerName = typeof(T).Name;
            
            if (m_singleManagers.ContainsKey(managerName))
                return m_singleManagers[managerName] as T;

            T manager = new T();
            manager.Init();
            m_singleManagers.Add(managerName, manager);
            return manager;
        }

        [ContextMenu("Test")]
        public void Test()
        {
            BundleManager.Instance.Test();
        }

        private void Update()
        {
            foreach (var pair in m_singleManagers)
            {
                pair.Value.Update();
            }
        }
        
        
    }
}