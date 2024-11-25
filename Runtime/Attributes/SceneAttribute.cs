using System;
using UnityEngine;

namespace AlkimeeGames.TagLayerTypeGenerator.Attributes
{
    /// <summary>Apply to any <see cref="string" /> property and it'll be converted into a scene field in the inspector.</summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SceneAttribute : PropertyAttribute
    {
    }
}