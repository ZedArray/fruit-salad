using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T:Singleton<T>
{

    private static Singleton<T> _instance;
    public static T instance {
        get {
            return (T)_instance;
        }
    }
    public void InitSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(instance);
        }
        else {
            Destroy(this);
        }
    }

}
