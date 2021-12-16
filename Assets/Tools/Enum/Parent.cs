using System.Linq;

namespace Tools.Enum
{
    public class Parent
    {
        #region Information

        public string name;
        public string path;

        #endregion

        public Parent(string path)
        {
            name = path.Split('/').Last().Split('.').First();

            this.path = path;
        }
    }
}