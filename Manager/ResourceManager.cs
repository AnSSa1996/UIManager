using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using UIFramework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class ResourceManager : Singleton<ResourceManager>
{
    private enum eAddressableType
    {
        Start,
        DefaultIsolation,
        Animation,
        Audio,
        Font,
        Texture,
        Shader,
        Material,
        Model,
        Prefab,
        Scenes,
        ScriptableObjects,
        Particle,
        End,
    }

    private readonly Dictionary<string, IResourceLocation> _addressableLocation = new Dictionary<string, IResourceLocation>();
    private long _currentSize = 0;
    private long _totalSize = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        StartCoroutine(CoInit());
    }

    private IEnumerator CoInit()
    {
        Debug.Log($"ResourceManager Init");
        SetFrameRate();
        yield return StartCoroutine(CoInitResourceManager());
    }

    private void SetFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private IEnumerator CoInitResourceManager()
    {
        _addressableLocation.Clear();
        var count = 0f;
        var total = (float)(eAddressableType.End - 1);
        _totalSize = 0;

        for (var addressableType = eAddressableType.Start + 1; addressableType < eAddressableType.End; addressableType++)
        {
            var result = Addressables.GetDownloadSizeAsync(addressableType.ToString());
            yield return result;
            _totalSize += result.Result;
        }

        if (_totalSize > 0)
        {
            for (var addressableType = eAddressableType.Start + 1; addressableType < eAddressableType.End; addressableType++)
            {
                yield return StartCoroutine(CoSetDownloadDependency(addressableType));
            }
        }

        for (var addressableType = eAddressableType.Start + 1; addressableType < eAddressableType.End; addressableType++)
        {
            yield return StartCoroutine(CoSetAddressableLocation(addressableType));
            yield return null;
        }
    }

    private IEnumerator CoSetAddressableLocation(eAddressableType addressableType)
    {
        var addressableHandle = Addressables.LoadResourceLocationsAsync(addressableType.ToString());
        while (addressableHandle.IsDone == false)
        {
            yield return null;
        }

        var locations = addressableHandle.Result;

        foreach (var location in locations)
        {
            var primaryKey = location.PrimaryKey;
            var split = primaryKey.Split('/');
            primaryKey = split[^1];
            var lastDotIndex = primaryKey.LastIndexOf('.');
            if (lastDotIndex != -1)
            {
                primaryKey = primaryKey.Substring(0, lastDotIndex);
            }

            if (_addressableLocation.ContainsKey(primaryKey))
            {
                continue;
            }

            _addressableLocation.Add(primaryKey, location);
        }

        Addressables.Release(addressableHandle);
        yield return null;
    }

    private IEnumerator CoSetDownloadDependency(eAddressableType addressableType)
    {
        var addressableHandle = Addressables.DownloadDependenciesAsync(addressableType.ToString());
        while (addressableHandle.IsDone == false)
        {
            var status = addressableHandle.GetDownloadStatus();
            var total = status.TotalBytes;
            if (total <= 0) total = 1;
            yield return null;
        }

        Addressables.Release(addressableHandle);
        yield return null;
    }

    public UniTask<T> LoadAssetAsync<T>(string assetName) where T : Object
    {
        if (_addressableLocation.ContainsKey(assetName) == false) return default;
        return AddressablesManager.LoadAssetAsync<T>(_addressableLocation[assetName]);
    }

    public T LoadAssetSync<T>(string assetName) where T : Object
    {
        if (_addressableLocation.ContainsKey(assetName) == false) return default;
        return AddressablesManager.LoadAssetSync<T>(_addressableLocation[assetName]);
    }

    public UniTask<SceneInstance> LoadSceneAsync(string sceneName, LoadSceneMode loadMode = LoadSceneMode.Single,
        bool activateOnLoad = true)
    {
        if (_addressableLocation.ContainsKey(sceneName) == false) return default;
        return AddressablesManager.LoadSceneAsync(_addressableLocation[sceneName], loadMode, activateOnLoad);
    }

    public async UniTask<GameObject> LoadGameObjectAsync(string gameObjectName)
    {
        if (_addressableLocation.ContainsKey(gameObjectName) == false)
        {
            var result = Resources.Load<GameObject>($"RawResources/Prefab/{gameObjectName}");
            return result;
        }

        var prefabGameObject = await AddressablesManager.LoadAssetAsync<GameObject>(_addressableLocation[gameObjectName]);
        var gameObject = PoolManager.Instance.spawnObject(prefabGameObject);
        return gameObject;
    }

    public async UniTask<T> LoadUIPrefabAsync<T>() where T : UIBase
    {
        Debug.Log($"LoadUIPrefabAsync {typeof(T).Name}");
        var uiName = PublicStaticMethod.GetTypeName<T>();
        GameObject prefabGameObject = null;
        if (_addressableLocation.ContainsKey(uiName) == false) prefabGameObject = Resources.Load<GameObject>($"RawResources/Prefab/{uiName}");
        if (prefabGameObject.IsUnityNull()) prefabGameObject = await AddressablesManager.LoadAssetAsync<GameObject>(_addressableLocation[uiName]);
        var gameObject = PoolManager.Instance.spawnObject(prefabGameObject);
        gameObject.SetActive(false);
        var uiComponent = gameObject.GetComponent<T>();
        return uiComponent;
    }
}