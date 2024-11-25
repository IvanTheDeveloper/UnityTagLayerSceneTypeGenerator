using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace AlkimeeGames.TagLayerTypeGenerator.Editor.Sync
{
    /// <summary>Checks for updates to the Scenes in the Project.</summary>
    internal sealed class SceneSync : ISync
    {
        /// <summary>Used to check if what scene strings and IDs are in the Scene type.</summary>
        private readonly HashSet<ValueTuple<string, int>> _inType = new HashSet<ValueTuple<string, int>>();

        /// <summary>Used to check if what scene strings and IDs are in the project.</summary>
        private readonly HashSet<ValueTuple<string, int>> _inUnity = new HashSet<ValueTuple<string, int>>();

        /// <inheritdoc />
        public bool IsInSync([NotNull] Type generatingType)
        {
            if (generatingType == null) throw new ArgumentNullException(nameof(generatingType));

            _inUnity.Clear();

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();

            foreach (EditorBuildSettingsScene scene in scenes)
            {
                int sceneBuildIndex = Array.IndexOf(scenes, scene);
                string safeName = Path.GetFileNameWithoutExtension(scene.path).Replace(" ", string.Empty);
                _inUnity.Add(new ValueTuple<string, int>(safeName, sceneBuildIndex));
            }

            _inType.Clear();

            FieldInfo[] fields = generatingType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fieldInfo in fields)
                if (fieldInfo.IsLiteral && fieldInfo.FieldType == typeof(int))
                    _inType.Add(new ValueTuple<string, int>(fieldInfo.Name, (int)fieldInfo.GetValue(null)));

            return _inType.SetEquals(_inUnity);
        }
    }
}