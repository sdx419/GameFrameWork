using System.Collections;

namespace Main
{
    public class SingleManager<T> where T : class, IManager, new()
    {
        public static T Instance => m_instance ??= GlobalManager.Instance.GetManagerSingleInstance<T>();

        private static T m_instance;
    }

    
}