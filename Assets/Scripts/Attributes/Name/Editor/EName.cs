using UnityEditor;
using UnityEngine;

namespace Attributes
{
    [CustomPropertyDrawer(typeof(Name))]
    public class EName : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (((Name)attribute).name == null)
            {
                EditorGUI.indentLevel--;
                
                EditorGUI.PropertyField(position, property, GUIContent.none);
            }
            else
                EditorGUI.PropertyField(position, property, new GUIContent(((Name)attribute).name));
        }
    }
}
