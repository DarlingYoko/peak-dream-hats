using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations;

[Serializable]
public class CustomizationData {
    
    private static Dictionary<string, AssetBundle> _assetBundleCache = new();
    
#pragma warning disable 0649
    
    [SerializeField]
    private string bundleFile;
    
    [SerializeField]
    private string type;
    
    [SerializeField]
    private string textureName;
    
    [SerializeField]
    private string fitPrefabName;
    
    [SerializeField]
    private string fitMainTextureName;
    
    [SerializeField]
    private string fitSubTextureName;
    
    [SerializeField]
    private Vector3 fitPrefabPosition;
    
    [SerializeField]
    private Vector3 fitPrefabEulerAngle;
    
#pragma warning restore 0649
    
    private string _absoluteBundleFilePath;
    
    public string BundleFile
        => bundleFile;
    
    public string TexturePath
        => $"assets/{bundleFile}/{textureName}";
    
    public string FitPrefabPath
        => $"assets/{bundleFile}/{fitPrefabName}";
    
    public string FitMainTexturePath
        => $"assets/{bundleFile}/{fitMainTextureName}";
    
    public string FitSubTexturePath
        => $"assets/{bundleFile}/{fitSubTextureName}";
    
    public Vector3 FitPrefabPosition
        => fitPrefabPosition;
    
    public Vector3 FitPrefabEulerAngle
        => fitPrefabEulerAngle;
    
    public Vector3 SwizzledFitPrefabPosition
        => new Vector3(
            fitPrefabPosition.x,
            -fitPrefabPosition.z,
            fitPrefabPosition.y
        );
    
    public Quaternion SwizzledFitPrefabRotation
        => Quaternion.Euler(
            new Vector3(
                fitPrefabEulerAngle.x,
                -fitPrefabEulerAngle.z,
                -fitPrefabEulerAngle.y
            )
        ) * Quaternion.AngleAxis(90, Vector3.right);
    
    public string AbsoluteBundleFilePath
        => _absoluteBundleFilePath ??= Path.Combine(LocalPath, bundleFile);
    
    public string TrimmedBundleFilePath
        => AbsoluteBundleFilePath[Plugin.AssetBundlePath.Length..];
    
    public string      Name           { get; private set; }
    public string      LocalPath      { get; private set; }
    public bool        IsLoaded       { get; private set; }
    public GameObject  FitPrefab      { get; private set; }
    public Texture     Texture        { get; private set; }
    public Texture     FitMainTexture { get; private set; }
    public Texture     FitSubTexture  { get; private set; }
    
    public Customization.Type Type {
        
        get {
            
            if (string.IsNullOrEmpty(type))
                return (Customization.Type)100;
            
            return type.ToLower() switch {
                
                "skin"      => Customization.Type.Skin,
                "accessory" => Customization.Type.Accessory,
                "eyes"      => Customization.Type.Eyes,
                "mouth"     => Customization.Type.Mouth,
                "fit"       => Customization.Type.Fit,
                "hat"       => Customization.Type.Hat,
                
                _ => (Customization.Type)100
            };
        }
    }

    internal void SetName(string name)
        => Name = name;
    
    public void SetLocalPath(string path)
        => LocalPath = path;
    
    public void LoadAsset() {
        
        if (IsLoaded) {
            
            Plugin.Logger.LogWarning($"'{Name}' is already loaded. ({TrimmedBundleFilePath})");
            return;
        }
        
        try {
            
            if (!_assetBundleCache.TryGetValue(AbsoluteBundleFilePath, out var assetBundle)) {
                
                assetBundle = AssetBundle.LoadFromFile(AbsoluteBundleFilePath);
                _assetBundleCache[AbsoluteBundleFilePath] = assetBundle;
                
                Plugin.Logger.LogInfo($"Asset bundle '{bundleFile}' loaded! ({TrimmedBundleFilePath})");
                Plugin.Logger.LogInfo($"Catalog list :");
                foreach (string assetPath in assetBundle.GetAllAssetNames())
                    Plugin.Logger.LogInfo($"- {assetPath}");
            }
            
            Texture = assetBundle.LoadAsset<Texture>(TexturePath);
            
            if (!Texture)
                throw new FileNotFoundException($"Cannot find textureName '{TexturePath}' in '{Name}'.");
            
            if (Type is Customization.Type.Hat or Customization.Type.Fit) {
                
                FitPrefab = assetBundle.LoadAsset<GameObject>(FitPrefabPath);
                
                if (!FitPrefab)
                    throw new FileNotFoundException($"Cannot find fitPrefabName '{FitPrefabPath}' in '{Name}'.");
                
                if (!string.IsNullOrWhiteSpace(FitMainTexturePath))
                    FitMainTexture = assetBundle.LoadAsset<Texture>(FitMainTexturePath);
                
                if (!string.IsNullOrWhiteSpace(FitSubTexturePath))
                    FitSubTexture = assetBundle.LoadAsset<Texture>(FitSubTexturePath);
            }
            
            IsLoaded = true;
            
        } catch (Exception ex) {
            
            Plugin.Logger.LogError($"Failed to load '{Name}'.");
            Plugin.Logger.LogError(ex.Message);
            Plugin.Logger.LogError(ex.StackTrace);
        }
    }

    public CustomizationOption CreateOption() {
        
        if (!IsLoaded)
            return null;
        
        var instance = ScriptableObject.CreateInstance<CustomizationOption>();
        
        instance.requiredAchievement = ACHIEVEMENTTYPE.NONE;
        
        instance.name    = Name;
        instance.type    = Type;
        instance.texture = Texture;
        
        return instance;
    }
    
    public bool HasValidConfig(out string invalidReason) {
        
        if (string.IsNullOrWhiteSpace(TexturePath)) {
            
            invalidReason = $"textureName is empty. (textureName = '{TexturePath}')";
            return false;
        }
        
        switch (Type) {
            
            case Customization.Type.Hat:
                
                if (string.IsNullOrWhiteSpace(FitPrefabPath)) {
                    
                    invalidReason = $"Specified type 'Hat' requires 'fitPrefabName', but it's empty. (fitPrefabName = '{FitPrefabPath}')";
                    return false;
                }
                break;
            
            //Not supported yet.
            case Customization.Type.Skin:
                
                invalidReason = "Type 'Skin' is not supported. (contributors are welcome!)";
                return false;
            
            case Customization.Type.Fit:
                
                invalidReason = "Type 'Fit' is not supported. (contributors are welcome!)";
                return false;
            
            case (Customization.Type)100:
                
                invalidReason = $"Specified type '{type}' is not supported.";
                return false;
        }
        
        if (!File.Exists(AbsoluteBundleFilePath)) {
            
            invalidReason = $"Bundle file not found. ({TrimmedBundleFilePath})";
            return false;
        }
        
        //TODO: Might be need validate bundle contents later with _textureName, _fitPrefabName.
        
        invalidReason = null;
        return true;
    }
}