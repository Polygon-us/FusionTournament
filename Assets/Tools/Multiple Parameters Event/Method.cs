using System.Reflection;
using UnityEngine;

namespace Tools.MultipleParametersEvent
{
    public class Method
    {
        #region Information

        public Component component;
        public MethodInfo methodInfo;

        #endregion

        public Method(Component component, MethodInfo methodInfo)
        {
            this.component = component;

            this.methodInfo = methodInfo;
        }
    }
}
