using UnityEditor;
using UnityEngine;
using System.Reflection;

// [CustomEditor(typeof(UnityEngine.Object), true)] // This is a custom editor for all Unity objects
public class ConditionalHideEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var so = new SerializedObject(target);

        var script = target;
        var type = script.GetType();

        // Iterate over all the serialized properties
        var property = so.GetIterator();
        while (property.NextVisible(true))
        {
            var field = type.GetField(
                property.name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
            );
            if (field != null)
            {
                var attrs = field.GetCustomAttributes(typeof(ConditionalHideAttribute), false);

                // Draw the property if it doesn't have a ConditionalHideAttribute, or if the condition is met
                if (attrs.Length == 0)
                {
                    EditorGUILayout.PropertyField(property, true);
                }
                else
                {
                    var conditionalHideAttribute = attrs[0] as ConditionalHideAttribute;
                    if (conditionalHideAttribute != null)
                    {
                        var conditionalField = type.GetField(
                            conditionalHideAttribute.ConditionalSourceField
                        );
                        if (conditionalField != null)
                        {
                            var conditionalValue = conditionalField.GetValue(script) as bool?;
                            if (conditionalValue == null || conditionalValue.Value)
                            {
                                EditorGUILayout.PropertyField(property, true);
                            }
                        }
                    }
                }
            }
        }

        so.ApplyModifiedProperties();
    }
}
