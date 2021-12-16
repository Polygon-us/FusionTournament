#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools.Enum
{
    public class CreationWindow : EditorWindow
    {
        #pragma warning disable CS0414

        #region Information

        #region Instance
        private static CreationWindow instance;
        #endregion

        #region Window
        private Vector2 windowSize = new Vector2(384f, 0f);
        private float padding = 0.1f;
        private new Rect position;
        private Vector2 fieldSize = new Vector2(0, 18f);
        #endregion

        #region Parent
        private List<string> parents = new List<string>();
        private int parent = 0;
        #endregion

        #region Enum
        private new string name = "";
        private List<string> entries = new List<string>();
        private ReorderableList reorderableList;
        #endregion

        #endregion

        public static void ShowWindow()
        {
            GetWindow<CreationWindow>(true, "Enum");
        }

        private void OnEnable()
        {
            instance = this;

            fieldSize = new Vector2(windowSize.x - (2f * (windowSize.x * padding)), fieldSize.y);

            parents = new List<string>(Array.ConvertAll(AssetDatabase.FindAssets("t:Script", new[] { "Assets" }), AssetDatabase.GUIDToAssetPath));

            for (int i = 0; i < parents.Count; i++)
                parents[i] = parents[i].Substring("Assets/".Length);

            parents.Insert(0, "null");

            reorderableList = new ReorderableList(entries, typeof(string), true, false, true, true)
            {
                elementHeight = fieldSize.y,

                drawElementCallback = (Rect position, int i, bool isactive, bool isfocused) =>
                {
                    string entry = EditorGUI.TextField(position, entries[i]);

                    if (entry != "")
                    {
                        if (entry != entries[i])
                        {
                            if (entry.All(c => char.IsLetter(c)))
                                entries[i] = entry;
                            else
                                entries[i] = "";
                        }
                    }
                    else
                        entries[i] = "";
                }
            };
        }

        private void OnDisable()
        {
            instance = null;
        }

        private void OnGUI()
        {
            float y = (windowSize.x * padding / 2f);

            position = SetPosition(y);

            EditorGUI.LabelField(position, "Parent");

            position = SetPosition(y += fieldSize.y);

            parent = EditorGUI.Popup(position, parent, parents.ToArray());

            position = SetPosition(y += fieldSize.y);

            EditorGUI.LabelField(position, "Name");

            position = SetPosition(y += fieldSize.y);

            string name = EditorGUI.TextField(position, this.name);

            if (name != "")
            {
                if (name != this.name)
                {
                    if (name.All(c => char.IsLetter(c)))
                        this.name = name;
                    else
                        this.name = "";
                }
            }
            else
                this.name = "";

            position = SetPosition(y += fieldSize.y + fieldSize.y);

            EditorGUI.LabelField(position, "Entries");

            position = SetPosition(y += fieldSize.y);

            reorderableList.DoList(position);

            y += reorderableList.GetHeight();

            padding = 0.4f;

            position = SetPosition(y += fieldSize.y);

            if (GUI.Button(new Rect(position.x, position.y, base.position.width * 0.2f, 16f), "Create"))
            {
                if (this.name == "")
                {
                    Debug.LogError("There can be no empty name");

                    return;
                }

                string entry = "";

                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i] == "")
                    {
                        Debug.LogError("There can be no empty entries");

                        return;
                    }

                    if (entries[i] != entry)
                        entry = entries[i];
                    else
                    {
                        Debug.LogError("There can be not the same entries");

                        return;
                    }
                }

                Manager.Create((parent == 0) ? null : new Parent("Assets/" + parents[parent]), this.name, entries);

                instance.Close();
                
                return;
            }

            padding = 0.1f;

            instance.minSize = new Vector2(windowSize.x, windowSize.y + fieldSize.y + ((windowSize.x * padding) / 2f));

            instance.maxSize = new Vector2(windowSize.x, windowSize.y + fieldSize.y + ((windowSize.x * padding) / 2f));
        }

        Rect SetPosition(float y)
        {
            windowSize = new Vector2(windowSize.x, y);

            return new Rect(windowSize.x * padding, y, fieldSize.x, fieldSize.y);
        }
    }
}

#endif
