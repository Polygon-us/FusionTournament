using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Tools.MultipleParametersEvent
{
    [Serializable]
    public class MultipleParametersAction
    {
        #pragma warning disable CS0414

        #region Information

        [SerializeField] private GameObject gameObject;
        [SerializeReference] private Component component;
        [SerializeField] private string method;
        [SerializeField] private bool isLambda;
        private Action lambdaMethod;     
        [SerializeReference] private UnityEngine.MonoBehaviour parameters;

        #endregion

        #region Properties

        public Action _LambdaMethod
        {
            get => lambdaMethod;
        }

        #endregion

        public MultipleParametersAction () { }

        public MultipleParametersAction(Action lambdaMethod)
        {
            this.lambdaMethod = lambdaMethod;

            isLambda = true;
        }

        public void Invoke()
        {
            if (gameObject != null)
            {
                if (component == null)
                {
                    if (method != "")
                    {
                        if (parameters == null)
                        {
                            MethodInfo method = gameObject.GetType().GetMethods().First(method => method.Name == this.method);

                            method.Invoke(gameObject, null);
                        }
                        else
                        {
                            FieldInfo[] parameters = this.parameters.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                            MethodInfo method = gameObject.GetType().GetMethods().First(method => method.Name == this.method && method.GetParameters().Length == parameters.Length);

                            List<object> values = new List<object>();

                            for (int i = 0; i < parameters.Length; i++)
                                values.Add(parameters[i].GetValue(this.parameters));

                            method.Invoke(gameObject, values.ToArray());
                        }
                    }
                }
                else
                {
                    if (method != "")
                    {
                        if (parameters == null)
                        {
                            MethodInfo method = component.GetType().GetMethods().First(method => method.Name == this.method);

                            method.Invoke(component, null);
                        }
                        else
                        {
                            FieldInfo[] parameters = this.parameters.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                            MethodInfo method = component.GetType().GetMethods().First(method => method.Name == this.method && method.GetParameters().Length == parameters.Length);

                            List<object> values = new List<object>();

                            for (int i = 0; i < parameters.Length; i++)
                                values.Add(parameters[i].GetValue(this.parameters));

                            method.Invoke(component, values.ToArray());
                        }
                    }
                }
            }

            if(lambdaMethod != null)
                lambdaMethod.Invoke();
        }
    }
}
