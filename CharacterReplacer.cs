using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityModelReplacement;

public class CharacterReplacer : MonoBehaviour
{
    public SkinnedMeshRenderer characterRenderer;

    public GameObject replacementModel;
    public SkinnedMeshRenderer replacementRendererBody;
    public SkinnedMeshRenderer replacementRendererHead;
    public List<Transform> rendererRootTransforms = new List<Transform>() { };

    public Transform targetRoot;
    public Transform sourceRoot;
    public string rootName = "DEF_Spine";
    public Vector3 rootOffset = new Vector3();

    public void LoadModelWithoutSeparateHead(string assetPath)
    {
        replacementModel = Instantiate(Plugin.ModBundle.LoadAsset<GameObject>(assetPath));
        replacementModel.transform.localScale = Vector3.one * 1.3f;
        replacementRendererBody = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();
        replacementRendererBody.gameObject.layer = characterRenderer.gameObject.layer;
        replacementRendererBody.updateWhenOffscreen = true;

        if (rendererRootTransforms.Any())
        {
            foreach (Transform transform in rendererRootTransforms)
            {
                foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.forceRenderingOff = true;
                }
            }
        }

        Material mat = new Material(Shader.Find("Shader Graphs/Puppet_Base"));
        if (replacementRendererBody.material != null)
        {
            mat.SetTexture("_Puppet_Colors", replacementRendererBody.material.GetTexture("_Puppet_Colors"));
            mat.SetTexture("_Normal", replacementRendererBody.material.GetTexture("_Normal"));
        }
        replacementRendererBody.material = mat;
    }

    public void LoadModel(string assetPath)
    {
        replacementModel = Instantiate(Plugin.ModBundle.LoadAsset<GameObject>(assetPath));
        replacementModel.transform.localScale = Vector3.one * 1.3f;
        replacementRendererBody = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();
        replacementRendererHead = replacementModel.transform.Find("Model_Head").GetComponent<SkinnedMeshRenderer>();
        replacementRendererBody.gameObject.layer = characterRenderer.gameObject.layer;
        replacementRendererHead.gameObject.layer = characterRenderer.gameObject.layer;
        replacementRendererBody.updateWhenOffscreen = true;
        replacementRendererHead.updateWhenOffscreen = true;

        if (rendererRootTransforms.Any())
        {
            foreach (Transform transform in rendererRootTransforms)
            {
                foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.forceRenderingOff = true;
                }
            }
        }

        Material mat = new Material(Shader.Find("Shader Graphs/Puppet_Base"));
        if (replacementRendererBody.material != null)
        {
            mat.SetTexture("_Puppet_Colors", replacementRendererBody.material.GetTexture("_Puppet_Colors"));
            mat.SetTexture("_Normal", replacementRendererBody.material.GetTexture("_Normal"));
        }
        replacementRendererBody.material = mat;
        replacementRendererHead.material = mat;
    }

    public void LoadModelByPlayerName()
    {
        string playerName = this.transform.Find("PlayerCanvas").GetComponent<PlayerCanvas>().NetworkplayerName;

        switch (playerName)
        {
            case "Toasted":
            case "ToastedShoes":
                LoadModel("Assets/_Modding/ToastedShoes.prefab");
                break;
            case "Homer":
            case "Homer Simpson":
                LoadModel("Assets/_Modding/Homer.prefab");
                break;
            case "Rick":
            case "Rick Sanchez":
                LoadModel("Assets/_Modding/Rick.prefab");
                break;
            case "Goofy":
                LoadModel("Assets/_Modding/Goofy.prefab");
                break;
            case "name":
                Debug.Log($"CharacterReplacer executed too early and got an invalid player name, this is currently unrecoverable.");
                Destroy(this);
                break;
            default:
                Debug.Log($"No replacement model found for player with name {playerName}");
                Destroy(this);
                break;
        }
    }

    public void Awake()
    {
        if (Plugin.ModBundle == null)
        {
            Destroy(this);
        }

        switch (this.gameObject.name)
        {
            case "Sock (1)": // Customization Menu
                characterRenderer = this.transform.Find("Puppet_deform/Objects/Base").GetChild(0).GetComponent<SkinnedMeshRenderer>();
                if (!characterRenderer) Destroy(this);
                rendererRootTransforms.Add(this.transform.Find("Puppet_deform/Objects"));
                rendererRootTransforms.Add(this.transform.Find("Puppet_deform/DEF_Spine/DEF_Spine.001/DEF_Spine.002/DEF_Spine.003/DEF_Spine.004/DEF_Head"));
                LoadModel("Assets/_Modding/ToastedShoes.prefab");
                break;
            case "Player_final(Clone)": // In-Game
                characterRenderer = this.transform.Find("models/Sock/Puppet_deform/Objects/Base").GetChild(0).GetComponent<SkinnedMeshRenderer>();
                if (!characterRenderer) Destroy(this);
                rendererRootTransforms.Add(this.transform.Find("models/Sock/Puppet_deform/Objects"));
                rendererRootTransforms.Add(this.transform.Find("models/Sock/Puppet_deform/DEF_Spine/DEF_Spine.001/DEF_Spine.002/DEF_Spine.003/DEF_Spine.004/DEF_Head"));
                LoadModelByPlayerName();
                break;
            case "Accused_Rig (1)":
                characterRenderer = this.transform.Find("Accused_Rig").GetComponent<SkinnedMeshRenderer>();
                if (!characterRenderer) Destroy(this);
                rendererRootTransforms.Add(this.transform);
                LoadModelWithoutSeparateHead("Assets/_Modding/Luigi.prefab");
                break;
            default:
                Debug.Log($"No match found for {this.gameObject.name}!");
                Destroy(this);
                break;
        }
    }

    public Transform GetBoneTransformFromRenderer(SkinnedMeshRenderer renderer, string boneName)
    {
        IEnumerable<Transform> bones = renderer.bones.OfType<Transform>().Where(x => x.name == boneName);

        return bones.Any() ? bones.First() : null;
    }

    public void CopyPose()
    {
        targetRoot = GetBoneTransformFromRenderer(replacementRendererBody, rootName);
        sourceRoot = GetBoneTransformFromRenderer(characterRenderer, rootName);
        if (sourceRoot == null || targetRoot == null) { Debug.Log("Failed to find root bone for either the source or target!"); return; }
        targetRoot.position = sourceRoot.position + rootOffset;

        foreach (Transform targetBone in replacementRendererBody.bones)
        {
            Transform sourceBone = GetBoneTransformFromRenderer(characterRenderer, targetBone.name);
            if (sourceBone == null)
            {
                Debug.Log($"Could not find bone {targetBone.name} on source model");
                continue;
            }

            targetBone.rotation = sourceBone.rotation;
        }

        return;
    }

    public void LateUpdate()
    {
        if (characterRenderer != null && replacementRendererBody != null)
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
        Debug.Log($"CharacterRenderer attached to {this.gameObject.name} was destroyed!");
        CancelInvoke();
        Destroy(replacementModel);
    }
}