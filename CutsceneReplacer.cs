using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace UnityModelReplacement
{
    public class CutsceneReplacer : MonoBehaviour
    {
        public PlayableDirector playableDirector;
        public SkinnedMeshRenderer playableRenderer;

        public GameObject replacementModel;
        public SkinnedMeshRenderer replacementRenderer;

        public string rootBoneName;
        public Transform targetRoot;
        public Transform sourceRoot;
        public Vector3 hipsOffset = new Vector3();

        public void LoadModel(string assetPath)
        {
            replacementModel = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementRenderer = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in replacementModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.updateWhenOffscreen = true;
            }

            playableRenderer.forceRenderingOff = true;
        }

        public void LoadModelByPlayableAsset()
        {
            switch (playableDirector.playableAsset.name)
            {
                case "HotelBottom-RickyCutscene":
                    rootBoneName = "Base";
                    playableRenderer = GameObject.Find("/RIG-Ricky-BakedAnimations-Cutscenes/Ricky").GetComponent<SkinnedMeshRenderer>();
                    LoadModel("Assets/CustomContent/Irastris/Jeffy.prefab");
                    break;
                case "Pearls-Rolodexer-Room-Ricky-Gordon":
                    rootBoneName = "Base";
                    playableRenderer = GameObject.Find("/RIG-Ricky-BakedAnimations/Ricky").GetComponent<SkinnedMeshRenderer>();
                    LoadModel("Assets/CustomContent/Irastris/Jeffy.prefab");
                    break;
                default:
                    Debug.Log($"No replacement model found for playable director {playableDirector.playableAsset.name}!");
                    Destroy(this);
                    break;
            }
        }

        public void Awake()
        {
            if (UnityModelReplacement.AssetBundle == null)
            {
                Destroy(this);
            }

            playableDirector = gameObject.GetComponent<PlayableDirector>();
            if (playableDirector != null)
            {
                LoadModelByPlayableAsset();
            }
            else
            {
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
            targetRoot = GetBoneTransformFromRenderer(replacementRenderer, rootBoneName);
            sourceRoot = GetBoneTransformFromRenderer(playableRenderer, rootBoneName);
            if (sourceRoot == null || targetRoot == null) { Debug.Log("Failed to find root bone for either the source or target!"); return; }
            targetRoot.position = sourceRoot.position + hipsOffset;

            foreach (Transform targetBone in replacementRenderer.bones)
            {
                if (targetBone == null) continue;
                Transform sourceBone = GetBoneTransformFromRenderer(playableRenderer, targetBone.name);
                if (sourceBone == null) { Debug.Log($"Could not find {targetBone.name} on {playableRenderer}!"); continue; }

                targetBone.rotation = sourceBone.rotation;
            }

            return;
        }

        public void LateUpdate()
        {
            if (playableRenderer != null && replacementRenderer != null)
            {
                CopyPose();
            }
        }

        public void OnDestroy()
        {
            CancelInvoke();
            Destroy(replacementModel);
        }
    }
}
