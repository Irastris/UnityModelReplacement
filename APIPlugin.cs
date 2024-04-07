using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

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

            Harmony harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PlayerController))]
        public class PlayerControllerPatch
        {
            [HarmonyPatch("Update")]
            [HarmonyPostfix]
            public static void AddPlayerReplacerComponent(ref PlayerController __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out PlayerReplacer existingPlayerReplacer))
                {
                    PlayerReplacer playerReplacer = __instance.gameObject.AddComponent<PlayerReplacer>();
                }

                return;
            }
        }

        // TODO: Another lazy fix for overly bright URP Lit materials, do something better
        [HarmonyPatch(typeof(FlashLightTrigger))]
        public class FlashLightTriggerPatch
        {
            [HarmonyPatch("Start")]
            [HarmonyPostfix]
            public static void LowerLightIntensity(ref FlashLightTrigger __instance)
            { 
                Transform componentOwner = __instance.transform.parent;
                if (componentOwner.name.Equals("FlipLight"))
                {
                    bool isOnSurface = __instance.gameObject.scene.name.Equals("SurfaceScene");
                    Light flipLight = componentOwner.GetComponent<Light>();
                    flipLight.intensity = isOnSurface ? 0f : (flipLight.intensity / 100f);
                }
            }
        }
    }
}