using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PassportManagerPatch {
    
    [HarmonyPatch(typeof(PassportManager), "Awake")]
    [HarmonyPrefix]
    private static void Awake(PassportManager __instance) {
        
        var allCustomizationsData = Plugin.AllCustomizationsData;
        
        if (allCustomizationsData == null) {
            
            Plugin.Logger.LogError("Customizations data are not loaded!");
            return;
        }
        
        if (allCustomizationsData.Count == 0) {
            
            Plugin.Logger.LogWarning("There's no customizations data.");
            return;
        }
        
        var customization = __instance.GetComponent<Customization>();
        
        var skins       = new List<CustomizationOption>(customization.skins);
        var accessories = new List<CustomizationOption>(customization.accessories);
        var eyes        = new List<CustomizationOption>(customization.eyes);
        var mouths      = new List<CustomizationOption>(customization.mouths);
        var fits        = new List<CustomizationOption>(customization.fits);
        var hats        = new List<CustomizationOption>(customization.hats);
        
        foreach (var (type, customizationsData) in allCustomizationsData) {
            
            var customizationOptions = type switch {
                
                Customization.Type.Skin      => skins,
                Customization.Type.Accessory => accessories,
                Customization.Type.Eyes      => eyes,
                Customization.Type.Mouth     => mouths,
                Customization.Type.Fit       => fits,
                Customization.Type.Hat       => hats,
                
                _ => null
            };
            
            if (customizationOptions == null)
                continue;
            
            foreach (var customizationData in customizationsData) {
                
                if (!customizationData || !customizationData.IsValid())
                    continue;
                
                var option = ScriptableObject.CreateInstance<CustomizationOption>();
        
                option.requiredAchievement = ACHIEVEMENTTYPE.NONE;
                
                option.name    = customizationData.name;
                option.type    = customizationData.Type;
                option.texture = customizationData.IconTexture;
                
                customizationOptions.Add(option);
            }
        }
        
        customization.skins       = skins.ToArray();
        customization.accessories = accessories.ToArray();
        customization.eyes        = eyes.ToArray();
        customization.mouths      = mouths.ToArray();
        customization.fits        = fits.ToArray();
        customization.hats        = hats.ToArray();
    }
}