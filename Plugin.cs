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
[BepInProcess("PEAK.exe")]
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
    
    [HarmonyPatch(typeof(Character), "Awake")]
    public class Patch_Character_Awake
    {
        public static void Postfix(ref MonoBehaviour __instance)
        {
            Debug.Log("Postfix is firing on method Awake from class Character!");
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
            if (ModBundle != null)
            {
                Mesh garyMesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/Gary.mesh");
                Texture2D garyTex = ModBundle.LoadAsset<Texture2D>("Assets/_Modding/Gary.png");

                foreach (MeshRenderer renderer in __instance.transform.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (renderer.transform.name == "Cube")
                    {
                        MeshFilter meshFilter = renderer.transform.GetComponent<MeshFilter>();
                        meshFilter.sharedMesh = garyMesh;
                        renderer.material.SetTexture("_BaseTexture", garyTex);
                    }
                    else
                    {
                        renderer.forceRenderingOff = true;
                    }
                }
            }
        }
    }
}
