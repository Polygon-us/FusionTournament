using UnityEngine;
using UnityEditor;

namespace Attributes
{
    [CustomPropertyDrawer(typeof(Title))]
    public class ETitle : DecoratorDrawer
    {
        private Title _Instance
        {
            get { return ((Title)attribute); }
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUI.LabelField(rect, _Instance.title);

            EditorGUI.DrawRect(new Rect(rect.x, rect.y + 18f, rect.width, 2f), new Color(0.1f, 0.1f, 0.1f, 1f));
        }

        public override float GetHeight()
        {
            return 4f + base.GetHeight();
        }
    }
}