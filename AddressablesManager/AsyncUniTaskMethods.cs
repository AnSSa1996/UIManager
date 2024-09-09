using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace UnityEngine.AddressableAssets
{
    using ResourceManagement.ResourceProviders;
    using AddressableAssets.ResourceLocators;

    public static partial class AddressablesManager
    {
        public static async UniTask<T> LoadAssetAsync<T>(IResourceLocation key) where T : Object
        {
            if (_objectAsyncOperationHandles.ContainsKey(key))
            {
                return _objectAsyncOperationHandles[key].Result as T;
            }

            var operation = Addressables.LoadAssetAsync<T>(key);
            await operation;
            return operation.Result;
        }
        
        public static T LoadAssetSync<T>(IResourceLocation key) where T : Object
        {
            if (_objectAsyncOperationHandles.ContainsKey(key))
            {
                return _objectAsyncOperationHandles[key].Result as T;
            }
            
            var operation = Addressables.LoadAssetAsync<T>(key);
            operation.WaitForCompletion();
            return operation.Result;
        }

        public static async UniTask<SceneInstance> LoadSceneAsync(IResourceLocation key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            
            if (_sceneAsyncOperationHandles.TryGetValue(key, out var loadedScene))
            {
                var scene = loadedScene.Result;
                if (activateOnLoad)
                {
                    var asyncOperation = loadedScene.Result.ActivateAsync();
                    asyncOperation.priority = priority;
                }

                return scene;
            }

            var operation = Addressables.LoadSceneAsync(key, loadMode, activateOnLoad, priority);
            await operation;
            return operation.Result;
        }


        public static async UniTask<SceneInstance> UnloadSceneAsync(IResourceLocation key, bool autoReleaseHandle = true)
        {
            if (_sceneAsyncOperationHandles.TryGetValue(key, out var scene) == false)
            {
                return default;
            }

            var operation = Addressables.UnloadSceneAsync(scene, autoReleaseHandle);
            await operation;
            return operation.Result;
        }
    }
}