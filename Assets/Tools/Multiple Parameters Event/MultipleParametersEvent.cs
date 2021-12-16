using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.MultipleParametersEvent
{
    [Serializable]
    public class MultipleParametersEvent
    {
        #region Information

        [SerializeField] private List<MultipleParametersAction> multipleParamatersActions;

        #endregion

        #region Properties

        public MultipleParametersAction this[int i]
        {
            get => multipleParamatersActions[i];
        }

        public int Count
        {
            get => multipleParamatersActions.Count;
        }

        #endregion

        public int Add(Action action)
        {
            multipleParamatersActions.Add(new MultipleParametersAction(action));

            return multipleParamatersActions.Count - 1;
        }

        public void Remove(Action action)
        {
            for (int i = 0; i < multipleParamatersActions.Count; i++)
            {
                if (multipleParamatersActions[i]._LambdaMethod == action)
                {
                    multipleParamatersActions.RemoveAt(i);

                    break;
                }
            }
        }

        public void Clear()
        {
            multipleParamatersActions.Clear();
        }

        public void Invoke()
        {
            foreach (MultipleParametersAction multipleParamatersAction in multipleParamatersActions)
                multipleParamatersAction.Invoke();
        }
    }
}
