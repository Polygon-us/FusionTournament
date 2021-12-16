using UnityEngine;

namespace Attributes
{
    public class Name : PropertyAttribute
    {
        #region Information

        public string name;

        #endregion

        public Name(string name)
        {
            this.name = name;
        }
    }
}
