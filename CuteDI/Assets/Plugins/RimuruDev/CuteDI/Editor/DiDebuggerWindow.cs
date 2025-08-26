#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AbyssMoth.CuteDI
{
    public class DiDebuggerWindow : EditorWindow
    {
        private Vector2 scroll;

        private enum Source
        {
            Project,
            ActiveScene,
            PrefabStage
        }

        private Source source;
        private string filter = "";

        [MenuItem("Tools/CuteDI/DI Debugger")]
        public static void Open() => GetWindow<DiDebuggerWindow>("DI Debugger");

        private void OnGUI()
        {
            source = (Source)EditorGUILayout.EnumPopup("Source", source);
            filter = EditorGUILayout.TextField("Filter", filter);
            
            EditorGUILayout.Space();

            var container = GetContainer();
            if (!Application.isPlaying || container == null)
            {
                EditorGUILayout.HelpBox("Нет доступного контейнера. Запусти Play Mode и проверь источник.", MessageType.Info);
                return;
            }

            var entries = container.DebugLayers().ToList();
            
            if (!string.IsNullOrEmpty(filter))
                entries = entries.Where(e =>
                        (e.Tag ?? "no-tag").ToLower().Contains(filter.ToLower()) ||
                        e.ServiceType.Name.ToLower().Contains(filter.ToLower()) ||
                        e.ServiceType.FullName.ToLower().Contains(filter.ToLower()))
                    .ToList();

            scroll = EditorGUILayout.BeginScrollView(scroll);

            foreach (var group in entries.GroupBy(e => e.Depth).OrderBy(g => g.Key))
            {
                var depth = group.Key;
                var first = group.First();
               
                var title = depth == 0 
                    ? $"Scene Container: {first.ContainerName}" 
                    : depth == 1 
                        ? $"Project Container: {first.ContainerName}" 
                        : $"Ancestor {depth - 1}: {first.ContainerName}";

                DrawSectionHeader(title, depth);
               
                foreach (var e in group.OrderBy(x => x.ServiceType.Name))
                {
                    var tag = string.IsNullOrEmpty(e.Tag) ? "no-tag" : e.Tag;
                    EditorGUILayout.LabelField($"{e.ServiceType.Name}   [{tag}]   {(e.IsSingleton ? "Single" : "Transient")}");
                }

                EditorGUILayout.Space(6);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSectionHeader(string title, int depth)
        {
            var rect = GUILayoutUtility.GetRect(0, 22, GUILayout.ExpandWidth(true));
           
            var col = depth == 0 ? new Color(0.24f, 0.36f, 0.60f, 0.18f)
                : depth == 1 ? new Color(0.22f, 0.60f, 0.30f, 0.18f)
                : new Color(0.60f, 0.50f, 0.22f, 0.18f);
         
            EditorGUI.DrawRect(rect, col);
          
            var rLabel = new Rect(rect.x + 6, rect.y + 2, rect.width - 12, rect.height - 4);
            EditorGUI.LabelField(rLabel, title, EditorStyles.boldLabel);
        }

        private DIContainer GetContainer()
        {
            switch (source)
            {
                case Source.Project:
                    return DiProvider.Project;
               
                case Source.ActiveScene:
                    var s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                    return DiProvider.GetScene(s);
              
                case Source.PrefabStage:
                    var stage = PrefabStageUtility.GetCurrentPrefabStage();
                  
                    if (stage == null) 
                        return null;
                    
                    return DiProvider.GetScene(stage.scene);
            }

            return null;
        }
    }
}
#endif