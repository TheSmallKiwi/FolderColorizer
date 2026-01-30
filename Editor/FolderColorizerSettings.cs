using UnityEditor;
using UnityEngine;

namespace FolderColorizer
{
    /// <summary>
    /// Settings for automatic folder colorization.
    /// </summary>
    [CreateAssetMenu(fileName = "FolderColorizerSettings", menuName = "Tools/Folder Colorizer Settings")]
    public class FolderColorizerSettings : ScriptableObject
    {
        private const string k_DefaultAssetPath = "Assets/Editor/FolderColorizerSettings.asset";

        [Tooltip("Enable folder colorization in Project window")]
        [SerializeField] private bool m_enabled = true;

        [Tooltip("Opacity of the colored background overlay")]
        [SerializeField, Range(0.1f, 0.5f)] private float m_backgroundAlpha = 0.35f;

        [Tooltip("Saturation of generated colors")]
        [SerializeField, Range(0.2f, 1f)] private float m_saturation = 0.7f;

        [Tooltip("Value/brightness of generated colors")]
        [SerializeField, Range(0.5f, 1f)] private float m_brightness = 0.85f;

        [Tooltip("Whether files inside folders inherit the folder's color")]
        [SerializeField] private bool m_colorizeChildren = true;

        [Tooltip("Minimum folder depth to colorize (0 = Assets, 1 = Assets/*, etc.)")]
        [SerializeField, Range(0, 5)] private int m_minDepth = 1;

        [Header("Text Colors")]
        [Tooltip("Color of the text outline stroke")]
        [SerializeField] private Color m_outlineColor = Color.black;

        [Tooltip("Color of the text")]
        [SerializeField] private Color m_textColor = Color.white;

        [Tooltip("Color of the text outline stroke on hover")]
        [SerializeField] private Color m_hoverOutlineColor = new Color(0.406f, 0.406f, 0.406f, 1f);

        [Tooltip("Color of the text on hover")]
        [SerializeField] private Color m_hoverTextColor = Color.white;

        private static FolderColorizerSettings s_instance;

        /// <summary>
        /// Singleton instance, creates asset if it doesn't exist.
        /// </summary>
        public static FolderColorizerSettings Instance => GetOrCreateInstance();

        public bool Enabled => m_enabled;
        public float BackgroundAlpha => m_backgroundAlpha;
        public float Saturation => m_saturation;
        public float Brightness => m_brightness;
        public bool ColorizeChildren => m_colorizeChildren;
        public int MinDepth => m_minDepth;
        public Color OutlineColor => m_outlineColor;
        public Color TextColor => m_textColor;
        public Color HoverOutlineColor => m_hoverOutlineColor;
        public Color HoverTextColor => m_hoverTextColor;

        /// <summary>
        /// Set enabled state.
        /// </summary>
        public void SetEnabled(bool value)
        {
            if (m_enabled != value)
            {
                m_enabled = value;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set background opacity.
        /// </summary>
        public void SetBackgroundAlpha(float alpha)
        {
            float clamped = Mathf.Clamp(alpha, 0.1f, 0.5f);
            if (!Mathf.Approximately(m_backgroundAlpha, clamped))
            {
                m_backgroundAlpha = clamped;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set color saturation.
        /// </summary>
        public void SetSaturation(float saturation)
        {
            float clamped = Mathf.Clamp(saturation, 0.2f, 1f);
            if (!Mathf.Approximately(m_saturation, clamped))
            {
                m_saturation = clamped;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set color brightness.
        /// </summary>
        public void SetBrightness(float brightness)
        {
            float clamped = Mathf.Clamp(brightness, 0.5f, 1f);
            if (!Mathf.Approximately(m_brightness, clamped))
            {
                m_brightness = clamped;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set whether children inherit parent folder colors.
        /// </summary>
        public void SetColorizeChildren(bool value)
        {
            if (m_colorizeChildren != value)
            {
                m_colorizeChildren = value;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set minimum folder depth to colorize.
        /// </summary>
        public void SetMinDepth(int depth)
        {
            int clamped = Mathf.Clamp(depth, 0, 5);
            if (m_minDepth != clamped)
            {
                m_minDepth = clamped;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set outline color.
        /// </summary>
        public void SetOutlineColor(Color color)
        {
            if (m_outlineColor != color)
            {
                m_outlineColor = color;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set text color.
        /// </summary>
        public void SetTextColor(Color color)
        {
            if (m_textColor != color)
            {
                m_textColor = color;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set hover outline color.
        /// </summary>
        public void SetHoverOutlineColor(Color color)
        {
            if (m_hoverOutlineColor != color)
            {
                m_hoverOutlineColor = color;
                MarkDirtyAndRefresh();
            }
        }

        /// <summary>
        /// Set hover text color.
        /// </summary>
        public void SetHoverTextColor(Color color)
        {
            if (m_hoverTextColor != color)
            {
                m_hoverTextColor = color;
                MarkDirtyAndRefresh();
            }
        }

        private void MarkDirtyAndRefresh()
        {
            EditorUtility.SetDirty(this);
            FolderColorizerDrawer.InvalidateCache();
        }

        private static FolderColorizerSettings GetOrCreateInstance()
        {
            if (s_instance != null)
            {
                return s_instance;
            }

            // Try to find existing asset
            string[] guids = AssetDatabase.FindAssets("t:FolderColorizerSettings");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                s_instance = AssetDatabase.LoadAssetAtPath<FolderColorizerSettings>(path);
                if (s_instance != null)
                {
                    return s_instance;
                }
            }

            // Create new asset
            s_instance = CreateInstance<FolderColorizerSettings>();

            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(k_DefaultAssetPath);
            if (!AssetDatabase.IsValidFolder(directory))
            {
                string parentDir = System.IO.Path.GetDirectoryName(directory);
                string folderName = System.IO.Path.GetFileName(directory);
                AssetDatabase.CreateFolder(parentDir, folderName);
            }

            AssetDatabase.CreateAsset(s_instance, k_DefaultAssetPath);
            AssetDatabase.SaveAssets();

            return s_instance;
        }

        private void OnEnable()
        {
            s_instance = this;
        }
    }
}
