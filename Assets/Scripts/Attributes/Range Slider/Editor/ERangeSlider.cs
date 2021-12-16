using UnityEditor;
using UnityEngine;

namespace Attributes
{
    [CustomPropertyDrawer(typeof(RangeSlider))]
    public class ERangeSlider : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                position = EditorGUI.PrefixLabel(position, (((RangeSlider)attribute).name == null) ? label : new GUIContent(((RangeSlider)attribute).name));

                float indentSpace = EditorGUI.indentLevel * 15f;

                float x = property.vector2Value.x;

                x = EditorGUI.FloatField(new Rect(-indentSpace + position.x, position.y, indentSpace + 50.9f, position.height), float.Parse(x.ToString("F2")));

                float y = property.vector2Value.y;

                y = EditorGUI.FloatField(new Rect(-indentSpace + position.x + position.width - 50.9f, position.y, indentSpace + 50.9f, position.height), float.Parse(y.ToString("F2")));

                EditorGUI.MinMaxSlider(new Rect(-indentSpace + position.x + 60.9f, position.y, indentSpace + position.width - 121.8f, position.height), ref x, ref y, ((RangeSlider)attribute).min, ((RangeSlider)attribute).max);

                property.vector2Value = new Vector2(x, y);
            }
            else
                EditorGUI.LabelField(position, label.text, "Use range slider with Vector2");
        }
    }
}
