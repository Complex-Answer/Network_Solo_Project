using UnityEngine;

public class SingleTone<T> : MonoBehaviour where T : MonoBehaviour
{
    static private T _instance;
    static public T Instance
    {
        get
        {
            if (_instance == null) //인스턴스가 없으면
            {
                _instance = FindAnyObjectByType<T>(); //씬의 하이어라키에 있는지 확인해보기
                if (_instance == null) //그래도 씬에 없으면 새로만들기
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    _instance = obj.AddComponent<T>(); //해당 컴포넌트를 붙여서
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
