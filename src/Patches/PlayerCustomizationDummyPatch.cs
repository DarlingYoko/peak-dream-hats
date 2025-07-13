using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PlayerCustomizationDummyPatch {
    
    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.SetPlayerHat))]
    [HarmonyPrefix]
    private static void SetPlayerHat(PlayerCustomizationDummy __instance, int index) {
        
        Character localCharacter = Character.localCharacter;
        
        if (!localCharacter || !localCharacter.TryGetComponent<CharacterCustomization>(out var characterCustomization))
            return;
        
        Renderer[] actualPlayerHats = characterCustomization.refs.playerHats;
        Renderer[] dummyPlayerHats  = __instance.refs.playerHats;
        
        //Check if character hats more than dummy hats
        if (actualPlayerHats.Length == dummyPlayerHats.Length)
            return;
        
        Renderer firstDummyHatRenderer = dummyPlayerHats.FirstOrDefault();
        
        if (!firstDummyHatRenderer) {
            
            Plugin.Logger.LogError($"Something went wrong in {nameof(PlayerCustomizationDummyPatch)}...");
            return;
        }
        
        int dummyHatLayer = firstDummyHatRenderer.gameObject.layer;     //layer-[Character]
        
        Transform actualHatTransform = characterCustomization.transform.Find(CharacterCustomizationPatch.HAT_PATH);
        Transform dummyHatTransform  = __instance.transform.Find(CharacterCustomizationPatch.HAT_PATH);
        
        var hatRenderList = new List<Renderer>(dummyPlayerHats);
        
        //NOTE: Since this instantiates hats are cloning on just in time, it may causes main thread lock until it finished.
        //    : But it doesn't seem like we're at a point where we need to consider it that way yet.
        for (int hatIndex = dummyPlayerHats.Length; hatIndex < actualPlayerHats.Length; hatIndex++) {
            
            Transform actualHatInstance = actualPlayerHats[hatIndex].transform;
            
            //Find the top gameObject of hat instance.
            while (actualHatInstance.parent != actualHatTransform)
                actualHatInstance = actualHatInstance.parent;
            
            //Clone the top gameObject of hat instance.
            GameObject clonedHatInstance = Object.Instantiate(actualHatInstance.gameObject, dummyHatTransform, false);
            clonedHatInstance.name = actualHatInstance.name;
            
            //Get Renderer and add it to list
            Renderer hatInstanceRender = clonedHatInstance.GetComponentInChildren<Renderer>(true);
            
            hatInstanceRender.gameObject.layer = dummyHatLayer;
            hatRenderList.Add(hatInstanceRender);
        }

        __instance.refs.playerHats = hatRenderList.ToArray();
    }
}
