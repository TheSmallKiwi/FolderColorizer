using UnityEditor;
using UnityEngine;

namespace FolderColorizer
{
    /// <summary>
    /// Editor window for configuring automatic folder colorization.
    /// </summary>
    public class FolderColorizerWindow : EditorWindow
    {
        [MenuItem("Window/Folder Colorizer")]
        public static void ShowWindow()
        {
            GetWindow<FolderColorizerWindow>("Folder Colorizer");
        }

        private void OnGUI()
        {
            var settings = FolderColorizerSettings.Instance;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("Could not load or create FolderColorizerSettings.", MessageType.Error);
                return;
            }

            DrawHeader();
            EditorGUILayout.Space();

            DrawSettings(settings);
            EditorGUILayout.Space();

            DrawPreview(settings);
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Folder Colorizer", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Automatically assigns rainbow colors to folders.", EditorStyles.miniLabel);
        }

        private void DrawSettings(FolderColorizerSettings settings)
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            // Enabled toggle
            EditorGUI.BeginChangeCheck();
            bool enabled = EditorGUILayout.Toggle("Enabled", settings.Enabled);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetEnabled(enabled);
            }

            EditorGUI.BeginDisabledGroup(!settings.Enabled);

            // Background alpha
            EditorGUI.BeginChangeCheck();
            float alpha = EditorGUILayout.Slider("Background Opacity", settings.BackgroundAlpha, 0.1f, 0.5f);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetBackgroundAlpha(alpha);
            }

            // Saturation
            EditorGUI.BeginChangeCheck();
            float saturation = EditorGUILayout.Slider("Saturation", settings.Saturation, 0.2f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetSaturation(saturation);
            }

            // Brightness
            EditorGUI.BeginChangeCheck();
            float brightness = EditorGUILayout.Slider("Brightness", settings.Brightness, 0.5f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetBrightness(brightness);
            }

            // Colorize children
            EditorGUI.BeginChangeCheck();
            bool colorizeChildren = EditorGUILayout.Toggle("Colorize Children", settings.ColorizeChildren);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetColorizeChildren(colorizeChildren);
            }

            // Min depth
            EditorGUI.BeginChangeCheck();
            int minDepth = EditorGUILayout.IntSlider("Min Folder Depth", settings.MinDepth, 0, 5);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetMinDepth(minDepth);
            }

            EditorGUILayout.HelpBox(
                "Depth 0 = Assets\n" +
                "Depth 1 = Assets/*\n" +
                "Depth 2 = Assets/*/*\n" +
                "Folders below min depth won't be colored.",
                MessageType.Info
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Text Colors", EditorStyles.boldLabel);

            // Normal state colors
            EditorGUI.BeginChangeCheck();
            Color outlineColor = EditorGUILayout.ColorField("Outline Color", settings.OutlineColor);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetOutlineColor(outlineColor);
            }

            EditorGUI.BeginChangeCheck();
            Color textColor = EditorGUILayout.ColorField("Text Color", settings.TextColor);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetTextColor(textColor);
            }

            // Hover state colors
            EditorGUI.BeginChangeCheck();
            Color hoverOutlineColor = EditorGUILayout.ColorField("Hover Outline Color", settings.HoverOutlineColor);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetHoverOutlineColor(hoverOutlineColor);
            }

            EditorGUI.BeginChangeCheck();
            Color hoverTextColor = EditorGUILayout.ColorField("Hover Text Color", settings.HoverTextColor);
            if (EditorGUI.EndChangeCheck())
            {
                settings.SetHoverTextColor(hoverTextColor);
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawPreview(FolderColorizerSettings settings)
        {
            EditorGUILayout.LabelField("Color Preview", EditorStyles.boldLabel);

            if (!settings.Enabled)
            {
                EditorGUILayout.HelpBox("Enable colorization to see preview.", MessageType.Info);
                return;
            }

            // Show sample colors for common folder names
            string[] sampleFolders =
            {
                "Assets/Scripts",
                "Assets/Prefabs",
                "Assets/Materials",
                "Assets/Textures",
                "Assets/Audio",
                "Assets/Scenes",
                "Assets/Editor",
                "Assets/Plugins"
            };

            foreach (string folder in sampleFolders)
            {
                Color color = FolderColorizerUtility.GenerateRainbowColor(
                    folder,
                    settings.Saturation,
                    settings.Brightness
                );

                EditorGUILayout.BeginHorizontal();

                // Color swatch
                Rect colorRect = GUILayoutUtility.GetRect(30, 18, GUILayout.Width(30));
                color.a = settings.BackgroundAlpha;
                EditorGUI.DrawRect(colorRect, color);

                // Folder name
                string folderName = System.IO.Path.GetFileName(folder);
                EditorGUILayout.LabelField(folderName);

                EditorGUILayout.EndHorizontal();
            }
        }

        private void OnFocus()
        {
            Repaint();
        }
    }
}
