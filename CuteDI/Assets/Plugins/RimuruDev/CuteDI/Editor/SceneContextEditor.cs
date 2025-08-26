#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AbyssMoth.CuteDI
{
    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : Editor
    {
        private Vector2 scroll;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var ctx = (SceneContext)target;

            if (!Application.isPlaying || !ctx.Built || ctx.Scene == null)
            {
                EditorGUILayout.HelpBox("Запусти сцену, чтобы увидеть реальные биндинги.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Registered Services", EditorStyles.boldLabel);

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(240));
           
            foreach (var info in ctx.Scene.DebugSnapshot())
            {
                var tag = string.IsNullOrEmpty(info.Tag) ? "no-tag" : info.Tag;
               
                EditorGUILayout.LabelField($"{info.ServiceType.Name}   [{tag}]   {(info.IsSingleton ? "Single" : "Transient")}");
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif