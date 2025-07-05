using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace MoreCustomizations;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public partial class MoreCustomizationsPlugin : BaseUnityPlugin {
    
    public const string ASSET_BUNDLE_DIR = "Customizations";
    
    private static string _assetBundlePath;
    public static string AssetBundlePath
        => _assetBundlePath ??= Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            ASSET_BUNDLE_DIR
        );
    
    internal static MoreCustomizationsPlugin Singleton { get; private set; }
    
    internal static new ManualLogSource Logger;
    
    internal static Harmony _patcher = new(MyPluginInfo.PLUGIN_GUID);
    
    public static IReadOnlyDictionary<Customization.Type, IReadOnlyList<CustomizationData>> AllCustomizationsData { get; private set; }
    
    private void Awake() {
        
        Singleton = this;
        
        Logger = base.Logger;
        
        LoadAllCustomizations();
        
        Logger.LogInfo("Patching methods...");
        _patcher.PatchAll(typeof(Patches.PassportManagerPatch));
        _patcher.PatchAll(typeof(Patches.CharacterCustomizationPatch));
        
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void LoadAllCustomizations() {
        
        if (!Directory.Exists(AssetBundlePath))
            Directory.CreateDirectory(AssetBundlePath);
        
        AllCustomizationsData = null;
        var loadedCustomizationsData = new Dictionary<Customization.Type, List<CustomizationData>>();
        
        string[] customizationDataPaths = Directory.GetFiles(AssetBundlePath, "*.json", SearchOption.AllDirectories);
        
        if (customizationDataPaths.Length == 0)
            throw new FileNotFoundException($"No customization files found in '{AssetBundlePath}'.");
        
        Logger.LogInfo($"Found {customizationDataPaths.Length} possible contents");
        
        var fetchedCustomizationData = new List<CustomizationData>();
        
        foreach (string customizationDataPath in customizationDataPaths) {
            
            string fileName = Path.GetFileNameWithoutExtension(customizationDataPath);
            
            try {
                
                string customizationDataContent = File.ReadAllText(customizationDataPath);
                var customizationData = JsonUtility.FromJson<CustomizationData>(customizationDataContent);
                
                customizationData.SetLocalPath(Path.GetDirectoryName(customizationDataPath));
                customizationData.SetName(fileName);
                
                if (!customizationData.HasValidConfig(out string invalidReason))
                    throw new InvalidDataException(
                        $"""
                        Customization data of '{fileName}' is invalid.
                            {invalidReason}
                        """
                    );
                
                fetchedCustomizationData.Add(customizationData);
                Logger.LogInfo($"Found '{fileName}'.");
                
            } catch (Exception ex) {
                
                Logger.LogError($"Failed to load '{fileName}'.");
                Logger.LogError(ex.Message);
                Logger.LogError(ex.StackTrace);
            }
        }
        
        Logger.LogInfo($"Loading {fetchedCustomizationData.Count} customizations...");
        
        foreach (var customizationData in fetchedCustomizationData) {
            
            customizationData.LoadAsset();
            
            if (!customizationData.IsLoaded)
                continue;
            
            var type = customizationData.Type;
            
            if (!loadedCustomizationsData.TryGetValue(type, out var loadedCustomizations)) {
                
                loadedCustomizations = new List<CustomizationData>();
                loadedCustomizationsData[type] = loadedCustomizations;
            }
            
            loadedCustomizations.Add(customizationData);
            Logger.LogInfo($"Loaded '{customizationData.Name}'! ({customizationData.TrimmedBundleFilePath})");
        }
        
        Logger.LogInfo("Done!");
        
        AllCustomizationsData = loadedCustomizationsData.ToDictionary(
            key   => key.Key,
            value => value.Value as IReadOnlyList<CustomizationData>
        );
    }
}