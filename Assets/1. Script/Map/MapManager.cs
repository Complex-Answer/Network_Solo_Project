using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    static public MapManager _instance;

    public int CurrentRow { get; set; } = -1; //-1은 시작 하지않은 위치
    public int CurrentCol { get; set; } = -1;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ExecuteEvent(NodeDataSO data)
    {
       
        switch (data._nodeType)
        {
            case NodeType.Combat: StartBattle(data._sceneName); break;
            case NodeType.Shop: OpenShop(); break;
            case NodeType.Box: OpenBox(); break;
            case NodeType.Boss: StartBattle(data._sceneName); break;
        }
    }
    private void StartBattle(string sceneName) => SceneManager.LoadScene(sceneName);
    private void OpenShop() => SceneManager.LoadScene("ShopScene");
    private void OpenBox() => SceneManager.LoadScene("BoxScene");

}
