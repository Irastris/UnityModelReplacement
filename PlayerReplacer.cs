using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityModelReplacement
{
    public class PlayerReplacer : MonoBehaviour
    {
        public GameObject replacementModel = null;
        public Animator replacementAnimator = null;

        public EnemyParent enemyParent = null;
        public SkinnedMeshRenderer enemyRenderer = null;

        public static Dictionary<string, HumanBodyBones> rigMapping = new Dictionary<string, HumanBodyBones>()
        {
            {"HIPS", HumanBodyBones.Hips},
            {"MIDRIF", HumanBodyBones.Spine},
            {"CHEST", HumanBodyBones.Chest},
            {"LEFT_SHOULDER", HumanBodyBones.LeftShoulder},
            {"LEFT_ARM", HumanBodyBones.LeftUpperArm},
            {"LEFT_ELBOW", HumanBodyBones.LeftLowerArm},
            {"LEFT_HAND", HumanBodyBones.LeftHand},
            {"LEFT_PINK", HumanBodyBones.LeftLittleProximal},
            {"LEFT_PINK2", HumanBodyBones.LeftLittleIntermediate},
            // {"LEFT_PINK3", HumanBodyBones.LeftLittleDistal},
            {"LEFT_MIDDLE", HumanBodyBones.LeftMiddleProximal},
            {"LEFT_MIDDLE2", HumanBodyBones.LeftMiddleIntermediate},
            // {"LEFT_MIDDLE3", HumanBodyBones.LeftMiddleDistal},
            {"LEFT_INDEX", HumanBodyBones.LeftIndexProximal},
            {"LEFT_INDEX2", HumanBodyBones.LeftIndexIntermediate},
            // {"LEFT_INDEX3", HumanBodyBones.LeftIndexDistal},
            {"LEFT_THUMB", HumanBodyBones.LeftThumbProximal},
            {"LEFT_THUMB2", HumanBodyBones.LeftThumbIntermediate},
            // {"LEFT_THUMB3", HumanBodyBones.LeftThumbDistal},
            {"RIGHT_SHOULDER", HumanBodyBones.RightShoulder},
            {"RIGHT_ARM", HumanBodyBones.RightUpperArm},
            {"RIGHT_ELBOW", HumanBodyBones.RightLowerArm},
            {"RIGHT_HAND", HumanBodyBones.RightHand},
            {"RIGHT_PINK", HumanBodyBones.RightLittleProximal},
            {"RIGHT_PINK2", HumanBodyBones.RightLittleIntermediate},
            // {"RIGHT_PINK3", HumanBodyBones.RightLittleDistal},
            {"RIGHT_MIDDLE", HumanBodyBones.RightMiddleProximal},
            {"RIGHT_MIDDLE2", HumanBodyBones.RightMiddleIntermediate},
            // {"RIGHT_MIDDLE3", HumanBodyBones.RightMiddleDistal},
            {"RIGHT_INDEX", HumanBodyBones.RightIndexProximal},
            {"RIGHT_INDEX2", HumanBodyBones.RightIndexIntermediate},
            // {"RIGHT_INDEX3", HumanBodyBones.RightIndexDistal},
            {"RIGHT_THUMB", HumanBodyBones.RightThumbProximal},
            {"RIGHT_THUMB2", HumanBodyBones.RightThumbIntermediate},
            // {"RIGHT_THUMB3", HumanBodyBones.RightThumbDistal},
            {"NECK", HumanBodyBones.Neck},
            {"HEAD", HumanBodyBones.Head},
            {"LEFT_LEG", HumanBodyBones.LeftUpperLeg},
            {"LEFT_KNEE", HumanBodyBones.LeftLowerLeg},
            {"LEFT_ANKLE", HumanBodyBones.LeftFoot},
            {"RIGHT_LEG", HumanBodyBones.RightUpperLeg},
            {"RIGHT_KNEE", HumanBodyBones.RightLowerLeg},
            {"RIGHT_ANKLE", HumanBodyBones.RightFoot},
        };

        /*
        public void ToggleRenderers(bool shouldBeHidden)
        {
            foreach (SkinnedMeshRenderer renderer in gameObject.transform.Find("CharacterModel").GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.forceRenderingOff = shouldBeHidden;
            }

            gameObject.transform.Find("HeadPosition/FACE").GetComponent<MeshRenderer>().forceRenderingOff = shouldBeHidden;
        }
        */

        public void LoadModel(string assetPath)
        {
            if (replacementModel != null)
            {
                Destroy(replacementModel);
            }

            replacementModel = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementAnimator = replacementModel.GetComponentInChildren<Animator>();

            /*
            foreach (SkinnedMeshRenderer renderer in replacementModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.updateWhenOffscreen = true;

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].shader = Shader.Find("Universal Render Pipeline/Lit");
                }
            }
            */

            enemyRenderer.forceRenderingOff = true; // ToggleRenderers(true);
        }

        public void LoadModelByEnemyName(string enemyName)
        {
            switch (enemyName)
            {
                case "Norman_Model":
                    LoadModel("Assets/CustomContent/Irastris/NormanTest.prefab");
                    break;
                default:
                    if (replacementModel != null)
                    {
                        Destroy(replacementModel);
                        enemyRenderer.forceRenderingOff = false; // ToggleRenderers(false);
                    }
                    break;
            }
        }

        public void Awake()
        {
            if (UnityModelReplacement.AssetBundle == null)
            {
                Destroy(this);
            }

            enemyParent = gameObject.GetComponent<EnemyParent>();
            enemyRenderer = enemyParent.myRenderer;
            LoadModelByEnemyName(enemyRenderer.name);
        }

        public Transform GetAvatarTransformFromBoneName(string boneName)
        {
            return rigMapping.TryGetValue(boneName, out HumanBodyBones avatarBone) ? replacementAnimator.GetBoneTransform(avatarBone) : null;
        }

        public Transform GetPlayerTransformFromBoneName(string boneName)
        {
            IEnumerable<Transform> playerBones = enemyRenderer.bones.Where(x => x.name == boneName);

            return playerBones.Any() ? playerBones.First() : null;
        }

        public void CopyPose()
        {
            Transform playerRootBone = GetPlayerTransformFromBoneName("HIPS");
            Transform rootBone = GetAvatarTransformFromBoneName("HIPS");
            rootBone.position = playerRootBone.position;

            foreach (Transform playerBone in enemyRenderer.bones)
            {
                Transform modelBone = GetAvatarTransformFromBoneName(playerBone.name);
                if (modelBone == null) { continue; }

                modelBone.rotation = playerBone.rotation;
            }
        }

        public void LateUpdate()
        {
            if (enemyRenderer != null && replacementAnimator != null)
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
