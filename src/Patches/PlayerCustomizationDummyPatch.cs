using HarmonyLib;

using Plugin = MoreCustomizations.MoreCustomizationsPlugin;

namespace MoreCustomizations.Patches;

public class PlayerCustomizationDummyPatch {

    [HarmonyPatch(typeof(PlayerCustomizationDummy), nameof(PlayerCustomizationDummy.SetPlayerHat))]
    [HarmonyPrefix]
    private static void SetPlayerHat(PlayerCustomizationDummy __instance, int index) {

        if (!Helpers.CustomizationRefsHelper.SyncCustomHats(__instance.refs))
            Plugin.Logger.LogError($"Something went wrong in {nameof(PlayerCustomizationDummy.SetPlayerHat)} patch...");
    }
}