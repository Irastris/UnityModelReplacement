using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityModelReplacement.AvatarBodyUpdater;
using UnityModelReplacement.Scripts;

namespace UnityModelReplacement
{
    public abstract class BodyReplacementBase : MonoBehaviour
    {
        //Base components
        public AvatarUpdater avatar { get; private set; }
        public GameObject replacementModel;
        public GameObject replacementViewModel;

        //Misc components
        private MaterialHelper matHelper = null;

        protected abstract GameObject LoadAssetsAndReturnModel(); // Loads necessary assets from assetBundle, perform any necessary modifications on the replacement model and return it.

        protected virtual AvatarUpdater GetAvatarUpdater() // Override this to return a derivative AvatarUpdater. Only do this if you really know what you are doing. 
        {
            return new AvatarUpdater();
        }

        private GameObject LoadModelReplacement()
        {
            // Load models
            GameObject tempReplacementModel = LoadAssetsAndReturnModel();
            if (tempReplacementModel == null)
            {
                UnityModelReplacement.Instance.Logger.LogFatal("LoadAssetsAndReturnModel() returned null. Verify that your assetbundle works and your asset name is correct. ");
            }

            //Offset Builder Data
            Animator replacementAnimator = tempReplacementModel.GetComponentInChildren<Animator>();
            OffsetBuilder offsetBuilder = replacementAnimator.gameObject.GetComponent<OffsetBuilder>();
            Vector3 rootScale = offsetBuilder.rootScale;

            /*
            // Fix Materials and renderers
            // Find some way to get the ingame material to translate unsupported materials. 
            Material gameMat = controller.gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
            gameMat = new Material(gameMat); // Copy so that shared material isn't accidently changed by overriders of GetReplacementMaterial()

            Renderer[] renderers = tempReplacementModel.GetComponentsInChildren<Renderer>();
            SkinnedMeshRenderer[] skinnedRenderers = tempReplacementModel.GetComponentsInChildren<SkinnedMeshRenderer>();
            Dictionary<Material, Material> matMap = new();
            List<Material> materials = ListPool<Material>.Get();
            foreach (Renderer renderer in renderers)
            {
                renderer.GetSharedMaterials(materials);
                for (int i = 0; i < materials.Count; i++)
                {
                    Material mat = materials[i];
                    if (!matMap.TryGetValue(mat, out Material replacementMat))
                    {
                        matMap[mat] = replacementMat = matHelper.GetReplacementMaterial(gameMat, mat);
                    }
                    materials[i] = replacementMat;
                }
                renderer.SetMaterials(materials);
            }
            ListPool<Material>.Release(materials);
            foreach (SkinnedMeshRenderer item in skinnedRenderers)
            {
                item.updateWhenOffscreen = true;
            }
            */

            // Sets y extents to the same size for player body and extents.
            // Get a new hardcoded player height
            float playerHeight = 1.465f; // Hardcode player height to account for emote mods. 
            float scale = playerHeight / GetBounds(tempReplacementModel).extents.y;
            tempReplacementModel.transform.localScale *= scale;

            Vector3 baseScale = tempReplacementModel.transform.localScale;
            tempReplacementModel.transform.localScale = Vector3.Scale(baseScale, rootScale);

            // Instantiate model
            tempReplacementModel = Instantiate(tempReplacementModel);
            tempReplacementModel.transform.localPosition = new Vector3(0, 0, 0);
            tempReplacementModel.SetActive(true);
            return tempReplacementModel;
        }

        protected virtual void Awake()
        {
            matHelper = new MaterialHelper(); // basic
            replacementModel = LoadModelReplacement(); // Load Models 
            avatar = GetAvatarUpdater(); // Assign avatars
            // avatar.AssignModelReplacement(controller.gameObject, replacementModel); // Get the character object through an analog for the PlayerVisual, or otherwise
            SetAvatarRenderers(true); // Misc
        }

        public virtual void LateUpdate()
        {
            avatar.Update(); // Update replacement models
        }

        protected virtual void OnDestroy()
        {
            Destroy(replacementModel);
            Destroy(replacementViewModel);
        }

        public void SetAvatarRenderers(bool enabled)
        {
            foreach (Renderer renderer in replacementModel.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = enabled;
            }
            if (replacementViewModel != null)
            {
                foreach (Renderer renderer in replacementViewModel.GetComponentsInChildren<Renderer>())
                {
                    renderer.enabled = enabled;
                }
            }
        }

        private Bounds GetBounds(GameObject model)
        {
            Bounds bounds = new Bounds();
            IEnumerable<Bounds> allBounds = model.GetComponentsInChildren<SkinnedMeshRenderer>().Select(r => r.bounds);

            float maxX = allBounds.OrderByDescending(x => x.max.x).First().max.x;
            float maxY = allBounds.OrderByDescending(x => x.max.y).First().max.y;
            float maxZ = allBounds.OrderByDescending(x => x.max.z).First().max.z;

            float minX = allBounds.OrderBy(x => x.min.x).First().min.x;
            float minY = allBounds.OrderBy(x => x.min.y).First().min.y;
            float minZ = allBounds.OrderBy(x => x.min.z).First().min.z;


            bounds.SetMinMax(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
            return bounds;
        }
    }
}
