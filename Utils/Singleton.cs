using System;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = (T)FindObjectOfType(typeof(T));
            if (_instance == null)
            {
                var singletonGameObjectName = $"@{typeof(T).ToString()}";
                var go = GameObject.Find(singletonGameObjectName);
                if (go == null)
                {
                    go = new GameObject();
                    go.name = singletonGameObjectName;
                }

                _instance = go.AddComponent<T>();
            }

            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (Instance.IsUnityNull())
        {
            Debug.LogError($"싱글톤 로드에 실패했습니다. {this.gameObject.name}");
        }
    }

    public virtual void OnApplicationQuit()
    {
        _instance = null;
    }
}