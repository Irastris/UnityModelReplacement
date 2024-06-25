using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityModelReplacement
{
    public class PlayerReplacer : MonoBehaviour
    {
        public SkinnedMeshRenderer playerRenderer = null;
        public PlayerVisor playerVisor = null;
        public GameObject replacementModel = null;
        public GameObject replacementHead = null;
        public GameObject replacementHeadShadow = null;
        public Animator replacementAnimator = null;

        public int cachedVisorColor = -1;
        public int visorColor
        {
            get { return cachedVisorColor; }
            set
            {
                if (cachedVisorColor != value)
                {
                    cachedVisorColor = value;
                    LoadModelByVisorColor();
                }
            }
        }

        public static Dictionary<string, HumanBodyBones> rigMapping = new Dictionary<string, HumanBodyBones>()
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

        public void DisableBloom()
        {
            GameObject postProcessing = GameObject.Find("GAME/Rendering/Post");
            if (postProcessing != null)
            {
                postProcessing.GetComponent<Volume>().profile.components[2].active = false;
            }
        }

        public void ToggleRenderers(bool shouldBeHidden)
        {
            foreach (SkinnedMeshRenderer renderer in gameObject.transform.Find("CharacterModel").GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.forceRenderingOff = shouldBeHidden;
            }

            gameObject.transform.Find("HeadPosition/FACE").GetComponent<MeshRenderer>().forceRenderingOff = shouldBeHidden;
        }

        public void LoadModel(string assetPath)
        {
            if (replacementModel != null)
            {
                Destroy(replacementModel);
            }

            replacementModel = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementHead = replacementModel.transform.Find("Head").gameObject;
            replacementHeadShadow = replacementModel.transform.Find("ShadowHead").gameObject;
            replacementAnimator = replacementModel.GetComponentInChildren<Animator>();

            foreach (SkinnedMeshRenderer renderer in replacementModel.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                renderer.updateWhenOffscreen = true;

                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    renderer.materials[i].shader = Shader.Find("Universal Render Pipeline/Lit");
                }
            }

            if (gameObject.GetComponent<Player>().IsLocal)
            {
                replacementHead.layer = LayerMask.NameToLayer("LocalDontSee");
            }
            else
            {
                replacementHeadShadow.SetActive(false); // No need to double up on shadow casting for non-local player replacements
            }

            ToggleRenderers(true);
        }

        public void LoadModelByVisorColor()
        {
            switch (cachedVisorColor)
            {
                case 0: // Yellow
                    LoadModel("Assets/_Modding/ModelA.prefab");
                    break;
                case 1: // Orange
                    LoadModel("Assets/_Modding/ModelB.prefab");
                    break;
                case 2: // Red
                    LoadModel("Assets/_Modding/ModelC.prefab");
                    break;
                case 3: // Pink
                    LoadModel("Assets/_Modding/ModelD.prefab");
                    break;
                case 4: // Blue
                case 5: // Teal
                case 6: // Green
                default:
                    if (replacementModel != null)
                    {
                        Destroy(replacementModel);
                        ToggleRenderers(false);
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

            playerRenderer = gameObject.transform.Find("CharacterModel/BodyRenderer").GetComponent<SkinnedMeshRenderer>();
            playerVisor = gameObject.GetComponent<PlayerVisor>();

            // TODO: Lazy fix for overly bright URP Lit materials, do something better
            InvokeRepeating("DisableBloom", 0f, 1.0f);
        }

        public Transform GetAvatarTransformFromBoneName(string boneName)
        {
            return rigMapping.TryGetValue(boneName, out HumanBodyBones avatarBone) ? replacementAnimator.GetBoneTransform(avatarBone) : null;
        }

        public Transform GetPlayerTransformFromBoneName(string boneName)
        {
            IEnumerable<Transform> playerBones = playerRenderer.bones.Where(x => x.name == boneName);

            return playerBones.Any() ? playerBones.First() : null;
        }

        public void CopyPose()
        {
            Transform playerRootBone = GetPlayerTransformFromBoneName("Hip");
            Transform rootBone = GetAvatarTransformFromBoneName("Hip");
            rootBone.position = playerRootBone.position;

            foreach (Transform playerBone in playerRenderer.bones)
            {
                Transform modelBone = GetAvatarTransformFromBoneName(playerBone.name);
                if (modelBone == null) { continue; }

                modelBone.rotation = playerBone.rotation;
            }
        }

        public void LateUpdate()
        {
            if (playerVisor != null)
            {
                visorColor = playerVisor.visorColorIndex;
            }

            if (playerRenderer != null && replacementAnimator != null)
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
