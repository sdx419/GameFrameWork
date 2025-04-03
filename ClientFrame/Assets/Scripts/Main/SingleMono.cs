using UnityEngine;

namespace Main
{
    public class SingleMono<T> : MonoBehaviour where T : Component
    {
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    GameObject go = GameObject.Find(typeof(T).Name) ?? new GameObject();
                    go.name = typeof(T).Name;
                    DontDestroyOnLoad(go);
                    m_instance = go.GetComponent<T>() ?? go.AddComponent<T>();
                }
                return m_instance;
            }
        }
        
        private static T m_instance;
    }

}