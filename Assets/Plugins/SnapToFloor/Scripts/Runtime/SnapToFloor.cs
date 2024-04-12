using System;
using UnityEditor;
using UnityEngine;

namespace NKStudio
{
    public partial class SnapToFloor : Editor
    {
        private static MeshFilter[] _meshFiltersToSkip = Array.Empty<MeshFilter>();

        private const int Mesh16BitBufferVertexLimit = 65535;

        private enum ResultType
        {
            X,
            Z
        }

        // 스냅이 허용되는 높이
        private const float Height = 1000f;

        // 경로를 지정하고, true를 해서 안보이게 하자, %는 윈도우는 컨트롤, 맥은 커맨드 키에 해당된다.
        [MenuItem("Edit/SnapToFloor _END")]
        public static void Snap2Surface()
        {
            var mode = EditorSettings.defaultBehaviorMode;

            // Selection은 현재 에디터에서 선택된 오브젝트를 뜻한다.
            foreach (Transform transform in Selection.transforms)
            {
                Undo.RecordObject(transform, "SnapUndoAction");

                if (mode == EditorBehaviorMode.Mode2D)
                    SnapToFloor2D(transform);
                
                if (mode == EditorBehaviorMode.Mode3D)
                    SnapToFloor3D(transform);
            }
        }
    }
}