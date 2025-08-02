using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using Recognissimo.Components;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityModelReplacement;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("MageArena.exe")]
public class UnityModelReplacement : BaseUnityPlugin
{
    public static UnityModelReplacement Instance = null;
    public static ManualLogSource MageLogger = null;
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
        if (MageLogger == null) MageLogger = Logger;

        LoadAssetBundle();

        Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll();
    }
    
    [HarmonyPatch(typeof(PlayerMovement), "OnStartClient")]
    public class Patch_PlayerMovement_OnStartClient
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
            {
                __instance.gameObject.AddComponent<CharacterReplacer>();
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), "ActuallyStartGameActually")]
    public class Patch_MainMenuManager_ActuallyStartGameActually
    {
        public static void Postfix(ref MainMenuManager __instance)
        {
            Bloom bloom;
            if (__instance.volume.profile.TryGet<Bloom>(out bloom))
            {
                bloom.active = false;
            }
        }
    }

    public static string[] CustomWords_HP = ["incendiary", "portal", "aperture", "destination", "destiny", "exit", "exodus", "hex"];
    public static string[] CustomWords_Meme = ["five", "big", "boom", "ice", "meet", "jeffrey", "epstein", "nothing", "jet", "holiday"];

    [HarmonyPatch(typeof(VoiceControlListener), "OnStartClient")]
    public class Patch_VoiceControlListener_OnStartClient
    {
        public static void Postfix(ref VoiceControlListener __instance)
        {
            SpeechRecognizer sr = __instance.GetComponent<SpeechRecognizer>();
            foreach (string word in CustomWords_HP)
            {
                sr.Vocabulary.Add(word);
            }
            foreach (string word in CustomWords_Meme)
            {
                sr.Vocabulary.Add(word);
            }
        }
    }

    [HarmonyPatch(typeof(VoiceControlListener), "resetmic")]
    public class Patch_VoiceControlListener_resetmic
    {
        public static void Postfix(ref VoiceControlListener __instance)
        {
            SpeechRecognizer sr = __instance.GetComponent<SpeechRecognizer>();
            foreach (string word in CustomWords_HP)
            {
                sr.Vocabulary.Add(word);
            }
            foreach (string word in CustomWords_Meme)
            {
                sr.Vocabulary.Add(word);
            }
        }
    }

    [HarmonyPatch(typeof(VoiceControlListener), "tryresult")]
    public class Patch_VoiceControlListener_tryresult
    {
        public static bool Prefix(ref VoiceControlListener __instance, ref string res)
        {
            if (res != null)
            {
                if (res.Contains("incendiary") || res.Contains("five") || res.Contains("big") || res.Contains("boom"))
                {
                    __instance.CastFireball();
                }
                else if (res.Contains("ice") || res.Contains("meet"))
                {
                    __instance.CastFrostBolt();
                }
                else if (res.Contains("portal") || res.Contains("aperture") || res.Contains("jeffrey"))
                {
                    __instance.CastWorm();
                }
                else if (res.Contains("destination") || res.Contains("destiny") || res.Contains("exit") || res.Contains("exodus") || res.Contains("epstein"))
                {
                    __instance.CastHole();
                }
                else if (res.Contains("hex") || res.Contains("nothing") || res.Contains("jet") || res.Contains("holiday"))
                {
                    __instance.CastMagicMissle();
                }
                else
                {
                    MageLogger.LogInfo($"Result contained no known custom words: {res}");
                }
            }

            return true;
        }
    }
}
