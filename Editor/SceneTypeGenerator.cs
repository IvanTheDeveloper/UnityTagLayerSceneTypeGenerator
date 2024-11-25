using System;
using System.CodeDom;
using System.Reflection;
using AlkimeeGames.TagLayerTypeGenerator.Editor.Settings;
using AlkimeeGames.TagLayerTypeGenerator.Editor.Sync;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static System.String;
using System.Linq;
using System.IO;

namespace AlkimeeGames.TagLayerTypeGenerator.Editor
{
    /// <summary>Generates a file containing a type; which contains constant int definitions for each Scene in the project.</summary>
    public sealed class SceneTypeGenerator : TypeGenerator<SceneTypeGenerator>
    {
        /// <inheritdoc />
        private SceneTypeGenerator([NotNull] TypeGeneratorSettings.Settings settings, ISync sync) : base(settings, sync)
        {
        }

        /// <summary>Runs when the Editor starts or on a domain reload.</summary>
        [InitializeOnLoadMethod]
        public static void InitializeOnLoad() => new SceneTypeGenerator(TypeGeneratorSettings.GetOrCreateSettings.Scene, new SceneSync());

        /// <summary>Creates members for each scene in the project and adds them to the <paramref name="sceneType" /> along with a nested type called "BuildIndex".</summary>
        /// <param name="sceneType">The <see cref="CodeTypeDeclaration" /> to add the scene ID's to.</param>
        protected override void CreateMembers(CodeTypeDeclaration sceneType)
        {
            // Make a nested type for the Scene Build Index
            var buildIndexType = new CodeTypeDeclaration("BuildIndex") { IsClass = true, TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed };
            sceneType.Members.Add(buildIndexType);

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();

            foreach (EditorBuildSettingsScene scene in scenes)
            {
                string sceneBuildIndex = Array.IndexOf(scenes, scene).ToString();
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                string safeName = sceneName.Replace(" ", string.Empty);
                const MemberAttributes attributes = MemberAttributes.Public | MemberAttributes.Const;

                AddSceneField(sceneType, safeName, attributes, sceneName);
                AddSceneBuildIndexField(buildIndexType, safeName, attributes, sceneBuildIndex);
            }

            AddCommentsToSceneType(sceneType);
            AddCommentsToSceneBuildIndexType(buildIndexType);
        }

        private static void AddSceneField([NotNull] CodeTypeDeclaration sceneType, [NotNull] string safeName, MemberAttributes attributes, [NotNull] string sceneName)
        {
            var sceneField = new CodeMemberField(typeof(string), safeName) { Attributes = attributes, InitExpression = new CodePrimitiveExpression(sceneName) };
            ValidateIdentifier(sceneField, sceneName);

            sceneType.Members.Add(sceneField);
        }

        private static void AddSceneBuildIndexField([NotNull] CodeTypeDeclaration buildIndexType, [NotNull] string safeName, MemberAttributes attributes, [NotNull] string sceneBuildIndex)
        {
            var buildIndexField = new CodeMemberField(typeof(int), safeName) { Attributes = attributes, InitExpression = new CodePrimitiveExpression(int.Parse(sceneBuildIndex)) };
            ValidateIdentifier(buildIndexField, sceneBuildIndex);

            buildIndexType.Members.Add(buildIndexField);
        }

        /// <summary>Adds a verbose comment on how to use the Scene enum.</summary>
        /// <param name="typeDeclaration">The <see cref="CodeTypeDeclaration" /> to add the comment to.</param>
        private void AddCommentsToSceneType([NotNull] CodeTypeMember typeDeclaration)
        {
            var commentStatement = new CodeCommentStatement(
                "<summary>\r\n Use this type in place of scene names in code / scripts.\r\n </summary>" +
                $"\r\n <example>\r\n <code>\r\n SceneManager.LoadScene({Settings.TypeName}.SampleScene); \r\n </code>\r\n </example>",
                true);

            typeDeclaration.Comments.Add(commentStatement);
        }

        /// <summary>Adds a verbose comment on how to use the Scene.BuildIndex type.</summary>
        /// <param name="typeDeclaration">The <see cref="CodeTypeDeclaration" /> to add the comment to.</param>
        private void AddCommentsToSceneBuildIndexType([NotNull] CodeTypeMember typeDeclaration)
        {
            var commentStatement = new CodeCommentStatement(
                "<summary>\r\n Use this type in place of scene names in code / scripts.\r\n </summary>" +
                $"\r\n <example>\r\n <code>\r\n SceneManager.LoadScene({Settings.TypeName}.BuildIndex.SampleScene + 1); \r\n </code>\r\n </example>",
                true);

            typeDeclaration.Comments.Add(commentStatement);
        }
    }
}