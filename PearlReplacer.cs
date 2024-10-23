using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityModelReplacement
{
    public class PearlReplacer : MonoBehaviour
    {
        public PearlAnimationController pearl;
        public Animator pearlAnimator;
        public SkinnedMeshRenderer pearlRenderer;

        public GameObject replacementModel;
        public SkinnedMeshRenderer replacementRenderer;

        public Transform sourceRoot;
        public Transform targetRoot;

        public void LoadModel(string assetPath)
        {
            if (replacementModel != null)
            {
                Destroy(replacementModel);
            }

            replacementModel = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementRenderer = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in replacementModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.updateWhenOffscreen = true;
            }

            foreach (MeshRenderer renderer in pearl.gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.forceRenderingOff = true;
            }
            foreach (SkinnedMeshRenderer renderer in pearl.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.forceRenderingOff = true;
            }
        }

        public void Awake()
        {
            if (UnityModelReplacement.AssetBundle == null)
            {
                Destroy(this);
            }

            pearl = gameObject.GetComponent<PearlAnimationController>();
            pearlAnimator = Traverse.Create(pearl).Field("myAnimator").GetValue() as Animator;
            pearlRenderer = pearlAnimator.transform.Find("Pearl_Body").GetComponent<SkinnedMeshRenderer>();
            if (pearl != null && pearlAnimator != null && pearlRenderer != null)
            {
                LoadModel("Assets/CustomContent/Irastris/BigBird.prefab");
            }
            else
            {
                Debug.Log($"Failed to find all required objects for {this.gameObject.name}!");
                Destroy(this);
            }
        }

        public Transform GetBoneTransformFromRenderer(SkinnedMeshRenderer renderer, string boneName)
        {
            IEnumerable<Transform> bones = renderer.bones.Where(x => x.name == boneName);

            return bones.Any() ? bones.First() : null;
        }

        public void CopyPose()
        {
            sourceRoot = GetBoneTransformFromRenderer(pearlRenderer, "Chest");
            targetRoot = GetBoneTransformFromRenderer(replacementRenderer, "Chest");
            targetRoot.position = sourceRoot.position;

            foreach (Transform targetBone in replacementRenderer.bones)
            {
                Transform sourceBone = GetBoneTransformFromRenderer(pearlRenderer, targetBone.name);
                if (sourceBone == null) { continue; }

                targetBone.rotation = sourceBone.rotation;
            }
        }

        public void LateUpdate()
        {
            if (pearlRenderer != null && replacementRenderer != null)
            {
                replacementModel.SetActive(gameObject.activeSelf);
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
