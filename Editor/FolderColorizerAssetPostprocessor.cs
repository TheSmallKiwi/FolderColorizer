using UnityEditor;

namespace FolderColorizer
{
    /// <summary>
    /// Invalidates folder colorizer cache when assets change.
    /// </summary>
    public class FolderColorizerAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            // Invalidate cache when any assets change
            // This ensures new folders get colored and moved folders update
            if (importedAssets.Length > 0 ||
                deletedAssets.Length > 0 ||
                movedAssets.Length > 0)
            {
                FolderColorizerDrawer.InvalidateCache();
            }
        }
    }
}
