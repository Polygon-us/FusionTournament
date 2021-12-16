using UnityEngine;

namespace Attributes
{
    public class Title : PropertyAttribute 
    {
        #region Information

        public string title;
        
        #endregion

        public Title(string title)
        {
            this.title = title;
        }
    }
}