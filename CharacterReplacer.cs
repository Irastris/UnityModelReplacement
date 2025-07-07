using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        replacementModel = Instantiate(Plugin.ModBundle.LoadAsset<GameObject>(assetPath));
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

        if (Plugin.ModBundle == null || !characterRenderer)
        {
            Destroy(this);
        }

        switch (this.gameObject.GetComponent<Character>().characterName.ToLower())
        {
            case "spongebob":
            case "spongebob squarepants":
                LoadModel("Assets/_Modding/SpongeBob.prefab");
                break;
            case "patrick":
            case "patrick star":
                LoadModel("Assets/_Modding/Patrick.prefab");
                break;
            case "eugene":
            case "eugene krabs":
            case "krabs":
            case "mr. krabs":
                LoadModel("Assets/_Modding/MrKrabs.prefab");
                break;
            case "squidward":
            case "squidward tentacles":
                LoadModel("Assets/_Modding/Squidward.prefab");
                break;
            case "plankton":
                LoadModel("Assets/_Modding/Plankton.prefab");
                break;
            case "sandy":
            case "sandy cheeks":
                LoadModel("Assets/_Modding/Sandy.prefab");
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
