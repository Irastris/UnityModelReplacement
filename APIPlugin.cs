using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using UnityEngine.Assertions;
using UnityEngine;
using System.Reflection;

namespace UnityModelReplacement
{
    public static class PluginInfo
    {
        public const string GUID = "irastris.UnityModelReplacement";
        public const string NAME = "UnityModelReplacement";
        public const string VERSION = "1.0.0";
        public const string WEBSITE = "https://github.com/Irastris/UnityModelReplacement";
    }

    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    [BepInProcess("Content Warning.exe")]

    public class UnityModelReplacement : BaseUnityPlugin
    {
        public static UnityModelReplacement Instance = null;
        public new ManualLogSource Logger;

        private void Awake()
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.GUID);

            if (Instance == null)
            {
                Instance = this;
            }

            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.GUID} is loaded!");
        }

        [HarmonyPatch(typeof(PlayerController))]
        public class PlayerControllerPatch
        {

            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void ManageRegistryBodyReplacements(ref PlayerController __instance)
            {
                // TODO?: Utilize SteamID for determining who gets what model
                /*
                if (player.playerSteamId == 0)
                {
                    return;
                }
                */

                if (!__instance.gameObject.TryGetComponent(out PlayerReplacer existingPlayerReplacer))
                {
                    PlayerReplacer playerReplacer = __instance.gameObject.AddComponent<PlayerReplacer>();
                }

                return;
            }
        }
    }
}