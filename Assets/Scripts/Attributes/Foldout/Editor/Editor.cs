using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Attributes
{
    [CanEditMultipleObjects]

    [CustomEditor(typeof(Object), true, isFallback = true)]
    public class Editor : UnityEditor.Editor
    {
        #region Information

        List<object> properties;

        #endregion

        void OnEnable()
        {
            if (serializedObject.FindProperty("folders") == null)
                return;

            this.properties = new List<object>();

            DeserializeFolders(this.properties);

            List<object> properties = new List<object>();

            SerializedProperty iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                do
                {
                    AddProperty(properties, iterator);
                }
                while (iterator.NextVisible(false));
            }

            this.properties = properties;

            SerializeFolders(this.properties);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (serializedObject.FindProperty("folders") == null)
                DrawDefaultInspector();
            else
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    if (properties[i] is string)
                    {
                        if ((string)properties[i] == "m_Script")
                            GUI.enabled = false;

                        if ((string)properties[i] == "folders")
                            continue;

                        EditorGUILayout.PropertyField(serializedObject.FindProperty((string)properties[i]), serializedObject.FindProperty((string)properties[i]).isExpanded);

                        if (!GUI.enabled)
                            GUI.enabled = true;
                    }
                    else
                    {
                        EditorGUILayout.Space();

                        GUI.skin.window.padding.top = GUI.skin.window.padding.top - 16;

                        ShowFolder((Folder)properties[i], true);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void AddProperty(List<object> properties, SerializedProperty property)
        {
            Foldout foldout = null;

            object[] attributes = GetPropertyAttributes<PropertyAttribute>(property);

            if (attributes != null)
            {
                foreach (object attribute in attributes)
                {
                    if (attribute is Foldout)
                    {
                        foldout = (Foldout)attribute;

                        break;
                    }
                }
            }

            if (foldout == null)
                properties.Add(property.propertyPath);
            else
            {
                string[] path = foldout.name.Split('/');

                Folder folder = GetFolder(properties, path);

                if (folder == null)
                    CreateFolder(properties, path, property);
                else
                    folder.properties.Add(property.propertyPath);
            }
        }

        object[] GetPropertyAttributes<T>(SerializedProperty property) where T : Attribute
        {
            BindingFlags bindingFlags = BindingFlags.IgnoreCase
            | BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.GetField
            | BindingFlags.GetProperty;

            if (property.serializedObject.targetObject == null)
                return null;

            Type targetType = property.serializedObject.targetObject.GetType();

            FieldInfo field = targetType.GetField(property.name, bindingFlags);

            if (field != null)
                return field.GetCustomAttributes(typeof(T), true);

            return null;
        }

        Folder GetFolder(List<object> properties, string[] path)
        {
            Folder folder = null;

            for (int i = 0; i < path.Length; i++)
            {
                folder = GetFolder((folder == null) ? properties : folder.properties, path[i]);

                if (folder == null)
                    break;
            }

            return folder;
        }

        Folder GetFolder(List<object> properties, string name)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i] is Folder)
                {
                    if (((Folder)properties[i]).name == name)
                        return (Folder)properties[i];
                }
            }

            return null;
        }

        void CreateFolder(List<object> properties, string[] path, SerializedProperty property, int fontSize = 0)
        {
            Folder folder = null;

            for (int i = 0; i < path.Length; i++)
            {
                Folder nexFolder = GetFolder((folder == null) ? properties : folder.properties, path[i]);

                if (nexFolder == null)
                {
                    if (i < path.Length - 1)
                        nexFolder = new Folder(path[i]);
                    else
                        nexFolder = new Folder(path[i], property.propertyPath);

                    Folder oldFolder = GetFolder(this.properties, path.Take(i + 1).ToArray());

                    if (oldFolder != null)
                        nexFolder.isExpanded = oldFolder.isExpanded;

                    if (folder == null)
                        properties.Add(nexFolder);
                    else
                        folder.properties.Add(nexFolder);

                    if (i == path.Length - 1)
                        return;
                }

                folder = nexFolder;
            }
        }

        void ShowFolder(Folder folder, bool isRoot)
        {
            GUILayout.BeginHorizontal();

            if(!isRoot)
                GUILayout.Space(16f);

            EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");

            bool isExpanded = GUILayout.Toggle(folder.isExpanded, folder.name, "foldout");

            if (isExpanded != folder.isExpanded)
            {
                folder.isExpanded = isExpanded;

                SerializeFolders(properties);
            }

            if (folder.isExpanded)
            {
                for (int i = 0; i < folder.properties.Count; i++)
                {
                    if (folder.properties[i] is string)
                    {
                        GUILayout.BeginHorizontal();

                        GUILayout.Space(16f);

                        if (serializedObject.FindProperty((string)folder.properties[i]).propertyType == SerializedPropertyType.Generic)
                        {
                            GUILayout.Space(16f);
                        }

                        EditorGUILayout.PropertyField(serializedObject.FindProperty((string)folder.properties[i]), serializedObject.FindProperty((string)folder.properties[i]).isExpanded);

                        EditorGUILayout.EndHorizontal();
                    }
                    else
                        ShowFolder((Folder)folder.properties[i], false);
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        void SerializeFolders(List<object> properties)
        {
            serializedObject.Update();

            string folders = "[";

            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i] is Folder)
                {
                    folders += ((Folder)properties[i]).Serialize();

                    if (i < properties.Count - 1)
                        folders += ",";
                }
            }

            if (folders[folders.Length - 1] == ',')
                folders = folders.Substring(0, folders.Length - 1);

            folders += "]";

            serializedObject.FindProperty("folders").stringValue = folders;

            serializedObject.ApplyModifiedProperties();
        }

        void DeserializeFolders(List<object> properties)
        {
            string folders = serializedObject.FindProperty("folders").stringValue;

            folders = folders.Substring(1, folders.Length - 2);

            string folder = "";

            int depth = 0;

            for (int i = 0; i < folders.Length; i++)
            {
                switch (folders[i])
                {
                    case '{':

                        depth++;

                        folder += folders[i];

                        break;
                    case ',':

                        if (depth == 0)
                        {
                            properties.Add(folder);

                            folder = "";
                        }
                        else
                            folder += folders[i];

                        break;
                    case '}':

                        depth--;

                        folder += folders[i];

                        break;
                    default:

                        folder += folders[i];

                        break;
                }
            }

            if (folder != "")
                properties.Add(folder);

            for (int i = 0; i < properties.Count; i++)
                properties[i] = new Folder().Deserialize((string)properties[i]);
        }
    }
}
