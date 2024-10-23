using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnityModelReplacement
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("My Friendly Neighborhood.exe")]

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

        [HarmonyPatch(typeof(EnemyParent))]
        public class EnemyParentPatch
        {
            [HarmonyPatch("Awake")] // Update
            [HarmonyPostfix]
            public static void AddEnemyReplacerComponent(ref EnemyParent __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out EnemyReplacer existingEnemyReplacer))
                {
                    __instance.gameObject.AddComponent<EnemyReplacer>();
                }

                return;
            }
        }

        [HarmonyPatch(typeof(PearlAnimationController))]
        public class PearlAnimationControllerPatch
        {
            [HarmonyPatch("Awake")] // LateUpdate
            [HarmonyPostfix]
            public static void AddPearlReplacerComponent(ref PearlAnimationController __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out PearlReplacer existingPearlReplacer))
                {
                    __instance.gameObject.AddComponent<PearlReplacer>();
                }

                return;
            }
        }

        [HarmonyPatch(typeof(GobbleInWorld))]
        public class GobbleInWorldPatch
        {
            [HarmonyPatch("Awake")] // LateUpdate
            [HarmonyPostfix]
            public static void AddGobbleReplacerComponent(ref GobbleInWorld __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out GobbleReplacer existingGobbleReplacer))
                {
                    __instance.gameObject.AddComponent<GobbleReplacer>();
                }

                return;
            }
        }

        [HarmonyPatch(typeof(SadGobletteController))]
        public class SadGoblettePatch
        {
            [HarmonyPatch("Awake")] // LateUpdate
            [HarmonyPostfix]
            public static void AddGobbleReplacerComponent(ref SadGobletteController __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out GobbleReplacer existingGobbleReplacer))
                {
                    __instance.gameObject.AddComponent<GobbleReplacer>();
                }

                return;
            }
        }

        /*
        [HarmonyPatch(typeof(Player))]
        public class PlayerPatches
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void HealthPatch(ref Player __instance)
            {
                return;
            }
        }
        */
    }
}