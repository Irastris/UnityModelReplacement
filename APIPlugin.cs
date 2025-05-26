using BepInEx;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UnityModelReplacement
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("SchoolBoy Runaway.exe")]

    public class UnityModelReplacement : BaseUnityPlugin
    {
        public static UnityModelReplacement Instance = null;
        public static AssetBundle ModBundle = null;
        public static AssetBundle SceneBundle = null;

        public void LoadAssetBundle()
        {
            MemoryStream memoryStream;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnityModelReplacement.mods.bundle"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }

            ModBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnityModelReplacement.scenes.bundle"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }

            SceneBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
        }
        
        public static void UpdateMaterials(ref Material originalMaterial, string materialName)
        {
            if (ModBundle != null)
            {
                string basePath = "Assets/_Modding/Textures/Minecraft";
                float brightnessBase = 0.25f;
                float brightnessStep1 = 0.1f;

                try
                {
                    if (materialName != "")
                    {
                        // Console.WriteLine($"Processing material {materialName}");
                        switch (materialName)
                        {
                            case "_Landscape":
                                originalMaterial.SetTexture("_TextureA", ModBundle.LoadAsset<Texture2D>($"{basePath}/Grass.png"));
                                originalMaterial.SetTexture("_TextureB", ModBundle.LoadAsset<Texture2D>($"{basePath}/Ground.png"));
                                break;
                            case "_Road":
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Road.png");
                                break;
                            case "_Tile":
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Tile.png");
                                break;
                            case "_Planks":
                            case "_Wall Planks":
                            case "_Wall Planks_B":
                            case "_Wall Planks_C":
                            case "_Wall Planks_D":
                            case "_Wall Planks_E":
                            case "_Wall Planks_F":
                            case "_Floor_Planks":
                            case "_Walls":
                            case "_Ceiling":
                            case "Windows":
                                originalMaterial.shader = Shader.Find("Standard");
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Planks.png");
                                originalMaterial.SetColor("_Color", Color.white);
                                originalMaterial.SetTexture("_EmissionMap", originalMaterial.mainTexture);
                                originalMaterial.SetColor("_EmissionColor", Color.white * brightnessBase);
                                originalMaterial.EnableKeyword("_EMISSION");
                                break;
                            case "_Skirting_Ceiling":
                            case "_Border_Floor":
                                originalMaterial.shader = Shader.Find("Standard");
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Planks.png");
                                originalMaterial.mainTextureScale = new Vector2(0.25f, 0.0625f);
                                originalMaterial.SetColor("_Color", Color.white);
                                originalMaterial.SetTexture("_EmissionMap", originalMaterial.mainTexture);
                                originalMaterial.SetColor("_EmissionColor", Color.white * brightnessBase);
                                originalMaterial.EnableKeyword("_EMISSION");
                                break;
                            case "_Roof":
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Roof.png");
                                break;
                            case "_Windows_Doors":
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Windows_Doors.png");
                                break;
                            case "Rug_A":
                            case "Rug_B":
                            case "Rug_C":
                            case "Rug_D":
                            case "Rug_F":
                                originalMaterial.shader = Shader.Find("Standard");
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Floor_Carpet.png");
                                originalMaterial.SetColor("_Color", Color.white);
                                originalMaterial.SetTexture("_EmissionMap", originalMaterial.mainTexture);
                                originalMaterial.SetColor("_EmissionColor", Color.white * brightnessBase);
                                originalMaterial.EnableKeyword("_EMISSION");
                                break;
                            case "_Tile_A":
                            case "_Tile_B":
                            case "_Tile_C":
                            case "Tile_D":
                                originalMaterial.shader = Shader.Find("Standard");
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Tile_Polished.png");
                                originalMaterial.SetColor("_Color", Color.white);
                                originalMaterial.SetTexture("_EmissionMap", originalMaterial.mainTexture);
                                originalMaterial.SetColor("_EmissionColor", Color.white * brightnessBase);
                                originalMaterial.EnableKeyword("_EMISSION");
                                break;
                            case "_Concrete":
                            case "_Wall Stone":
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/Cobblestone.png");
                                break;
                            case "Entrance Door_A":
                            case "Entrance Door_B":
                                originalMaterial.shader = Shader.Find("Standard");
                                originalMaterial.mainTexture = ModBundle.LoadAsset<Texture2D>($"{basePath}/OakDoor.png");
                                originalMaterial.SetColor("_Color", Color.white);
                                originalMaterial.SetTexture("_EmissionMap", originalMaterial.mainTexture);
                                originalMaterial.SetColor("_EmissionColor", Color.white * (brightnessBase + brightnessStep1));
                                originalMaterial.EnableKeyword("_EMISSION");
                                break;
                            case "Bathroom_Pack_A":
                            case "Bathroom_Pack_B":
                            case "Washer":
                            case "Sink_A":
                            case "Toilet":
                            case "BathCabinet":
                            case "Bath":
                            case "Bathroom_Lamp":
                            case "Floor_Grid":
                            case "Water_Pipe_Grid":
                                originalMaterial.shader = Shader.Find("Standard");
                                originalMaterial.SetColor("_Color", Color.white);
                                originalMaterial.SetTexture("_EmissionMap", originalMaterial.mainTexture);
                                originalMaterial.SetColor("_EmissionColor", Color.white * brightnessBase);
                                originalMaterial.EnableKeyword("_EMISSION");
                                break;
                            default:
                                // Debug.Log($"No replacement found for {materialName}!");
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void UpdateScene()
        {
            // Vegetation
            GameObject.Find("/_Parent_Grass").SetActive(false);
            GameObject.Find("/_Parent_Flowers").SetActive(false);
            GameObject.Find("/_Parent_Bush").SetActive(false);
            GameObject.Find("/_Parent_Trees").SetActive(false);
            GameObject.Find("/_Paret Flower Watering").SetActive(false);
            GameObject.Find("/_Parent_Lake/_Parent_Grass").SetActive(false);

            // Old House
            GameObject.Find("/_Parent_Houses/Old House").SetActive(false);

            // Andrew's Room
            GameObject.Find("/_Parent_Mainroom/Bed_A").SetActive(false);
            GameObject.Find("/_Parent_Mainroom/Curtain (3)").SetActive(false);
            GameObject.Find("/_Parent_Mainroom/Curtain (4)").SetActive(false);
            GameObject.Find("/_Parent_Mainroom/Chandelier_C").SetActive(false);
            GameObject.Find("/_Parent_Mainroom/Group LOD Mainroom/Hanger_B").SetActive(false);

            // Entrance & Hallway
            GameObject.Find("/_Parent_Objects/Curtain (5)").SetActive(false);

            // Bathroom
            GameObject.Find("/_Parent_Bathroom/BathCurtain").SetActive(false);
            GameObject.Find("/_Parent_Bathroom/Plug").SetActive(false);
            GameObject.Find("/_Parent_Bathroom/Washer_Wire").SetActive(false);
            GameObject.Find("/_Parent_Bathroom/Cloth_4").SetActive(false);
            GameObject.Find("/_Parent_Bathroom/Cloth_6").SetActive(false);
            GameObject.Find("/_Parent_Bathroom/Cloth_10").SetActive(false);
            GameObject.Find("/_Parent_Bathroom/Cloth_20").SetActive(false);
            GameObject.Find("/_Parent_Bathroom/Cloth_27").SetActive(false);

            // Kitchen
            // GameObject.Find("/_Parent_Kitchen/Chandelier_A").SetActive(false); // Needed for puzzle
            GameObject.Find("/_Parent_Kitchen/Curtain (8)").SetActive(false);
            GameObject.Find("/_Parent_Kitchen/Curtain (9)").SetActive(false);
            GameObject.Find("/_Parent_Kitchen/Kitchen_TV").SetActive(false);
            GameObject.Find("/_Parent_Kitchen/Plug").SetActive(false);
            GameObject.Find("/_Parent_Kitchen/Calendar").SetActive(false);

            // Living Room
            GameObject.Find("/_Parent_Livingroom/Curtain").SetActive(false);
            GameObject.Find("/_Parent_Livingroom/Curtain (1)").SetActive(false);
            GameObject.Find("/_Parent_Livingroom/Curtain (2)").SetActive(false);
            GameObject.Find("/_Parent_Livingroom/Chandelier_B").SetActive(false);
            GameObject.Find("/_Parent_Livingroom/Group LOD Livingroom/Floor_Lamp").SetActive(false);
            GameObject.Find("/_Parent_Livingroom/Group LOD Livingroom/Floor_Lamp (1)").SetActive(false);
            GameObject.Find("/_Parent_Livingroom/Group LOD Livingroom/Conditioner_A (1)").SetActive(false);

            // Parent's Bedroom
            GameObject.Find("/_Parent_Bedroom/Parents_Bed").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Curtain (6)").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Curtain (7)").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Chandelier_D").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Group LOD Bedroom/Cloth_6").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Group LOD Bedroom/Cloth_10").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Group LOD Bedroom/Cloth_12").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Group LOD Bedroom/Conditioner_B").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Group LOD Bedroom/Yoga_Roller").SetActive(false);
            GameObject.Find("/_Parent_Bedroom/Group LOD Bedroom/Yoga_Mat").SetActive(false);

            // Exterior
            GameObject.Find("/_Location/Curb").SetActive(false);
            GameObject.Find("/_Parent_Objects/Mailbox").SetActive(false);
            GameObject.Find("/_Parent_Yard/Car").SetActive(false);

            // Doors
            GameObject.Find("/_Parent_Objects/Entrance Door/Door").GetComponent<MeshFilter>().mesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/OakDoor.mesh");
            GameObject.Find("/_Parent_Objects/Entrance Door/Door/Lock_A").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door/Door/Glass").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Storeroom/Anim Door Knocking/Door").GetComponent<MeshFilter>().mesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/OakDoor.mesh");
            GameObject.Find("/_Parent_Objects/Entrance Door Storeroom/Anim Door Knocking/Door/Lock_A").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Bathroom/Door").GetComponent<MeshFilter>().mesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/OakDoor.mesh");
            GameObject.Find("/_Parent_Objects/Entrance Door Bathroom/Door/Lock_A").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Bathroom/Door/Inside").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Kitchen/Door").GetComponent<MeshFilter>().mesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/OakDoor.mesh");
            GameObject.Find("/_Parent_Objects/Entrance Door Kitchen/Door/Lock_A").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Kitchen/Door/Inside").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Mainroom/Door").GetComponent<MeshFilter>().mesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/OakDoor.mesh");
            GameObject.Find("/_Parent_Objects/Entrance Door Mainroom/Door/Lock_A").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Mainroom/Door/Inside").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Livingroom/Door").GetComponent<MeshFilter>().mesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/OakDoor.mesh");
            GameObject.Find("/_Parent_Objects/Entrance Door Livingroom/Door/Lock_A").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Livingroom/Door/Inside").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Bedroom/Door").GetComponent<MeshFilter>().mesh = ModBundle.LoadAsset<Mesh>("Assets/_Modding/OakDoor.mesh");
            GameObject.Find("/_Parent_Objects/Entrance Door Bedroom/Door/Lock_A").SetActive(false);
            GameObject.Find("/_Parent_Objects/Entrance Door Bedroom/Door/Inside").SetActive(false);

            // New Stuff
            SceneManager.LoadScene("Minecraft", LoadSceneMode.Additive);
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

                // Textures
                Material[] array = Resources.FindObjectsOfTypeAll<Material>();
                for (int i = 0; i < array.Length; i++)
                {
                    UpdateMaterials(ref array[i], array[i].name);
                }

                // Scene
                UpdateScene();

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

        /*
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
        */
    }
}