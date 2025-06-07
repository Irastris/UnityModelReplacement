using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace UnityModelReplacement;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("StandaloneWindows64.exe")]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance = null;
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

    [HarmonyPatch(typeof(SockAnimationCustomisation), "OnEnable")]
    public class Patch_SockAnimationCustomisation_OnEnable
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
            {
                __instance.gameObject.AddComponent<CharacterReplacer>();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerCanvas), "UpdatedName")]
    public class Patch_PlayerCanvas_UpdatedName
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.transform.parent.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
            {
                __instance.transform.parent.gameObject.AddComponent<CharacterReplacer>();
            }
        }
    }

    [HarmonyPatch(typeof(Player), "SwitchToCam")]
    public class Patch_Player_SwitchToCam
    {
        public static void Postfix(ref MonoBehaviour __instance, bool fps)
        {
            if (__instance.gameObject.TryGetComponent<CharacterReplacer>(out CharacterReplacer cr))
            {
                if (cr.replacementRendererHead != null)
                {
                    cr.replacementRendererHead.forceRenderingOff = fps;
                }
            }
            else
            {
                // Bandaid fix
                Debug.Log($"SwitchToCam postfix ran, but couldn't find a CharacterReplacer component on {__instance.gameObject.name}, so let's create it!");
                __instance.gameObject.AddComponent<CharacterReplacer>();
            }
        }
    }

    [HarmonyPatch(typeof(AccusedFaces), "Awake")]
    public class Patch_AccusedFaces_Awake
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
            {
                __instance.gameObject.AddComponent<CharacterReplacer>();
            }
        }
    }
}
