using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class CharacterCustomizationPatch {
    
    public const string HAT_PATH = @"Scout/Armature/Hip/Mid/AimJoint/Torso/Head/Hat";
    private static Shader _characterShader;
    private static MaterialPropertyBlock _materialPropertyBlock = new();
    private static readonly Vector3 INITIAL_HAT_OFFSET = new(0, 0.2f, 6.0f);
    
    [HarmonyPatch(typeof(CharacterCustomization), "Awake")]
    [HarmonyPostfix]
    private static void Awake(CharacterCustomization __instance) {
        
        var allCustomizationsData = Plugin.AllCustomizationsData;
        
        if (allCustomizationsData == null) {
            
            Plugin.Logger.LogError("Customizations data are not loaded!");
            return;
        }
        
        if (allCustomizationsData.Count == 0) {
            
            Plugin.Logger.LogWarning("There's no customizations data.");
            return;
        }
        
        if (!_characterShader)
            _characterShader = Shader.Find("W/Character");
        
        //TODO: Instantiate prefab of fits
        
        //Hats
        if (allCustomizationsData.TryGetValue(Customization.Type.Hat, out var customizationsData)) {
            
            var hatTransform = __instance.transform.Find(HAT_PATH);
            
            if (!hatTransform) {
                
                Plugin.Logger.LogError("Something went wrong...");
                return;
            }
            
            var instantiatedHats = new List<Renderer>(__instance.refs.playerHats);
            
            foreach (var customizationData in customizationsData) {
                
                if (!customizationData.IsLoaded)
                    continue;
                
                GameObject hatInstance = Object.Instantiate(customizationData.FitPrefab, hatTransform, false);
                Renderer   hatInstanceRenderer = hatInstance.GetComponentInChildren<Renderer>();
                
                if (!hatInstanceRenderer) {
                    
                    Plugin.Logger.LogError(
                        $"Cannot find Renderer component of customization data '{customizationData.Name}'. ({customizationData.AbsoluteBundleFilePath})"
                    );
                    Object.Destroy(hatInstance);
                    continue;
                }
                
                hatInstance.transform.localPosition = INITIAL_HAT_OFFSET + customizationData.SwizzledFitPrefabPosition;
                hatInstance.transform.localRotation = customizationData.SwizzledFitPrefabRotation;
                
                hatInstanceRenderer.gameObject.SetActive(false);
                hatInstanceRenderer.name = customizationData.Name;
                
                var mainMaterial = new Material(_characterShader);
                var subMaterial  = new Material(_characterShader);
                
                if (customizationData.FitMainTexture)
                    mainMaterial.SetTexture("_MainTex", customizationData.FitMainTexture);
                
                if (customizationData.FitSubTexture)
                    subMaterial.SetTexture("_MainTex", customizationData.FitSubTexture);
                
                hatInstanceRenderer.materials = [
                    mainMaterial,
                    subMaterial
                ];
                
                instantiatedHats.Add(hatInstanceRenderer);
            }
            
            __instance.refs.playerHats = instantiatedHats.ToArray();
        }
    }
}