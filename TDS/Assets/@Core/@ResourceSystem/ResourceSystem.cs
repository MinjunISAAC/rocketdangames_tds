// ----- System
using System;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core
{
    public class ResourceSystem
    {
        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private Dictionary<string, UnityEngine.Object> _globalObjectSet = new Dictionary<string, UnityEngine.Object>();

        // --------------------------------------------------
        // Methods - Normal
        // --------------------------------------------------
        public void Clear()
        {
            foreach (var pair in _globalObjectSet)
                Addressables.Release(pair.Value);

            _globalObjectSet.Clear();
        }

        public T Load<T>(string key) where T : UnityEngine.Object
        {
            if (_globalObjectSet.TryGetValue(key, out UnityEngine.Object resource))
                return resource as T;

            return null;
        }

        public void LoadAsync<T>(string key, Define.ELoadType loadType = Define.ELoadType.Global, Action<T> callback = null) where T : UnityEngine.Object
        {
            if (_globalObjectSet.TryGetValue(key, out UnityEngine.Object resource))
            {
                callback?.Invoke(resource as T);
                return;
            }

            var asyncOperation = Addressables.LoadAssetAsync<T>(key);
            asyncOperation.Completed += (handle) => CheckAddressableDownloadEvent(handle);

            switch (loadType)
            {
                case Define.ELoadType.Global:
                    asyncOperation.Completed += (handle) =>
                    {
                        if (handle.Status == AsyncOperationStatus.Failed)
                        {
                            callback?.Invoke(null);
                            return;
                        }

                        if (!_globalObjectSet.TryAdd(key, handle.Result))
                        {
                            Addressables.Release(handle);
                            if (_globalObjectSet.TryGetValue(key, out UnityEngine.Object existingResource))
                                callback?.Invoke(existingResource as T);
                        }
                        else
                        {
                            callback?.Invoke(handle.Result);
                        }
                    };
                    break;

                default:
                    asyncOperation.Completed += (handle) =>
                    {
                        Addressables.Release(handle);
                        callback?.Invoke(null);
                    };
                    break;
            }
        }

        public void LoadAllAsync<T>(string label, Define.ELoadType loadType = Define.ELoadType.Global, Action<string, int, int> callback = null) where T : UnityEngine.Object
        {
            var asynceOperation = Addressables.LoadResourceLocationsAsync(label, typeof(T));
            asynceOperation.Completed += (handler) =>
            {
                int loadcount = 0;
                int totalCount = handler.Result.Count;

                foreach (var result in handler.Result)
                {
                    LoadAsync<T>(result.PrimaryKey, loadType, (obj) =>
                    {
                        loadcount++;
                        callback?.Invoke(result.PrimaryKey, loadcount, totalCount);
                    });
                }
            };
        }

        private void CheckAddressableDownloadEvent<T>(AsyncOperationHandle<T> handle)
        {
            var downloadStatus = handle.GetDownloadStatus();
            if (downloadStatus.DownloadedBytes > 0)
            {
                Debug.Log($"[Addressable Downloaded Checker] 다운로드 발생: {downloadStatus.DownloadedBytes} 바이트");
                Debug.Log($"Asset: {handle.DebugName}, Status: {handle.Status}, Percent Complete: {handle.PercentComplete * 100}%");
            }
        }

        public GameObject Instantiate(string path, Transform parent = null)
        {
            GameObject prefab = Load<GameObject>($"{path}");
            if (prefab == null)
            {
                Debug.Log($"Failed to load prefab : {path}");
                return null;
            }

            GameObject go = UnityEngine.Object.Instantiate(prefab, parent);
            go.name = prefab.name;
            return go;
        }
    }
}