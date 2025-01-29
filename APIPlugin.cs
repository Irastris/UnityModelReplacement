using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnityModelReplacement
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Zort.exe")]

    public class UnityModelReplacement : BaseUnityPlugin
    {
        public static UnityModelReplacement Instance = null;
        public static AssetBundle AssetBundle = null;

        public void LoadAssetBundle()
        {
            MemoryStream memoryStream;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnityModelReplacement.mods.bundle"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }

            AssetBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            LoadAssetBundle();

            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PlayerController))]
        public class PlayerControllerPatch
        {
            [HarmonyPatch("Update")] // Awake
            [HarmonyPostfix]
            public static void AddPlayerReplacerComponent(ref PlayerController __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out PlayerReplacer existingPlayerReplacer))
                {
                    __instance.gameObject.AddComponent<PlayerReplacer>();
                }
            }
        }

        [HarmonyPatch(typeof(CinematicHandler))]
        public class CinematicHandlerPatch
        {
            [HarmonyPatch("PlayCinematicFunction")]
            [HarmonyPostfix]
            public static void AddCinematicReplacerComponent(ref CinematicHandler __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out CinematicReplacer existingCinematicReplacer))
                {
                    __instance.gameObject.AddComponent<CinematicReplacer>();
                }
                else
                {
                    existingCinematicReplacer.Awake();
                }
            }
        }

        [HarmonyPatch(typeof(FriendlySkinwalker))]
        public class FriendlySkinwalkerPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddSkinwalkerReplacerComponent(ref FriendlySkinwalker __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out SkinwalkerReplacer existingSkinwalkerReplacer))
                {
                    __instance.gameObject.AddComponent<SkinwalkerReplacer>();
                }
            }
        }
    }
}