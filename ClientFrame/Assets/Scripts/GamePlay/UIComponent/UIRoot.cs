using UnityEngine;

public class UIRoot : MonoBehaviour
{
    public Transform SingleRoot => m_singleRoot;

    [SerializeField]
    private Transform m_singleRoot;
    
    public Transform MainRoot => m_mainRoot;
    
    [SerializeField]
    private Transform m_mainRoot;
    
    public Transform PopRoot => m_popRoot;
    
    [SerializeField]
    private Transform m_popRoot;
    
    public Transform CoverRoot => m_coverRoot;
    
    [SerializeField]
    private Transform m_coverRoot;
    
    void Start()
    {
    }

    [ContextMenu("testShow")]
    public void test()
    {
        UIManager.Instance.ShowUI("UIMainView", ()=>{Debug.LogError("load main finish");});
    }

    [ContextMenu("testHide")]
    public void testHideUI()
    {
        UIManager.Instance.HideUI("UIMainView");
    }
    
}
