using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.AddressableAssets
{
    using ResourceManagement.ResourceProviders;
    using ResourceManagement.ResourceLocations;

    public static partial class AddressablesManager
    {
        private static readonly Dictionary<string, List<IResourceLocation>> _locations;
        private static readonly Dictionary<IResourceLocation, AsyncOperationHandle<SceneInstance>> _sceneAsyncOperationHandles;
        private static readonly Dictionary<IResourceLocation, AsyncOperationHandle<GameObject>> _gameObjectAsyncOperationHandles;
        private static readonly Dictionary<IResourceLocation, AsyncOperationHandle<Object>> _objectAsyncOperationHandles;

        static AddressablesManager()
        {
            _locations = new Dictionary<string, List<IResourceLocation>>();
            _sceneAsyncOperationHandles = new Dictionary<IResourceLocation, AsyncOperationHandle<SceneInstance>>();
            _gameObjectAsyncOperationHandles = new Dictionary<IResourceLocation, AsyncOperationHandle<GameObject>>();
            _objectAsyncOperationHandles = new Dictionary<IResourceLocation, AsyncOperationHandle<Object>>();
        }

        private static void Clear()
        {
            _locations.Clear();
            _sceneAsyncOperationHandles.Clear();
            _gameObjectAsyncOperationHandles.Clear();
            _objectAsyncOperationHandles.Clear();
            
        }

        private static bool IsValidKey(string key, out string result)
        {
            result = key ?? string.Empty;
            return !string.IsNullOrEmpty(key);
        }

        private static bool IsValidKey(AssetReference reference, out string result)
        {
            if (reference == null)
            {
                Debug.LogException(new ArgumentNullException(nameof(reference)));

                result = string.Empty;
            }
            else
            {
                result = reference.RuntimeKey.ToString();
            }

            return !string.IsNullOrEmpty(result);
        }
    }
}