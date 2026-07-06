using UnityEditor;
using UnityEngine;

namespace Editor
{

    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    public class RequireInterfaceDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            var attribute = (RequireInterfaceAttribute)this.attribute;

            EditorGUI.BeginProperty(position, label, property);

            var current = property.objectReferenceValue;

            var picked = EditorGUI.ObjectField(
                position,
                label,
                current,
                typeof(MonoBehaviour),
                true);

            if (picked == null)
            {
                property.objectReferenceValue = null;
            }
            else
            {
                var component = picked as Component;

                if (component != null &&
                    attribute.RequiredType.IsAssignableFrom(component.GetType()))
                {
                    property.objectReferenceValue = component;
                }
                else
                {
                    Debug.LogWarning(
                        $"{picked.name} does not implement {attribute.RequiredType.Name}");
                }
            }

            EditorGUI.EndProperty();
        }
    }
}