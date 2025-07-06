using System.IO;
using UnityEditor;

namespace MoreCustomizations.Tools {
    
    internal static class BuildAssetBundle {
        
        [MenuItem("For PEAK/Build asset bundle")]
        private static void Build() {
            
            var outputPath = "Assets/AssetBundles";
            
            if(!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            BuildPipeline.BuildAssetBundles(
                outputPath,
                BuildAssetBundleOptions.RecurseDependencies,
                BuildTarget.StandaloneWindows
            );
            
            AssetDatabase.Refresh();
        }
    }
}