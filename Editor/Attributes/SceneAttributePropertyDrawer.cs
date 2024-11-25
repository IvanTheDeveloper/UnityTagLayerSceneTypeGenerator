using AlkimeeGames.TagLayerTypeGenerator.Attributes;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace AlkimeeGames.TagLayerTypeGenerator.Editor.Attributes
{
    /// <summary>Converts a <see cref="string" /> property into a dropdown to select a scene from the project.</summary>
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    internal sealed class SceneAttributePropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc />
        public override void OnGUI(Rect position, [NotNull] SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                // Get all scenes in the build settings
                var scenes = EditorBuildSettings.scenes;
                var sceneNames = new string[scenes.Length];
                for (int i = 0; i < scenes.Length; i++)
                {
                    sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
                }

                // Find the current index of the property value in the scene list
                int currentIndex = System.Array.IndexOf(sceneNames, property.stringValue);
                if (currentIndex < 0) currentIndex = 0; // Default to the first scene if not found

                // Show a popup to select a scene
                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, sceneNames);

                // Update the property value with the selected scene name
                property.stringValue = sceneNames.Length > 0 ? sceneNames[selectedIndex] : string.Empty;
            }

            EditorGUI.EndProperty();
        }
    }
}