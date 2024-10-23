using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityModelReplacement
{
    public class EnemyReplacer : MonoBehaviour
    {
        public EnemyParent enemyParent;
        public SkinnedMeshRenderer enemyRenderer;

        public GameObject replacementModel;
        public SkinnedMeshRenderer replacementRenderer;

        public Transform targetRoot;
        public Transform sourceRoot;
        public Vector3 hipsOffset = new Vector3();

        public string[] blacklist = ["Rotateable", "GobbleInWorld"];

        public void LoadModel(string assetPath)
        {
            replacementModel = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementRenderer = replacementModel.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer renderer in replacementModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.updateWhenOffscreen = true;
            }

            enemyRenderer.forceRenderingOff = true;

            if (enemyRenderer.name == "George_Body")
            {
                enemyRenderer.transform.parent.Find("ROOT/BELLY/MIDRIF/CHEST/NECK/HEAD/JAW/TOP_LIP/George_Hat").GetComponent<MeshRenderer>().forceRenderingOff = true;
            }
        }

        public void LoadModelByEnemyName(string enemyName)
        {
            switch (enemyName)
            {
                case "Norman_Model":
                    LoadModel("Assets/CustomContent/Irastris/Kermit.prefab");
                    hipsOffset = new Vector3(0f, 0.17f, 0f);
                    break;
                case "Lenard":
                    LoadModel("Assets/CustomContent/Irastris/Ernie.prefab");
                    break;
                case "Junebug_Model":
                    LoadModel("Assets/CustomContent/Irastris/MissPiggy.prefab");
                    break;
                case "George_Body":
                    LoadModel("Assets/CustomContent/Irastris/Elmo.prefab");
                    break;
                default:
                    Debug.Log($"No replacement model found for renderer {enemyName}!");
                    Destroy(this);
                    break;
            }
        }

        public void Awake()
        {
            if (UnityModelReplacement.AssetBundle == null || blacklist.Contains(this.gameObject.name))
            {
                Destroy(this);
            }

            enemyParent = gameObject.GetComponent<EnemyParent>();
            enemyRenderer = Traverse.Create(enemyParent).Field("myRenderer").GetValue() as SkinnedMeshRenderer;
            if (enemyParent != null && enemyRenderer != null)
            {
                LoadModelByEnemyName(enemyRenderer.name);
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
            targetRoot = GetBoneTransformFromRenderer(replacementRenderer, "HIPS");
            sourceRoot = GetBoneTransformFromRenderer(enemyRenderer, "HIPS");
            if (sourceRoot == null || targetRoot == null) { Debug.Log("Failed to find root bone for either the source or target!"); return; }
            targetRoot.position = sourceRoot.position + hipsOffset;

            foreach (Transform targetBone in replacementRenderer.bones)
            {
                Transform sourceBone = GetBoneTransformFromRenderer(enemyRenderer, targetBone.name);
                if (sourceBone == null) { Debug.Log($"Could not find {targetBone.name} on {enemyRenderer}!"); continue; }

                targetBone.rotation = sourceBone.rotation;
            }

            return;
        }

        public void LateUpdate()
        {
            if (enemyRenderer != null && replacementRenderer != null)
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
