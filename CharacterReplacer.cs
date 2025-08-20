using System;
using UnityEngine;

namespace UnityModelReplacement;

public class CharacterReplacer : MonoBehaviour
{
    public GameObject replacementModel;
    public SkinnedMeshRenderer characterRenderer;
    public SkinnedMeshRenderer replacementRenderer;
    public Transform targetRoot;
    public Transform sourceRoot;
    public string rootName = "Hip";

    public void LoadModel(string assetPath)
    {
        replacementModel = Instantiate(UnityModelReplacement.ModBundle.LoadAsset<GameObject>(assetPath));
        replacementModel.transform.localScale = Vector3.one * 1.2121f; // Bandaid for me fucking up when preparing my Blender project
        replacementRenderer = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();
        replacementRenderer.updateWhenOffscreen = true;

        foreach (Renderer renderer in this.gameObject.GetComponentsInChildren<Renderer>(true))
        {
            renderer.forceRenderingOff = true;
        }

        foreach (Material material in replacementRenderer.materials)
        {
            material.shader = Shader.Find("W/Character");
            material.DisableKeyword("_USESKINTONE_ON");
            if (this.gameObject.GetComponent<Character>().IsLocal)
            {
                material.SetFloat("_VertexGhost", 1f);
            }
        }
    }

    public void Awake()
    {
        characterRenderer = this.transform.Find("Scout/MainMesh").GetComponent<SkinnedMeshRenderer>();

        if (UnityModelReplacement.ModBundle == null || !characterRenderer)
        {
            Destroy(this);
        }

        switch (this.gameObject.GetComponent<Character>().characterName.ToLower())
        {
            case "homer":
            case "homer simpson":
                LoadModel("Assets/_Modding/HomerSimpson.prefab");
                break;
            case "marge":
            case "marge simpson":
                LoadModel("Assets/_Modding/MargeSimpson.prefab");
                break;
            case "bart":
            case "bart simpson":
                LoadModel("Assets/_Modding/BartSimpson.prefab");
                break;
            case "ned":
            case "ned flanders":
                LoadModel("Assets/_Modding/NedFlanders.prefab");
                break;
            default:
                Destroy(this);
                break;
        }
    }

    public Transform GetBoneTransformFromRenderer(SkinnedMeshRenderer renderer, string boneName)
    {
        Transform bone = Array.Find(renderer.bones, bone => bone.name == boneName);

        return bone;
    }

    public void CopyPose()
    {
        if (characterRenderer == null || replacementRenderer == null)
        {
            return;
        }

        targetRoot = GetBoneTransformFromRenderer(replacementRenderer, rootName);
        sourceRoot = GetBoneTransformFromRenderer(characterRenderer, rootName);
        targetRoot.position = sourceRoot.position;

        foreach (Transform targetBone in replacementRenderer.bones)
        {
            Transform sourceBone = GetBoneTransformFromRenderer(characterRenderer, targetBone.name);
            targetBone.rotation = sourceBone.rotation;
        }

        return;
    }

    public void LateUpdate()
    {
        CopyPose();
    }

    public void OnDestroy()
    {
        Debug.Log($"CharacterRenderer attached to {this.gameObject.name} was destroyed!");
        CancelInvoke();
        Destroy(replacementModel);
    }
}
