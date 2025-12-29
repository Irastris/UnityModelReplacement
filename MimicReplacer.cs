using Mimic.Actors;
using System;
using UnityEngine;

namespace UnityModelReplacement;

public class MimicReplacer : MonoBehaviour
{
    public GameObject replacementModel;
    public SkinnedMeshRenderer mimicRenderer;
    public SkinnedMeshRenderer replacementRenderer;
    public Transform targetRoot;
    public Transform sourceRoot;
    public string rootName = "Hips";

    public void LoadModel(string assetPath)
    {
        replacementModel = Instantiate(UnityModelReplacement.ModBundle.LoadAsset<GameObject>(assetPath));
        replacementRenderer = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();
        replacementRenderer.updateWhenOffscreen = true;

        mimicRenderer.forceRenderingOff = true;

        foreach (Material material in replacementRenderer.materials)
        {
            material.shader = Shader.Find("relu/Lit3Actor");
        }
    }

    public void Awake()
    {
        if (!this.transform.GetComponent<ProtoActor>().nickName.StartsWith("mimic"))
        {
            Destroy(this);
        }

        this.transform.Find("prefab_MimicPuppet(Clone)/PlayerCharacter/Body").TryGetComponent<SkinnedMeshRenderer>(out mimicRenderer);

        // LoadModel("Assets/_Modding/HomerSimpson.prefab");
        LoadModel("Assets/_Modding/Kermit.prefab");
        // LoadModel("Assets/_Modding/MargeSimpson.prefab");
        // LoadModel("Assets/_Modding/MortySmith.prefab");
    }

    public Transform GetBoneTransformFromRenderer(SkinnedMeshRenderer renderer, string boneName)
    {
        Transform bone = Array.Find(renderer.bones, bone => bone.name == boneName);

        return bone;
    }

    public void CopyPose()
    {
        if (mimicRenderer == null)
        {
            this.transform.Find("prefab_MimicPuppet(Clone)/PlayerCharacter/Body").TryGetComponent<SkinnedMeshRenderer>(out mimicRenderer);
            return;
        }

        targetRoot = GetBoneTransformFromRenderer(replacementRenderer, rootName);
        sourceRoot = GetBoneTransformFromRenderer(mimicRenderer, rootName);
        targetRoot.position = sourceRoot.position;

        foreach (Transform targetBone in replacementRenderer.bones)
        {
            Transform sourceBone = GetBoneTransformFromRenderer(mimicRenderer, targetBone.name);
            if (sourceBone == null)
            {
                Debug.Log($"Failed to find matching bone on source renderer for {targetBone.name}");
                continue;
            }
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
        Debug.Log($"MimicRenderer attached to {this.transform.name} was destroyed!");
        CancelInvoke();
        Destroy(replacementModel);
    }
}
