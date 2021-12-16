using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Tools.ServicesManager
{
    [CustomEditor(typeof(ServicesManager), true)]
    public class EServicesManager : Editor
    {
        #region Information

        ReorderableList services;

        #endregion

        private void OnEnable()
        {
            services = new ReorderableList(serializedObject, serializedObject.FindProperty("services"), true, false, true, true)
            {
                elementHeight = 22f,

                drawElementCallback = (Rect position, int i, bool isActive, bool isFocused) =>
                {
                    EditorGUI.LabelField(new Rect(position.x, position.y, 54f, position.height), "Service");

                    EditorGUI.PropertyField(new Rect(position.x + 52f, position.y + 3f, (position.width - 52f) / 2f - 2f, position.height), serializedObject.FindProperty("services").GetArrayElementAtIndex(i).FindPropertyRelative("gameObject"), GUIContent.none, true);

                    if (serializedObject.FindProperty("services").GetArrayElementAtIndex(i).FindPropertyRelative("gameObject").objectReferenceValue != null)
                    {
                        List<Component> components = new List<Component>(((GameObject)serializedObject.FindProperty("services").GetArrayElementAtIndex(i).FindPropertyRelative("gameObject").objectReferenceValue).GetComponents<Component>());

                        components.Insert(0, null);

                        int component = 0;

                        for (int j = 0; j < components.Count; j++)
                        {
                            if ((Component)serializedObject.FindProperty("services").GetArrayElementAtIndex(i).FindPropertyRelative("component").objectReferenceValue == components[j])
                            {
                                component = j;

                                break;
                            }
                        }

                        component = EditorGUI.Popup(new Rect(position.x + 52f + ((position.width - 52f) / 2f) + 4f, position.y + 3f, position.width - (52f + ((position.width - 52f) / 2f) + 4f), position.height), component, Array.ConvertAll(components.ToArray(), (Component component) =>
                        {
                            if (component == null)
                                return "None";
                            else
                                return component.GetType().FullName;
                        }));

                        serializedObject.FindProperty("services").GetArrayElementAtIndex(i).FindPropertyRelative("component").objectReferenceValue = components[component];
                    }
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

            GUI.enabled = true;

            EditorGUILayout.Space(6f);

            serializedObject.FindProperty("isExpanded").boolValue = EditorGUILayout.Foldout(serializedObject.FindProperty("isExpanded").boolValue, "Services");

            if (serializedObject.FindProperty("isExpanded").boolValue)
                services.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
