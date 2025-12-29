using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using Mimic.Actors;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnityModelReplacement;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("MIMESIS.exe")]
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

    [HarmonyPatch(typeof(ProtoActor), "SetAsOtherPlayer")]
    public class Patch_ProtoActor_SetAsOtherPlayer
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
            {
                __instance.gameObject.AddComponent<CharacterReplacer>();
            }
        }
    }

    [HarmonyPatch(typeof(ProtoActor), "SetAsMonster")]
    public class Patch_ProtoActor_SetAsMonster
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out MimicReplacer existingMimicReplacer))
            {
                __instance.gameObject.AddComponent<MimicReplacer>();
            }
        }
    }
}
