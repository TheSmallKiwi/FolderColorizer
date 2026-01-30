using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FolderColorizer
{
    /// <summary>
    /// Utility methods for the folder colorizer system.
    /// </summary>
    public static class FolderColorizerUtility
    {
        // Golden ratio conjugate for optimal hue distribution
        private const float k_GoldenRatioConjugate = 0.618033988749895f;

        // Cache for sibling indices (path -> index among siblings)
        private static readonly Dictionary<string, int> s_siblingIndexCache = new();

        // Cache for sibling counts (parent path -> child count)
        private static readonly Dictionary<string, int> s_siblingCountCache = new();

        /// <summary>
        /// Clear cached sibling data (call when project structure changes).
        /// </summary>
        public static void ClearSiblingCache()
        {
            s_siblingIndexCache.Clear();
            s_siblingCountCache.Clear();
        }

        /// <summary>
        /// Check if the given path is a folder.
        /// </summary>
        public static bool IsFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            return AssetDatabase.IsValidFolder(path);
        }

        /// <summary>
        /// Get the depth of a path (number of folder levels from Assets).
        /// Assets = 0, Assets/Foo = 1, Assets/Foo/Bar = 2, etc.
        /// </summary>
        public static int GetPathDepth(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return -1;
            }

            path = path.Replace('\\', '/').TrimEnd('/');
            int count = 0;
            foreach (char c in path)
            {
                if (c == '/')
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Get all parent folder paths for a given asset path, from immediate parent to root.
        /// </summary>
        public static IEnumerable<string> GetParentFolders(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                yield break;
            }

            string current = Path.GetDirectoryName(assetPath);
            while (!string.IsNullOrEmpty(current) && current != "Assets")
            {
                current = current.Replace('\\', '/');
                yield return current;
                current = Path.GetDirectoryName(current);
            }

            if (!string.IsNullOrEmpty(current))
            {
                yield return "Assets";
            }
        }

        /// <summary>
        /// Generate a rainbow color for a folder using golden ratio distribution.
        /// Sibling folders get maximally separated hues.
        /// </summary>
        public static Color GenerateRainbowColor(string folderPath, float saturation, float brightness)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return Color.gray;
            }

            float hue = CalculateHue(folderPath);
            return Color.HSVToRGB(hue, saturation, brightness);
        }

        /// <summary>
        /// Calculate hue for a folder based on its position among siblings.
        /// Uses golden ratio to ensure adjacent siblings have different colors.
        /// </summary>
        private static float CalculateHue(string folderPath)
        {
            // Get parent path
            string parentPath = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(parentPath))
            {
                // Root level - use simple hash
                return GetHashHue(folderPath);
            }

            // Get or calculate sibling index
            if (!s_siblingIndexCache.TryGetValue(folderPath, out int siblingIndex))
            {
                CacheSiblingIndices(parentPath);
                s_siblingIndexCache.TryGetValue(folderPath, out siblingIndex);
            }

            // Get parent's hue as base (creates visual hierarchy)
            float parentHue = GetHashHue(parentPath);

            // Offset by golden ratio * sibling index for maximum separation
            float hue = parentHue + (siblingIndex * k_GoldenRatioConjugate);

            // Keep in 0-1 range
            return hue - Mathf.Floor(hue);
        }

        /// <summary>
        /// Cache sibling indices for all folders in a parent directory.
        /// </summary>
        private static void CacheSiblingIndices(string parentPath)
        {
            if (s_siblingCountCache.ContainsKey(parentPath))
            {
                return;
            }

            // Get all subfolders
            string[] subfolders = AssetDatabase.GetSubFolders(parentPath);

            // Sort alphabetically for consistent ordering
            var sorted = subfolders.OrderBy(p => Path.GetFileName(p)).ToArray();

            // Assign indices
            for (int i = 0; i < sorted.Length; i++)
            {
                s_siblingIndexCache[sorted[i]] = i;
            }

            s_siblingCountCache[parentPath] = sorted.Length;
        }

        /// <summary>
        /// Generate a hue value from a path hash.
        /// </summary>
        private static float GetHashHue(string path)
        {
            int hash = GetStableHash(path);
            return (hash & 0x7FFFFFFF) / (float)int.MaxValue;
        }

        /// <summary>
        /// Generate a stable hash code for a string (consistent across sessions).
        /// </summary>
        private static int GetStableHash(string str)
        {
            unchecked
            {
                int hash = 17;
                foreach (char c in str)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        /// <summary>
        /// Ping and select an asset in the Project window.
        /// </summary>
        public static void PingAsset(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset != null)
            {
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }
        }
    }
}
