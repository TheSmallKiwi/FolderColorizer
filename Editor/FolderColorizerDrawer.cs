using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FolderColorizer
{
    /// <summary>
    /// Draws automatically generated rainbow colors for folders in the Project window.
    /// </summary>
    [InitializeOnLoad]
    public static class FolderColorizerDrawer
    {
        private const float k_ListViewIconWidth = 16f;
        private const float k_ListViewPadding = 2f;
        private const float k_ListViewMaxHeight = 20f;
        private const float k_OutlineOffset = 1f;

        // Cache for computed colors (GUID -> color)
        private static readonly Dictionary<string, Color?> s_colorCache = new();

        // Flag to rebuild cache
        private static bool s_cacheInvalid = true;

        // Reusable GUIStyle for text rendering
        private static GUIStyle s_labelStyle;

        // Outline offsets for 8-direction stroke
        private static readonly Vector2[] s_outlineOffsets =
        {
            new Vector2(-k_OutlineOffset, -k_OutlineOffset),
            new Vector2(0, -k_OutlineOffset),
            new Vector2(k_OutlineOffset, -k_OutlineOffset),
            new Vector2(-k_OutlineOffset, 0),
            new Vector2(k_OutlineOffset, 0),
            new Vector2(-k_OutlineOffset, k_OutlineOffset),
            new Vector2(0, k_OutlineOffset),
            new Vector2(k_OutlineOffset, k_OutlineOffset)
        };

        static FolderColorizerDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
            EditorApplication.projectChanged += OnProjectChanged;
        }

        /// <summary>
        /// Invalidate the color cache (call when settings change).
        /// </summary>
        public static void InvalidateCache()
        {
            s_cacheInvalid = true;
            s_colorCache.Clear();
            FolderColorizerUtility.ClearSiblingCache();
            EditorApplication.RepaintProjectWindow();
        }

        private static void OnProjectChanged()
        {
            InvalidateCache();
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            if (s_cacheInvalid)
            {
                s_colorCache.Clear();
                s_cacheInvalid = false;
            }

            var settings = FolderColorizerSettings.Instance;
            if (settings == null || !settings.Enabled)
            {
                return;
            }

            // Try to get color for this item
            Color? color = GetColorForGuid(guid, settings);
            if (!color.HasValue)
            {
                return;
            }

            // Draw background
            Color drawColor = color.Value;
            drawColor.a = settings.BackgroundAlpha;
            EditorGUI.DrawRect(selectionRect, drawColor);

            // Draw outlined text in list view
            bool isListView = selectionRect.height <= k_ListViewMaxHeight;
            if (isListView)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetName = Path.GetFileNameWithoutExtension(assetPath);
                DrawOutlinedLabel(selectionRect, assetName, settings);
            }
        }

        private static void DrawOutlinedLabel(Rect selectionRect, string text, FolderColorizerSettings settings)
        {
            if (s_labelStyle == null)
            {
                s_labelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(0, 0, 0, 0)
                };
            }

            // Calculate text rect (after icon)
            Rect textRect = new Rect(
                selectionRect.x + k_ListViewIconWidth + k_ListViewPadding,
                selectionRect.y,
                selectionRect.width - k_ListViewIconWidth - k_ListViewPadding,
                selectionRect.height
            );

            // Check if mouse is hovering over this item
            bool isHovered = selectionRect.Contains(Event.current.mousePosition);

            // Determine colors from settings
            Color outlineColor = isHovered ? settings.HoverOutlineColor : settings.OutlineColor;
            Color textColor = isHovered ? settings.HoverTextColor : settings.TextColor;

            // Draw outline - set all states to prevent Unity from changing colors
            SetAllTextColors(s_labelStyle, outlineColor);

            foreach (var offset in s_outlineOffsets)
            {
                Rect offsetRect = new Rect(
                    textRect.x + offset.x,
                    textRect.y + offset.y,
                    textRect.width,
                    textRect.height
                );
                GUI.Label(offsetRect, text, s_labelStyle);
            }

            // Draw main text
            SetAllTextColors(s_labelStyle, textColor);
            GUI.Label(textRect, text, s_labelStyle);
        }

        private static Color? GetColorForGuid(string guid, FolderColorizerSettings settings)
        {
            // Check cache first
            if (s_colorCache.TryGetValue(guid, out Color? cachedColor))
            {
                return cachedColor;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath))
            {
                s_colorCache[guid] = null;
                return null;
            }

            Color? color = null;

            // If this is a folder, generate its color directly
            if (FolderColorizerUtility.IsFolder(assetPath))
            {
                int depth = FolderColorizerUtility.GetPathDepth(assetPath);
                if (depth >= settings.MinDepth)
                {
                    color = FolderColorizerUtility.GenerateRainbowColor(
                        assetPath,
                        settings.Saturation,
                        settings.Brightness
                    );
                }
            }
            // If colorizing children, check parent folders
            else if (settings.ColorizeChildren)
            {
                color = GetInheritedColor(assetPath, settings);
            }

            s_colorCache[guid] = color;
            return color;
        }

        private static void SetAllTextColors(GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            style.hover.textColor = color;
            style.active.textColor = color;
            style.focused.textColor = color;
            style.onNormal.textColor = color;
            style.onHover.textColor = color;
            style.onActive.textColor = color;
            style.onFocused.textColor = color;
        }

        private static Color? GetInheritedColor(string assetPath, FolderColorizerSettings settings)
        {
            // Walk up from asset path, return first (innermost) qualifying parent color
            foreach (string parentPath in FolderColorizerUtility.GetParentFolders(assetPath))
            {
                int depth = FolderColorizerUtility.GetPathDepth(parentPath);
                if (depth >= settings.MinDepth)
                {
                    return FolderColorizerUtility.GenerateRainbowColor(
                        parentPath,
                        settings.Saturation,
                        settings.Brightness
                    );
                }
            }

            return null;
        }
    }
}
