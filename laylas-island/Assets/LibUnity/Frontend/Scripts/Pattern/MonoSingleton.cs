/*
 * https://gist.github.com/boscohyun/b649fea1b7d6fb1f7849466cbe1a1f93
 */

using System;
using UnityEngine;

namespace Boscohyun
{
    /// <summary>
    /// usage.
    /// public class MyClassName : Singleton<MyClassName>
    /// {
    ///     protected MyClassName() { }
    ///
    ///     private void Awake()
    ///     {
    ///         if (!IsValidInstance())
    ///         {
    ///             return;
    ///         }
    ///
    ///         // GameObject is alive!
    ///     }
    /// }
    /// </summary>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        // ReSharper disable StaticMemberInGenericType
        private static T _instanceCache;

        private static bool _applicationQuit;

        private static readonly object Lock = new object();
        // ReSharper restore StaticMemberInGenericType

        public static T Instance
        {
            get
            {
                if (_applicationQuit)
                {
                    var message = $"[{nameof(MonoSingleton<T>)}] Application quit. {nameof(Instance)} return null.";
                    Debug.LogWarning(message);
                    return null;
                }

                lock (Lock)
                {
                    if (!(_instanceCache is null))
                    {
                        return _instanceCache;
                    }

                    var type = typeof(T);
                    _instanceCache = (T) FindObjectOfType(type);

                    if (!(_instanceCache is null))
                    {
                        _instanceCache.Initialize();
                        return _instanceCache;
                    }

                    var go = new GameObject(type.Name, type);
                    _instanceCache = go.GetComponent<T>();
                    _instanceCache.Initialize();
                    return _instanceCache;
                }
            }
        }

        protected MonoSingleton()
        {
        }

        private void Initialize()
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[{GetType().Name}] singleton instance initialized.");
        }

        protected bool IsValidInstance()
        {
            if (Instance is null)
            {
                _instanceCache = GetComponent<T>();
                if (_instanceCache is null)
                {
                    throw new Exception($"[{GetType().Name}] Failed to get component {typeof(T).Name}");
                }

                Initialize();
                return true;
            }

            if (Instance.Equals(this))
            {
                return true;
            }

            Destroy(gameObject);
            return false;
        }

        private void OnApplicationQuit()
        {
            _applicationQuit = true;
        }
    }
}
