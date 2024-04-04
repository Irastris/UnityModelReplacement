using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityModelReplacement.AvatarBodyUpdater
{
    public class AvatarUpdater
    {
        protected SkinnedMeshRenderer playerModelRenderer = null;
        protected Animator replacementAnimator = null;
        protected GameObject player = null;
        protected GameObject replacement = null;

        protected Vector3 rootPositionOffset = Vector3.zero;

        public virtual void AssignModelReplacement(GameObject player, GameObject replacement)
        {
            playerModelRenderer = null;

            this.player = player;
            replacementAnimator = replacement.GetComponentInChildren<Animator>();
            this.replacement = replacement;

            OffsetBuilder offsetBuilder = replacementAnimator.gameObject.GetComponent<OffsetBuilder>();
            rootPositionOffset = offsetBuilder.rootPositionOffset;

            // Transform upperChestTransform = replacementAnimator.GetBoneTransform(HumanBodyBones.UpperChest);
        }

        protected virtual void UpdateModel()
        {
            Transform rootBone = GetAvatarTransformFromBoneName("Hip");
            Transform playerRootBone = GetPlayerTransformFromBoneName("Hip");
            rootBone.position = playerRootBone.position + playerRootBone.TransformVector(rootPositionOffset);

            foreach (Transform playerBone in playerModelRenderer.bones)
            {
                Transform modelBone = GetAvatarTransformFromBoneName(playerBone.name);
                if (modelBone == null) { continue; }

                modelBone.rotation = playerBone.rotation;
                RotationOffset offset = modelBone.GetComponent<RotationOffset>();
                if (offset) { modelBone.rotation *= offset.offset; }
            }
        }

        public void Update()
        {
            if (playerModelRenderer == null || replacementAnimator == null) { return; }
            UpdateModel();
        }

        public Transform GetAvatarTransformFromBoneName(string boneName)
        {
            return modelToAvatarBone.TryGetValue(boneName, out HumanBodyBones avatarBone) ? replacementAnimator.GetBoneTransform(avatarBone) : null;
        }

        public Transform GetPlayerTransformFromBoneName(string boneName)
        {
            IEnumerable<Transform> playerBones = playerModelRenderer.bones.Where(x => x.name == boneName);
            return playerBones.Any() ? playerBones.First() : null;
        }

        protected static Dictionary<string, HumanBodyBones> modelToAvatarBone = new Dictionary<string, HumanBodyBones>()
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

            // TODO: The following bones either do not exist on Content Warning's rig, or do not distinguish between left and right making mapping difficult
            // {"shoulder.L" , HumanBodyBones.LeftShoulder},
            // {"shoulder.R" , HumanBodyBones.RightShoulder},
            // {"toe.L" , HumanBodyBones.LeftToes},
            // {"toe.R" , HumanBodyBones.RightToes},
            // {"finger5.L" , HumanBodyBones.LeftLittleProximal},
            // {"finger5.L.001" , HumanBodyBones.LeftLittleIntermediate},
            // {"finger4.L" , HumanBodyBones.LeftRingProximal},
            // {"finger4.L.001" , HumanBodyBones.LeftRingIntermediate},
            // {"finger3.L" , HumanBodyBones.LeftMiddleProximal},
            // {"finger3.L.001" , HumanBodyBones.LeftMiddleIntermediate},
            // {"finger2.L" , HumanBodyBones.LeftIndexProximal},
            // {"finger2.L.001" , HumanBodyBones.LeftIndexIntermediate},
            // {"finger1.L" , HumanBodyBones.LeftThumbProximal},
            // {"finger1.L.001" , HumanBodyBones.LeftThumbDistal},
            // {"finger5.R" , HumanBodyBones.RightLittleProximal},
            // {"finger5.R.001" , HumanBodyBones.RightLittleIntermediate},
            // {"finger4.R" , HumanBodyBones.RightRingProximal},
            // {"finger4.R.001" , HumanBodyBones.RightRingIntermediate},
            // {"finger3.R" , HumanBodyBones.RightMiddleProximal},
            // {"finger3.R.001" , HumanBodyBones.RightMiddleIntermediate},
            // {"finger2.R" , HumanBodyBones.RightIndexProximal},
            // {"finger2.R.001" , HumanBodyBones.RightIndexIntermediate},
            // {"finger1.R" , HumanBodyBones.RightThumbProximal},
            // {"finger1.R.001" , HumanBodyBones.RightThumbDistal},
        };
    }
}