using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityModelReplacement
{
    public class AvatarUpdater
    {
        protected GameObject player = null;
        protected GameObject replacement = null;
        protected Animator replacementAnimator = null;
        protected SkinnedMeshRenderer playerModelRenderer = null;

        protected static Dictionary<string, HumanBodyBones> rigMapping = new Dictionary<string, HumanBodyBones>()
        {
            {"Hip", HumanBodyBones.Hips},
            {"Torso", HumanBodyBones.Spine},
            {"Arm_L", HumanBodyBones.LeftUpperArm},
            {"Elbow_L", HumanBodyBones.LeftLowerArm},
            {"Hand_L", HumanBodyBones.LeftHand},
            {"Arm_R", HumanBodyBones.RightUpperArm},
            {"Elbow_R", HumanBodyBones.RightLowerArm},
            {"Hand_R", HumanBodyBones.RightHand},
            {"Head", HumanBodyBones.Head},
            {"Leg_L", HumanBodyBones.LeftUpperLeg},
            {"Knee_L", HumanBodyBones.LeftLowerLeg},
            {"Foot_L", HumanBodyBones.LeftFoot},
            {"Leg_R", HumanBodyBones.RightUpperLeg},
            {"Knee_R", HumanBodyBones.RightLowerLeg},
            {"Foot_R", HumanBodyBones.RightFoot},

            // TODO: Content Warning's rig does not distinguish between the left and right finger bones making mapping difficult. Solve for this.
            /*
            {"", HumanBodyBones.LeftLittleProximal},
            {"", HumanBodyBones.LeftLittleIntermediate},
            {"", HumanBodyBones.LeftRingProximal},
            {"", HumanBodyBones.LeftRingIntermediate},
            {"", HumanBodyBones.LeftMiddleProximal},
            {"", HumanBodyBones.LeftMiddleIntermediate},
            {"", HumanBodyBones.LeftIndexProximal},
            {"", HumanBodyBones.LeftIndexIntermediate},
            {"", HumanBodyBones.LeftThumbProximal},
            {"", HumanBodyBones.LeftThumbDistal},
            {"", HumanBodyBones.RightLittleProximal},
            {"", HumanBodyBones.RightLittleIntermediate},
            {"", HumanBodyBones.RightRingProximal},
            {"", HumanBodyBones.RightRingIntermediate},
            {"", HumanBodyBones.RightMiddleProximal},
            {"", HumanBodyBones.RightMiddleIntermediate},
            {"", HumanBodyBones.RightIndexProximal},
            {"", HumanBodyBones.RightIndexIntermediate},
            {"", HumanBodyBones.RightThumbProximal},
            {"", HumanBodyBones.RightThumbDistal},
            */
        };

        public void AssignModelReplacement(GameObject player, GameObject replacement)
        {
            this.player = player;
            this.replacement = replacement;
            this.replacementAnimator = replacement.GetComponentInChildren<Animator>();
            this.playerModelRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();

            return;
        }

        private Transform GetAvatarTransformFromBoneName(string boneName)
        {
            return rigMapping.TryGetValue(boneName, out HumanBodyBones avatarBone) ? replacementAnimator.GetBoneTransform(avatarBone) : null;
        }

        private Transform GetPlayerTransformFromBoneName(string boneName)
        {
            IEnumerable<Transform> playerBones = playerModelRenderer.bones.Where(x => x.name == boneName);
            return playerBones.Any() ? playerBones.First() : null;
        }

        private void UpdateModel()
        {
            Transform playerRootBone = GetPlayerTransformFromBoneName("Hip");
            Transform rootBone = GetAvatarTransformFromBoneName("Hip");
            rootBone.position = playerRootBone.position;

            foreach (Transform playerBone in playerModelRenderer.bones)
            {
                Transform modelBone = GetAvatarTransformFromBoneName(playerBone.name);
                if (modelBone == null) { continue; }

                modelBone.rotation = playerBone.rotation;
            }
        }

        public void Update()
        {
            if (playerModelRenderer == null || replacementAnimator == null)
            {
                return;
            }

            UpdateModel();
        }
    }

    public class PlayerReplacer : MonoBehaviour
    {
        public AvatarUpdater avatar;
        public GameObject replacementModel;

        public Material GenerateMaterial()
        {
            Shader niceShader = Shader.Find("NiceShader");
            Debug.Log(niceShader);
            Material replacementMat = new Material(niceShader);
            replacementMat.color = UnityEngine.Random.ColorHSV();
            return replacementMat;
        }

        private GameObject LoadModelFromAssetBundle(string assetPath)
        {
            MemoryStream memoryStream;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnityModelReplacement.suzanne.bundle"))
            {
                memoryStream = new MemoryStream((int)stream.Length);
                stream.CopyTo(memoryStream);
            }

            AssetBundle modBundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());

            return modBundle.LoadAsset<GameObject>(assetPath);
        }

        private GameObject LoadModelReplacement(string assetPath)
        {
            GameObject replacementModel = LoadModelFromAssetBundle(assetPath);
            replacementModel = Instantiate(replacementModel);

            SkinnedMeshRenderer[] skinnedRenderers = replacementModel.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
            {
                renderer.updateWhenOffscreen = true;

                List<Material> materials = ListPool<Material>.Get();
                renderer.GetSharedMaterials(materials);
                for (int i = 0; i < materials.Count; i++)
                {
                    materials[i] = GenerateMaterial();
                }
                renderer.SetMaterials(materials);
            }

            return replacementModel;
        }

        protected virtual void Awake()
        {
            replacementModel = LoadModelReplacement("Assets/_Modding/Temp.prefab");
            avatar = new AvatarUpdater();
            avatar.AssignModelReplacement(gameObject, replacementModel);

            SkinnedMeshRenderer[] skinnedRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedRenderers)
            {
                renderer.forceRenderingOff = true;
            }
        }

        public virtual void LateUpdate()
        {
            avatar.Update();
        }
    }
}
