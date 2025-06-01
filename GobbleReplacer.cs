using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityModelReplacement
{
    public class GobbleReplacer : MonoBehaviour
    {
        public SkinnedMeshRenderer gobbleRenderer;

        public GameObject replacementModel;
        public SkinnedMeshRenderer replacementRenderer;

        public Transform targetRoot;
        public Transform sourceRoot;
        public Vector3 hipsOffset = new Vector3(0f, -0.01f, 0f);

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

            gobbleRenderer.forceRenderingOff = true;
        }

        public void Awake()
        {
            Debug.Log(gameObject.name);
            // Debug.Log(gameObject.active);

            if (UnityModelReplacement.AssetBundle == null)
            {
                Destroy(this);
            }

            gobbleRenderer = gameObject.transform.Find("Rotateable/Visuals/Goblet/GOB_BOD").GetComponent<SkinnedMeshRenderer>();
            if (gobbleRenderer != null)
            {
                LoadModel("Assets/CustomContent/Irastris/CookieMonster.prefab");
            }
            else
            {
                Debug.Log($"Failed to find gobbleRenderer for object {this.gameObject.name}!");
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
            sourceRoot = GetBoneTransformFromRenderer(gobbleRenderer, "Torso");
            targetRoot = GetBoneTransformFromRenderer(replacementRenderer, "Torso");
            if (sourceRoot == null || targetRoot == null) { Debug.Log("Failed to find root bone for either the source or target!"); return; }
            targetRoot.position = sourceRoot.position + hipsOffset;

            foreach (Transform targetBone in replacementRenderer.bones)
            {
                Transform sourceBone = GetBoneTransformFromRenderer(gobbleRenderer, targetBone.name);
                if (sourceBone == null) { Debug.Log($"Could not find {targetBone.name} on {gobbleRenderer}!"); continue; }

                targetBone.rotation = sourceBone.rotation;
            }
        }

        public void LateUpdate()
        {
            if (gobbleRenderer != null && replacementRenderer != null)
            {
                // replacementModel.SetActive(gameObject.activeInHierarchy);
                CopyPose();
            }
        }

        public void OnEnable()
        {
            if (replacementModel != null)
            {
                replacementModel.SetActive(true);
            }
        }

        public void OnDisable()
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
