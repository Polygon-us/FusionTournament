#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Tools.Enum
{
    public class Manager
    {
        [MenuItem("Assets/Create/Enum", false, 100)]
        private static void Create()
        {
            CreationWindow.ShowWindow();
        }

        public static void Create(Parent parent, string name, List<string> entries)
        {   
            if (!Directory.Exists("Assets/Scripts"))
                Directory.CreateDirectory("Assets/Scripts");

            if (!Directory.Exists("Assets/Scripts/Enums"))
                Directory.CreateDirectory("Assets/Scripts/Enums");

            List<string> nameSpaces = new List<string>();

            List<string> script;

            if (parent != null)
            {
                script = new List<string>(File.ReadAllLines(parent.path));

                for (int i = 0; i < script.Count; i++)
                {
                    if (script[i].Contains("namespace"))
                    {
                        for (int j = 0; j < script[i].Length - 8; j++)
                        {
                            if (script[i][j] == 'n' && script[i][j + 1] == 'a' && script[i][j + 2] == 'm' && script[i][j + 3] == 'e' && script[i][j + 4] == 's' && script[i][j + 5] == 'p' && script[i][j + 6] == 'a' && script[i][j + 7] == 'c' && script[i][j + 8] == 'e')
                            {
                                nameSpaces.Add(script[i].Substring(j + 10).Split(' ').First());

                                j += 10 + nameSpaces[nameSpaces.Count - 1].Length;
                            }
                        }
                    }

                    if (script[i].Contains("class " + parent.name))
                    {
                        if(script[i].Contains("private"))
                            script[i] = script[i].Replace("private", "public");

                        if (!script[i].Contains("partial"))
                            script[i] = script[i].Replace("class", "partial class");

                        break;
                    }
                }

                string path = parent.path;

                File.WriteAllLines(path, script);

                AssetDatabase.Refresh();

                script.Clear();
            }
            else
                script = new List<string>();

            string space = "";

            if (parent != null)
            {
                for (int i = 0; i < nameSpaces.Count; i++)
                {
                    script.Add(space + "namespace " + nameSpaces[i]);

                    script.Add(space + "{");

                    space += "    ";
                }

                script.Add(space + "public partial class " + parent.name);

                script.Add(space + "{");

                space += "    ";
            }

            script.Add(space + "public enum " + name);

            script.Add(space + "{");

            space += "    ";

            for (int i = 0; i < entries.Count; i++)
                script.Add(space + entries[i] + ((i < entries.Count - 1) ? "," : ""));

            space = space.Substring(0, space.Length - 4);

            script.Add(space + "}");

            if (parent != null)
            {
                space = space.Substring(0, space.Length - 4);

                script.Add(space + "}");

                for (int i = 0; i < nameSpaces.Count; i++)
                {
                    space = space.Substring(0, space.Length - 4);

                    script.Add(space + "}");
                }

                string path = "Assets/Scripts/Enums/" + string.Join("/", nameSpaces.ToArray()) + "/" + parent.name;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path += "/" + name + ".cs";

                File.WriteAllLines(path, script);
            }
            else
            {
                string path = "Assets/Scripts/Enums/" + name + ".cs";

                File.WriteAllLines(path, script);
            }

            AssetDatabase.Refresh();
        }
    }
}

#endif
