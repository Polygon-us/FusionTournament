using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools.ServicesManager
{
    public class ServicesManager : MonoBehaviour
    {
        #region Singleton
        public static ServicesManager instance;
        #endregion

        #region Information

        #if UNITY_EDITOR
        [SerializeField] private bool isExpanded;
        #endif

        [SerializeField] private List<Service> services;

        #endregion

        #region Properties

        private Dictionary<Type, object> _Services { get; set; }

        #endregion

        private void Awake()
        {
            _Services = new Dictionary<Type, object>();

            for (int i = 0; i < services.Count; i++)
            {
                if (services[i]._GameObject != null)
                {
                    if (services[i]._Component != null)
                    {
                        Type type = services[i]._Component.GetType();

                        if (_Services.ContainsKey(type))
                            Debug.Log($"You can't add {type} twice");
                        else
                            _Services.Add(type, services[i]._Component);
                    }
                    else
                        Debug.Log($"{services[i]._GameObject.name} has not type");
                }
                else
                    Debug.Log($"GameObject {i} is empty");
            }

            if (instance == null)
            {
                instance = this;

                if (transform.parent != null)
                    transform.SetParent(null, true);

                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        public void Register<T>(T service, bool permanent = false) where T : Component
        {
            Type type = typeof(T);

            if (_Services.ContainsKey(type))
                Destroy(service.gameObject);
            else
            {
                services.Add(new Service(service.gameObject, service));

                if (permanent)
                {
                    Transform parent = service.gameObject.transform.parent;

                    if (parent == null)
                        service.gameObject.transform.SetParent(gameObject.transform);

                    while (parent != null)
                    {
                        if (parent.parent == null)
                        {
                            parent.SetParent(gameObject.transform);

                            break;
                        }
                        else
                            parent = parent.parent;
                    }
                }

                _Services.Add(type, service);
            }
        }

        public T Get<T>() where T : Component
        {
            if (_Services.TryGetValue(typeof(T), out object service))
                return (T)service;

            return default;
        }

        private void SceneUnloaded(Scene scene)
        {
            for (int i = 0; i < services.Count; i++)
            {
                if (services[i]._GameObject == null)
                {
                    _Services.Remove(services[i]._Component.GetType());

                    services.RemoveAt(i);

                    i--;
                }
            }
        }
    }
}