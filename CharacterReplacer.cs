using BepInEx.Logging;
using System;
using System.Collections;
using UnityEngine;

namespace UnityModelReplacement;

public class CharacterReplacer : MonoBehaviour
{
    public PlayerMovement player;
    public GameObject replacementModel;
    public SkinnedMeshRenderer characterRenderer;
    public SkinnedMeshRenderer replacementRenderer;
    public Transform targetRoot;
    public Transform sourceRoot;
    public string rootName = "spine";

    public ManualLogSource MageLogger = UnityModelReplacement.MageLogger;

    public void LoadModel(string assetPath)
    {
        replacementModel = Instantiate(UnityModelReplacement.ModBundle.LoadAsset<GameObject>(assetPath));
        replacementRenderer = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();
        replacementRenderer.updateWhenOffscreen = true;

        foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.transform.Find("wizardtrio").GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            skinnedMeshRenderer.sharedMesh = null;
        }

        this.transform.Find("armz/wizardarms/Plane.001").GetComponent<SkinnedMeshRenderer>().sharedMesh = null;

        if (player.IsOwner) Destroy(this); // No replacement character if first person
    }

    public void AssignModel()
    {
        switch (player.playername.ToLower())
        {
            case "dobby":
                LoadModel("Assets/_Modding/Dobby.prefab");
                break;
            case "gandalf":
            case "gandalf the grey":
                LoadModel("Assets/_Modding/Gandalf.prefab");
                break;
            case "hagrid":
            case "rubeus hagrid":
                LoadModel("Assets/_Modding/Hagrid.prefab");
                break;
            case "harry":
            case "harry potter":
                LoadModel("Assets/_Modding/Harry.prefab");
                break;
            case "ice king":
            case "simon":
            case "simon petrikov":
                LoadModel("Assets/_Modding/IceKing.prefab");
                break;
            case "mickey":
            case "mickey mouse":
                LoadModel("Assets/_Modding/Mickey.prefab");
                break;
            case "ron":
            case "ronald":
            case "ron weasley":
            case "ronald weasley":
                LoadModel("Assets/_Modding/Ron.prefab");
                break;
            default:
                MageLogger.LogInfo($"No match found for player {player.playername}!");
                Destroy(this);
                break;
        }
    }

    public IEnumerator CheckPlayerNameHasSet()
    {
        bool valid = false;
        while (!valid)
        {
            yield return new WaitForSeconds(1.0f);
            if (player.playername != null && player.playername != "") // I'm stupid and don't know which an uninitialized string is, so let's check both
            {
                valid = true;
            }
        }
        AssignModel();
    }

    public void Awake()
    {
        player = this.transform.GetComponent<PlayerMovement>();
        characterRenderer = player.wizardBody[player.playerTeam];

        if (UnityModelReplacement.ModBundle == null || player == null || characterRenderer == null)
        {
            Destroy(this);
        }

        StartCoroutine(CheckPlayerNameHasSet());
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
        MageLogger.LogInfo($"CharacterRenderer attached to {this.gameObject.name} was destroyed!");
        CancelInvoke();
        Destroy(replacementModel);
    }
}
