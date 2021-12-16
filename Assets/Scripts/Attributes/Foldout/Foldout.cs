using UnityEngine;

namespace Attributes
{
    public partial class Foldout: PropertyAttribute
    {
        #region Information

        public string name;

        #endregion

        public Foldout(string name)
        {
            this.name = name;
        }
    }
}
