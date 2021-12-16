using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tools.MultipleParametersEvent
{
    [CustomPropertyDrawer(typeof(MultipleParametersEvent), true)]
    public class EMultipleParametersEvent : PropertyDrawer
    {
        #region Information

        ReorderableList reorderableList;

        #endregion

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            reorderableList.DoList(EditorGUI.IndentedRect(position));

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (reorderableList == null)
                reorderableList = new ReorderableList(property.serializedObject, property.FindPropertyRelative("multipleParamatersActions"))
                {
                    drawHeaderCallback = (Rect position) =>
                    {
                        EditorGUI.LabelField(position, label);
                    },

                    elementHeightCallback = (int i) =>
                    {
                        int height = 20;

                        if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue != null)
                        {
                            SerializedObject parameters = new SerializedObject(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue);

                            SerializedProperty iterator = parameters.GetIterator();

                            if (iterator.NextVisible(true))
                            {
                                do
                                {
                                    height += (int)EditorGUI.GetPropertyHeight(iterator, iterator.isExpanded);
                                }
                                while (iterator.NextVisible(false));
                            }
                        }

                        return height;
                    },

                    drawElementCallback = (Rect position, int i, bool isActive, bool isFocused) =>
                    {
                        if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("isLambda").boolValue)
                        {
                            EditorGUI.LabelField(new Rect(position.x, position.y + 2f, position.width / 2f - 8f, 16f), "Lambda Method");

                            return;
                        }

                        EditorGUI.BeginChangeCheck();

                        property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("gameObject").objectReferenceValue = (GameObject)EditorGUI.ObjectField(new Rect(position.x, position.y + 2f, position.width / 2f - 8f, 16f), property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("gameObject").objectReferenceValue, typeof(GameObject), true);

                        if (EditorGUI.EndChangeCheck())
                        {
                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").objectReferenceValue = null;

                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").serializedObject.ApplyModifiedProperties();

                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("method").stringValue = "";

                            if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue != null)
                            {
                                UnityEngine.Object.DestroyImmediate(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue);

                                property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue = null;

                                property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").serializedObject.ApplyModifiedProperties();
                            }

                            return;
                        }

                        if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("gameObject").objectReferenceValue == null)
                        {
                            GUI.enabled = false;

                            List<string> components = new List<string>()
                            {
                                "None"
                            };

                            EditorGUI.Popup(new Rect(position.x + position.width / 2f + 8f, position.y + 1f, position.width / 2f - 8f, 16f), 0, components.ToArray());
                        }
                        else
                        {
                            List<Method> methods = GetMethods(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("gameObject"));

                            int methodIndex = 0;

                            for (int j = 1; j < methods.Count; j++)
                            {
                                if ((Component)property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").objectReferenceValue == methods[j].component
                                    &&
                                    property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("method").stringValue == methods[j].methodInfo.Name)
                                {
                                    methodIndex = j;

                                    break;
                                }
                            }

                            EditorGUI.BeginChangeCheck();

                            methodIndex = EditorGUI.Popup(new Rect(position.x + position.width / 2f + 8f, position.y + 1f, position.width / 2f - 8f, 16f), methodIndex, Array.ConvertAll(methods.ToArray(), (Method method) =>
                            {
                                if (method == null)
                                    return "None";
                                else
                                {
                                    if (method.component == null)
                                        return "UnityEngine.GameObject/" + method.methodInfo.Name;
                                    else
                                        return method.component.GetType().FullName + "/" + method.methodInfo.Name;
                                }
                            }));

                            if (EditorGUI.EndChangeCheck())
                            {
                                if (methodIndex == 0)
                                {
                                    property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").objectReferenceValue = null;

                                    property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").serializedObject.ApplyModifiedProperties();

                                    property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("method").stringValue = "";

                                    if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue != null)
                                    {
                                        UnityEngine.Object.DestroyImmediate(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue);

                                        property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue = null;

                                        property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").serializedObject.ApplyModifiedProperties();
                                    }
                                }
                                else
                                {
                                    Method method = methods[methodIndex];

                                    property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").objectReferenceValue = method.component;

                                    property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").serializedObject.ApplyModifiedProperties();

                                    MethodInfo methodInfo = method.methodInfo;

                                    property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("method").stringValue = methodInfo.Name;

                                    ParameterInfo[] parametersInfo = methodInfo.GetParameters();

                                    string name = "";

                                    for (int j = 0; j < parametersInfo.Length; j++)
                                    {
                                        if (parametersInfo[j].ParameterType.IsArray)
                                        {
                                            name += "Array";

                                            string[] parameter = parametersInfo[j].ParameterType.GetElementType().FullName.Replace("+", ".").Split('.');

                                            for (int k = 0; k < parameter.Length; k++)
                                                name += parameter[k];
                                        }
                                        else
                                        {
                                            string[] parameter = parametersInfo[j].ParameterType.FullName.Replace("+", ".").Split('.');

                                            for (int k = 0; k < parameter.Length; k++)
                                                name += parameter[k];
                                        }

                                        if (j < parametersInfo.Length - 1)
                                            name += "And";
                                    }

                                    if (name != "")
                                    {
                                        if (!File.Exists("Assets/Scripts/Multiple Paramaters Actions/" + name + ".cs"))
                                        {
                                            if (!Directory.Exists("Assets/Scripts"))
                                                Directory.CreateDirectory("Assets/Scripts");

                                            if (!Directory.Exists("Assets/Scripts/Multiple Paramaters Actions"))
                                                Directory.CreateDirectory("Assets/Scripts/Multiple Paramaters Actions");

                                            StreamWriter writer = new StreamWriter("Assets/Scripts/Multiple Paramaters Actions/" + name + ".cs");

                                            writer.WriteLine("using System;");
                                            writer.WriteLine("using UnityEngine;");
                                            writer.WriteLine("");

                                            writer.WriteLine("namespace Tools.MultipleParametersEvent");
                                            writer.WriteLine("{");
                                            writer.WriteLine("    public class " + name + " : UnityEngine.MonoBehaviour");
                                            writer.WriteLine("    {");
                                            writer.WriteLine("        #region Information");
                                            writer.WriteLine("");

                                            for (int j = 0; j < parametersInfo.Length; j++)
                                            {
                                                if (parametersInfo[j].ParameterType.IsArray)
                                                    writer.WriteLine("        [SerializeField] " + parametersInfo[j].ParameterType.GetElementType().FullName.Replace("+", ".") + "[]" + " " + ConvertToLetter(j) + ";");
                                                else
                                                    writer.WriteLine("        [SerializeField] " + parametersInfo[j].ParameterType.FullName.Replace("+", ".") + " " + ConvertToLetter(j) + ";");
                                            }

                                            writer.WriteLine("");
                                            writer.WriteLine("        #endregion");
                                            writer.WriteLine("    }");
                                            writer.WriteLine("}");

                                            writer.Close();

                                            AssetDatabase.SaveAssets();

                                            AssetDatabase.Refresh();

                                            return;
                                        }
                                    }
                                }
                            }

                            if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("method").stringValue != "")
                            {
                                Method method = methods[methodIndex];

                                MethodInfo methodInfo = method.methodInfo;

                                if (methodInfo.GetParameters().Length == 0)
                                {
                                    if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue != null)
                                    {
                                        UnityEngine.Object.DestroyImmediate(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue);

                                        property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue = null;

                                        property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").serializedObject.ApplyModifiedProperties();
                                    }

                                    return;
                                }
                                else
                                {
                                    ParameterInfo[] parametersInfo = methodInfo.GetParameters();

                                    string name = "";

                                    for (int j = 0; j < parametersInfo.Length; j++)
                                    {
                                        if (parametersInfo[j].ParameterType.IsArray)
                                        {
                                            name += "Array";

                                            string[] parameter = parametersInfo[j].ParameterType.GetElementType().FullName.Replace("+", ".").Split('.');

                                            for (int k = 0; k < parameter.Length; k++)
                                                name += parameter[k];
                                        }
                                        else
                                        {
                                            string[] parameter = parametersInfo[j].ParameterType.FullName.Replace("+", ".").Split('.');

                                            for (int k = 0; k < parameter.Length; k++)
                                                name += parameter[k];
                                        }

                                        if (j < parametersInfo.Length - 1)
                                            name += "And";
                                    }

                                    if (property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue == null)
                                    {
                                        Type classType = ((MonoScript)AssetDatabase.LoadMainAssetAtPath("Assets/Scripts/Multiple Paramaters Actions/" + name + ".cs")).GetClass();

                                        if (classType == null)
                                            return;
                                        else
                                        {
                                            UnityEngine.MonoBehaviour monobehaviour = (UnityEngine.MonoBehaviour)((Component)property.serializedObject.targetObject).gameObject.AddComponent(classType.GetTypeInfo().UnderlyingSystemType);

                                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue = monobehaviour;

                                            monobehaviour.hideFlags = HideFlags.HideInInspector | HideFlags.DontUnloadUnusedAsset;

                                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").serializedObject.ApplyModifiedProperties();
                                        }
                                    }
                                    else
                                    {
                                        if (name != property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue.GetType().Name)
                                        {
                                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").objectReferenceValue = method.component;

                                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("component").serializedObject.ApplyModifiedProperties();

                                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("method").stringValue = methodInfo.Name;

                                            UnityEngine.Object.DestroyImmediate(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue);

                                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue = null;

                                            property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").serializedObject.ApplyModifiedProperties();

                                            if (!File.Exists("Assets/Scripts/Multiple Paramaters Actions/" + name + ".cs"))
                                            {
                                                if (!Directory.Exists("Assets/Scripts"))
                                                    Directory.CreateDirectory("Assets/Scripts");

                                                if (!Directory.Exists("Assets/Scripts/Multiple Paramaters Actions"))
                                                    Directory.CreateDirectory("Assets/Scripts/Multiple Paramaters Actions");

                                                StreamWriter writer = new StreamWriter("Assets/Scripts/Multiple Paramaters Actions/" + name + ".cs");

                                                writer.WriteLine("using System;");
                                                writer.WriteLine("using UnityEngine;");
                                                writer.WriteLine("");

                                                writer.WriteLine("namespace Tools.MultipleParametersEvent");
                                                writer.WriteLine("{");
                                                writer.WriteLine("    public class " + name + " : UnityEngine.MonoBehaviour");
                                                writer.WriteLine("    {");
                                                writer.WriteLine("        #region Information");
                                                writer.WriteLine("");

                                                for (int j = 0; j < parametersInfo.Length; j++)
                                                {
                                                    if (parametersInfo[j].ParameterType.IsArray)
                                                        writer.WriteLine("        [SerializeField] " + parametersInfo[j].ParameterType.GetElementType().FullName.Replace("+", ".") + "[]" + " " + ConvertToLetter(j) + ";");
                                                    else
                                                        writer.WriteLine("        [SerializeField] " + parametersInfo[j].ParameterType.FullName.Replace("+", ".") + " " + ConvertToLetter(j) + ";");
                                                }

                                                writer.WriteLine("");
                                                writer.WriteLine("        #endregion");
                                                writer.WriteLine("    }");
                                                writer.WriteLine("}");

                                                writer.Close();

                                                AssetDatabase.SaveAssets();

                                                AssetDatabase.Refresh();
                                            }

                                            return;
                                        }
                                        else
                                        {
                                            EditorGUI.LabelField(new Rect(position.x, position.y + 22f, position.width, 16f), "Parameters", EditorStyles.boldLabel);

                                            SerializedObject parameters = new SerializedObject(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(i).FindPropertyRelative("parameters").objectReferenceValue);

                                            int y = 38;

                                            for (int j = 0; j < parametersInfo.Length; j++)
                                            {
                                                EditorGUI.PropertyField(new Rect(position.x + (((parameters.FindProperty(ConvertToLetter(j))).propertyType == SerializedPropertyType.Generic) ? 16 : 0), position.y + y, position.width - (((parameters.FindProperty(ConvertToLetter(j))).propertyType == SerializedPropertyType.Generic) ? 16 : 0), 16f), parameters.FindProperty(ConvertToLetter(j)), new GUIContent(parametersInfo[j].Name), parameters.FindProperty(ConvertToLetter(j)).isExpanded);

                                                y += (int)EditorGUI.GetPropertyHeight(parameters.FindProperty(ConvertToLetter(j)));
                                            }

                                            parameters.ApplyModifiedProperties();
                                        }
                                    }
                                }
                            }
                        }

                        if(!GUI.enabled)
                            GUI.enabled = true;
                    },

                    onAddCallback = (ReorderableList list) =>
                    {
                        property.FindPropertyRelative("multipleParamatersActions").arraySize++;

                        SerializedProperty multipleParamatersAction = property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(property.FindPropertyRelative("multipleParamatersActions").arraySize - 1);

                        multipleParamatersAction.FindPropertyRelative("gameObject").objectReferenceValue = null;

                        multipleParamatersAction.FindPropertyRelative("method").stringValue = "";

                        multipleParamatersAction.FindPropertyRelative("parameters").objectReferenceValue = null;
                    },

                    onRemoveCallback = (ReorderableList list) =>
                    {
                        if(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(list.index).FindPropertyRelative("parameters").objectReferenceValue != null)
                            UnityEngine.Object.DestroyImmediate(property.FindPropertyRelative("multipleParamatersActions").GetArrayElementAtIndex(list.index).FindPropertyRelative("parameters").objectReferenceValue);

                        property.FindPropertyRelative("multipleParamatersActions").DeleteArrayElementAtIndex(list.index);
                    }
                };

            return base.GetPropertyHeight(property, label) + reorderableList.GetHeight() - 16f;
        }

        public List<Method> GetMethods(SerializedProperty property)
        {
            List<Method> methods = new List<Method>();

            MethodInfo[] methodsInfo = ((GameObject)property.objectReferenceValue).GetType().GetMethods();

            for (int i = 0; i < methodsInfo.Length; i++)
            {
                if (methodsInfo[i].ReturnType == typeof(void))
                {
                    ParameterInfo[] parameters = methodsInfo[i].GetParameters();

                    bool found = false;

                    for (int j = 0; j < parameters.Length; j++)
                    {
                        if (parameters[j].ParameterType == typeof(Type)
                            || 
                            parameters[j].ParameterType == typeof(object)
                            ||                          
                            parameters[j].ParameterType.IsArray
                            ||
                            parameters[j].ParameterType.IsConstructedGenericType)
                        {
                            found = true;

                            break;
                        }
                    }

                    if(!found)
                        methods.Add(new Method(null, methodsInfo[i]));
                }
            }

            List<Component> components = new List<Component>(((GameObject)property.objectReferenceValue).GetComponents<Component>());

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].hideFlags.HasFlag(HideFlags.HideInInspector))
                    continue;
                else
                {
                    methodsInfo = components[i].GetType().GetMethods();

                    for (int j = 0; j < methodsInfo.Length; j++)
                    {
                        if (methodsInfo[j].ReturnType == typeof(void))
                        {
                            ParameterInfo[] parameters = methodsInfo[j].GetParameters();

                            bool found = false;

                            for (int k = 0; k < parameters.Length; k++)
                            {
                                if (parameters[k].ParameterType == typeof(Type)
                                    ||
                                    parameters[k].ParameterType == typeof(object)
                                    ||
                                    ((parameters[k].ParameterType.IsArray) ? parameters[k].ParameterType.GetElementType().IsArray : false)
                                    ||
                                    parameters[k].ParameterType.IsConstructedGenericType)
                                {
                                    found = true;

                                    break;
                                }
                            }

                            if(!found)
                                methods.Add(new Method(components[i], methodsInfo[j]));
                        }
                    }
                }
            }

            methods.Insert(0, null);

            return methods;
        }

        string ConvertToLetter(int index)
        {
            return char.ConvertFromUtf32(index + 97);
        }
    }
}
