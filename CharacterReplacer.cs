using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityModelReplacement
{
    public class CharacterReplacer : MonoBehaviour
    {
        public GameObject character;
        public SkinnedMeshRenderer characterRenderer;

        public GameObject replacementModel;
        public SkinnedMeshRenderer replacementRenderer;

        public Transform targetRoot;
        public Transform sourceRoot;
        public string hipsBoneName;
        public Vector3 hipsOffset = new Vector3();

        public void LoadModel(string assetPath)
        {
            replacementModel = Instantiate(UnityModelReplacement.ModBundle.LoadAsset<GameObject>(assetPath));
            replacementRenderer = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in replacementModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.updateWhenOffscreen = true;
            }

            if (characterRenderer.name == "Dad")
            {
                characterRenderer.forceRenderingOff = true;
            }
            else
            {
                characterRenderer.sharedMesh = null;
            }

            if (characterRenderer.name == "Andrew")
            {
                characterRenderer.transform.Find("Head").GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
                characterRenderer.transform.parent.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/Backpack").GetComponent<MeshFilter>().sharedMesh = null;
            }

            if (characterRenderer.name == "Andrew1")
            {
                characterRenderer.transform.parent.Find("Head").GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
            }

            if (character.name == "Mom" || characterRenderer.name == "Igor" || characterRenderer.name == "Vika")
            {
                characterRenderer.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
                characterRenderer.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
            }
            else if (characterRenderer.name == "Dad")
            {
                characterRenderer.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().forceRenderingOff = true;
                characterRenderer.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().forceRenderingOff = true;
            }
            else if (characterRenderer.name == "Dog_Lod1")
            {
                characterRenderer.transform.parent.Find("Dog_Lod2").GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
                characterRenderer.transform.parent.Find("Dog_Lod3").GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
            }
        }

        public void Awake()
        {
            if (UnityModelReplacement.ModBundle == null)
            {
                Destroy(this);
            }

            character = this.gameObject;

            if (character != null)
            {
                switch (character.name)
                {
                    case "Andrew":
                        characterRenderer = this.transform.Find("Andrew").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "mixamorig:Hips";
                        hipsOffset = new Vector3(0f, -0.003045f, 0f);
                        LoadModel("Assets/_Modding/Villager.prefab");
                        break;
                    case "AndrewGRP":
                        characterRenderer = this.transform.Find("Andrew1").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "Hips";
                        hipsOffset = new Vector3(0f, -0.003045f, 0f);
                        LoadModel("Assets/_Modding/VillagerCutscene.prefab");
                        break;
                    case "Mom":
                    case "MomGRP":
                        characterRenderer = this.transform.Find("Mom").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "mixamorig:Hips";
                        hipsOffset = new Vector3(0f, -0.2f, 0f);
                        LoadModel("Assets/_Modding/Alex.prefab");
                        break;
                    case "Igor":
                    case "Igor Cut Scene":
                        characterRenderer = this.transform.Find("Igor").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "mixamorig:Hips";
                        hipsOffset = new Vector3(0f, -0.003045f, 0f);
                        LoadModel("Assets/_Modding/Villager.prefab");
                        break;
                    case "Vika":
                        characterRenderer = this.transform.Find("Vika").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "mixamorig:Hips";
                        hipsOffset = new Vector3(0f, -0.003045f, 0f);
                        LoadModel("Assets/_Modding/Villager.prefab");
                        break;
                    case "Dad":
                    case "Dad Cutscene":
                        characterRenderer = this.transform.Find("Dad").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "mixamorig:Hips";
                        hipsOffset = new Vector3(0f, -0.3f, 0f);
                        LoadModel("Assets/_Modding/Steve.prefab");
                        break;
                    case "Dog":
                        characterRenderer = this.transform.Find("Dog_Lod1").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "Root_M";
                        hipsOffset = new Vector3(0f, -0.022811f, 0f);
                        LoadModel("Assets/_Modding/Wolf.prefab");
                        break;
                    /*
                    case "Fishman":
                        characterRenderer = this.transform.Find("FishMan").GetComponent<SkinnedMeshRenderer>();
                        if (!characterRenderer) Destroy(this);
                        hipsBoneName = "mixamorig:Hips";
                        hipsOffset = new Vector3(0f, -0.237751f, 0f);
                        LoadModel("Assets/_Modding/Captain.prefab");
                        break;
                    */
                    default:
                        Debug.Log($"No character renderer or replacement model found for {character.name}!");
                        Destroy(this);
                        break;
                }
            }
            else
            {
                Debug.Log("No character found!");
                Destroy(this);
            }
        }

        public Transform GetBoneTransformFromRenderer(SkinnedMeshRenderer renderer, string boneName)
        {
            IEnumerable<Transform> bones = renderer.bones.OfType<Transform>().Where(x => x.name == boneName);

            return bones.Any() ? bones.First() : null;
        }

        public void CopyPose()
        {
            targetRoot = GetBoneTransformFromRenderer(replacementRenderer, hipsBoneName);
            sourceRoot = GetBoneTransformFromRenderer(characterRenderer, hipsBoneName);
            if (sourceRoot == null || targetRoot == null) { Debug.Log("Failed to find root bone for either the source or target!"); return; }
            targetRoot.position = sourceRoot.position + hipsOffset;

            foreach (Transform targetBone in replacementRenderer.bones)
            {
                Transform sourceBone = GetBoneTransformFromRenderer(characterRenderer, targetBone.name);
                if (sourceBone == null) continue; // Debug.Log($"Could not find {targetBone.name} on {characterRenderer}!");

                targetBone.rotation = sourceBone.rotation;
            }

            return;
        }

        public void LateUpdate()
        {
            if (characterRenderer != null && replacementRenderer != null)
            {
                replacementModel.SetActive(gameObject.activeInHierarchy);
                CopyPose();
            }
        }
        void OnEnable()
        {
            if (replacementModel != null)
            {
                replacementModel.SetActive(true);
            }
        }

        void OnDisable()
        {
            if (replacementModel != null)
            {
                replacementModel.SetActive(false);
            }
        }

        public void OnDestroy()
        {
            CancelInvoke();
            Destroy(replacementModel);
        }
    }
}
