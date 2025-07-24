using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PeakHandlePatch {

    [HarmonyPatch(typeof(PeakHandler), "SetCosmetics")]
    [HarmonyPrefix]
    private static void SetCosmetics(PeakHandler __instance, List<Character> characters) {

        // the character who climbs the helicopter
        if (!Helpers.CustomizationRefsHelper.SyncCustomHats(__instance.firstCutsceneScout))
            Plugin.Logger.LogError($"Something went wrong in {nameof(PeakHandlePatch)} [firstCutsceneScout]...");

        // the characters who sit down
        for (int i = 0; i < __instance.cutsceneScoutRefs.Count(); i++) {
            
            if (!Helpers.CustomizationRefsHelper.SyncCustomHats(__instance.cutsceneScoutRefs[i]))
                Plugin.Logger.LogError($"Something went wrong in {nameof(PeakHandlePatch)} [cutsceneScoutRefs-{i}]...");
        }
    }
}

