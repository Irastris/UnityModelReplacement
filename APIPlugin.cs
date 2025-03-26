using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UnityModelReplacement
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("SchoolBoy Runaway.exe")]

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

        [HarmonyPatch(typeof(AI.Andrew.Andrew))]
        public class AndrewPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddCharacterReplacerComponent(ref AI.Andrew.Andrew __instance)
            {
                // Self
                if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
                {
                    __instance.gameObject.AddComponent<CharacterReplacer>();
                }

                // Cutscenes
                foreach (string characterPath in new string[] { "Pre Story cut-scene/AndrewGRP", "Pre Story cut-scene/MomGRP", "Pre Story cut-scene/Igor Cut Scene", "Belt Ending/Dad Cutscene" })
                {
                    Transform character = GameObject.Find("_Parent_Timelines").transform.Find(characterPath);
                    if (character)
                    {
                        if (!character.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer2))
                        {
                            character.gameObject.AddComponent<CharacterReplacer>();
                            Debug.Log($"CharacterReplacer component added to {character.name}");
                        }
                    }
                    else
                    {
                        Debug.Log($"Character {characterPath} not found!");
                    }
                }

                return;
            }
        }

        [HarmonyPatch(typeof(AI.Mom.Mom))]
        public class MomPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddCharacterReplacerComponent(ref AI.Mom.Mom __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
                {
                    __instance.gameObject.AddComponent<CharacterReplacer>();
                }
            }
        }

        [HarmonyPatch(typeof(AI.Igor.Igor))]
        public class IgorPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddCharacterReplacerComponent(ref AI.Igor.Igor __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
                {
                    __instance.gameObject.AddComponent<CharacterReplacer>();
                }
            }
        }

        [HarmonyPatch(typeof(AI.Vika.Vika))]
        public class VikaPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddCharacterReplacerComponent(ref AI.Vika.Vika __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
                {
                    __instance.gameObject.AddComponent<CharacterReplacer>();
                }
            }
        }

        [HarmonyPatch(typeof(AI.Dad.Dad))]
        public class DadPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddCharacterReplacerComponent(ref AI.Dad.Dad __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
                {
                    __instance.gameObject.AddComponent<CharacterReplacer>();
                }
            }
        }

        [HarmonyPatch(typeof(AI.Dog.Dog))]
        public class DogPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddCharacterReplacerComponent(ref AI.Dog.Dog __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
                {
                    __instance.gameObject.AddComponent<CharacterReplacer>();
                }
            }
        }

        [HarmonyPatch(typeof(AI.Fishman.Fishman))]
        public class FishermanPatch
        {
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void AddCharacterReplacerComponent(ref AI.Fishman.Fishman __instance)
            {
                if (!__instance.gameObject.TryGetComponent(out CharacterReplacer existingCharacterReplacer))
                {
                    __instance.gameObject.AddComponent<CharacterReplacer>();
                }
            }
        }
    }
}