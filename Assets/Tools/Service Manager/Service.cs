using System;
using UnityEngine;

namespace Tools.ServicesManager
{
    [Serializable]
    public class Service
    {
        #region Information

        [SerializeField] private GameObject gameObject;  
        [SerializeField] private Component component;
      
        #endregion

        #region Properties

        public GameObject _GameObject
        {
            get => gameObject;
        }
        public Component _Component
        {
            get => component;
        }

        #endregion

        public Service(GameObject gameObject, Component component)
        {
            this.gameObject = gameObject;

            this.component = component;
        }
    }
}