using UnityEngine;

namespace UnityModelReplacement
{
    public class SkinwalkerReplacer : MonoBehaviour
    {
        public SkinnedMeshRenderer skinwalkerRenderer = null;

        public GameObject replacementPrefab = null;
        public SkinnedMeshRenderer replacementRenderer = null;

        public void LoadModel(string assetPath)
        {
            if (replacementPrefab != null)
            {
                Destroy(replacementPrefab);
            }

            replacementPrefab = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementPrefab.SetActive(false);
            replacementRenderer = replacementPrefab.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();

            skinwalkerRenderer.sharedMesh = replacementRenderer.sharedMesh;
            skinwalkerRenderer.material.SetTexture("_BaseColorMap", replacementRenderer.material.GetTexture("_BaseColorMap"));
            skinwalkerRenderer.material.DisableKeyword("_NORMALMAP");
            skinwalkerRenderer.material.DisableKeyword("_MASKMAP");

            foreach (MeshRenderer renderer in gameObject.transform.Find("PlayerModel/ZortRagdollPlayer/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").GetComponentsInChildren<MeshRenderer>())
            {
                renderer.forceRenderingOff = true;
            }
        }

        public void LoadRandomModel()
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    LoadModel("Assets/_Modding/Daphne.prefab");
                    break;
                case 1:
                    LoadModel("Assets/_Modding/Fred.prefab");
                    break;
                case 2:
                    LoadModel("Assets/_Modding/Scooby.prefab");
                    break;
                case 3:
                    LoadModel("Assets/_Modding/Shaggy.prefab");
                    break;
                case 4:
                    LoadModel("Assets/_Modding/Velma.prefab");
                    break;
                default:
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

            skinwalkerRenderer = gameObject.transform.Find("PlayerModel/ZortRagdollPlayer/Zort_Character_Low").GetComponent<SkinnedMeshRenderer>();

            LoadRandomModel();
        }

        public void OnDestroy()
        {
            CancelInvoke();
            Destroy(replacementPrefab);
        }
    }
}
