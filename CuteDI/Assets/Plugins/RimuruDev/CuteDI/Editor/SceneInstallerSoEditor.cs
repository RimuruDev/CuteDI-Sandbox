#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AbyssMoth.CuteDI
{
    [CustomEditor(typeof(SceneInstallerSO), true)]
    public class SceneInstallerSoEditor : Editor
    {
        private bool showPreview = true;
        private Vector2 scroll;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var so = (SceneInstallerSO)target;
          
            showPreview = EditorGUILayout.Foldout(showPreview, "Preview Bindings");
           
            if (!showPreview) 
                return;

            var hints = so.PreviewHints()?.ToList();
            if (hints == null || hints.Count == 0)
            {
                EditorGUILayout.HelpBox("Предпросмотр пуст. Реализуй PreviewHints() для отображения.", MessageType.None);
                return;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(200));
            foreach (var h in hints)
            {
                var tag = string.IsNullOrEmpty(h.Tag) ? "no-tag" : h.Tag;
                EditorGUILayout.LabelField($"{h.Service?.Name}  ←  {h.Implementation?.Name}   [{tag}]   {(h.Singleton ? "Single" : "Transient")}");
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif