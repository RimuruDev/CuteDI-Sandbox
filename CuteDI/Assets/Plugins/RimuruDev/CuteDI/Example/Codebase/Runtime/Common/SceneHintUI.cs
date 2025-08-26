using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public sealed class SceneHintUI : MonoBehaviour
    {
        private GUIStyle style;

        private void Awake()
        {
            style = new GUIStyle
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
        }

        private void OnGUI()
        {
            var w = Screen.width;
            var h = Screen.height;

            GUI.Label(new Rect(0, h / 2f - 60, w, 40), "Нажми [1] → MainMenu", style);
            GUI.Label(new Rect(0, h / 2f - 20, w, 40), "Нажми [2] → Gameplay", style);
            GUI.Label(new Rect(0, h / 2f + 20, w, 40), "Нажми [3] → Other", style);
        }
    }
}