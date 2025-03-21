using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnityModelReplacement
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("REPO.exe")]

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

        [HarmonyPatch(typeof(PlayerAvatar))]
        public class PlayerAvatarPatch
        {
            [HarmonyPatch("LateStart")]
            [HarmonyPostfix]
            public static void AddPlayerReplacerComponent(ref PlayerAvatar __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out PlayerReplacer existingPlayerReplacer))
                {
                    __instance.gameObject.AddComponent<PlayerReplacer>();
                    __instance.gameObject.GetComponent<PlayerReplacer>().playerName = Traverse.Create(__instance).Field("playerName").GetValue<string>();
                }
            }
        }
    }
}