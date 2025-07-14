using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Helpers;

public class CustomizationRefsHelper {

    public const string REF_TO_HATS_PATH = @"Armature/Hip/Mid/AimJoint/Torso/Head/Hat";

    public static bool SyncCustomHats(CustomizationRefs dstRefs, CustomizationRefs srcRefs = null) {
        if (srcRefs == null) {
            Character localCharacter = Character.localCharacter;

            if (!localCharacter || !localCharacter.TryGetComponent<CharacterCustomization>(out var characterCustomization)) {
                Plugin.Logger.LogError($"Cannot get [LocalCharacter] or its' [CustomizationRefs] ...");
                return false; 
            }
            srcRefs = characterCustomization.refs;
        }

        Renderer[] srcPlayerHats = srcRefs.playerHats;
        Renderer[] dstPlayerHats = dstRefs.playerHats;

        //Check if character hats more than dest hats
        if (srcPlayerHats.Length == dstPlayerHats.Length)
            return true;

        Renderer firstDummyHatRenderer = dstPlayerHats.FirstOrDefault();

        if (!firstDummyHatRenderer) {
            Plugin.Logger.LogError($"Cannot find renders in dstPlayerHats...");
            return false;
        }

        int hatLayer = firstDummyHatRenderer.gameObject.layer;     //layer-[Character]

        Transform srcHatsTransform = srcRefs.transform.Find(REF_TO_HATS_PATH);
        Transform dstHatsTransform = dstRefs.transform.Find(REF_TO_HATS_PATH);

        var hatRenderList = new List<Renderer>(dstPlayerHats);

        //NOTE: Since this instantiates hats are cloning on just in time, it may causes main thread lock until it finished.
        //    : But it doesn't seem like we're at a point where we need to consider it that way yet.
        for (int hatIndex = dstPlayerHats.Length; hatIndex < srcPlayerHats.Length; hatIndex++) {

            Transform srcHatInstance = srcPlayerHats[hatIndex].transform;

            //Find the top gameObject of hat instance.
            while (srcHatInstance.parent != srcHatsTransform)
                srcHatInstance = srcHatInstance.parent;

            //Clone the top gameObject of hat instance.
            GameObject clonedHatInstance = Object.Instantiate(srcHatInstance.gameObject, dstHatsTransform, false);
            clonedHatInstance.name = srcHatInstance.name;

            //Get Renderer and add it to list
            Renderer hatInstanceRender = clonedHatInstance.GetComponentInChildren<Renderer>(true);

            hatInstanceRender.gameObject.layer = hatLayer;
            hatRenderList.Add(hatInstanceRender);
        }

        dstRefs.playerHats = hatRenderList.ToArray();
        return true;
    }
}
