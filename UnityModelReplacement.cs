using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnityModelReplacement;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("PEAK.exe")]
public class UnityModelReplacement : BaseUnityPlugin
{
    public static UnityModelReplacement Instance = null;
    public static AssetBundle ModBundle = null;
        
    public void LoadAssetBundle()
    {
        MemoryStream memoryStream;

        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnityModelReplacement.mods.bundle"))
        {
            memoryStream = new MemoryStream((int)stream.Length);
            stream.CopyTo(memoryStream);
        }

        ModBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
    }

    public void Awake()
    {
        if (Instance == null) Instance = this;

        LoadAssetBundle();

        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }

    [HarmonyPatch(typeof(Character), "Awake")]
    public class Patch_Character_Awake
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
            {
                __instance.gameObject.AddComponent<CharacterReplacer>();
            }
        }
    }

    [HarmonyPatch(typeof(BingBong), "Start")]
    public class Patch_BingBong_Start
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out BingBongReplacer existingBingBongReplacer))
            {
                __instance.gameObject.AddComponent<BingBongReplacer>();
            }
        }
    }
}
