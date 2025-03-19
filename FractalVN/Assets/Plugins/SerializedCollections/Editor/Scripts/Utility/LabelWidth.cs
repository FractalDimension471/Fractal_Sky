using System;
using UnityEditor;

namespace AYellowpaper.SerializedCollections.Editor
{
    public struct LabelWidth : IDisposable
    {
        public float PreviousWidth { get; }

        public LabelWidth(float width)
        {
            PreviousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = PreviousWidth;
        }
    }
}