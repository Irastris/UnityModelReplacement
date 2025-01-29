using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityModelReplacement
{
    public class PlayerReplacer : MonoBehaviour
    {
        public SkinnedMeshRenderer playerRenderer = null;

        public GameObject replacementPrefab = null;
        public SkinnedMeshRenderer replacementRenderer = null;

        public string cachedPlayerName = "";
        public string playerName
        {
            get { return cachedPlayerName; }
            set
            {
                if (cachedPlayerName != value)
                {
                    cachedPlayerName = value;
                    LoadModelByPlayerName();
                }
            }
        }

        public void LoadModel(string assetPath)
        {
            if (replacementPrefab != null)
            {
                Destroy(replacementPrefab);
            }

            replacementPrefab = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementPrefab.SetActive(false);
            SceneManager.MoveGameObjectToScene(replacementPrefab, SceneManager.GetSceneByName("Players"));
            replacementRenderer = replacementPrefab.transform.Find("Model").GetComponent<SkinnedMeshRenderer>();

            playerRenderer.sharedMesh = replacementRenderer.sharedMesh;
            playerRenderer.material.SetTexture("_BaseColorMap", replacementRenderer.material.GetTexture("_BaseColorMap"));
            playerRenderer.material.DisableKeyword("_NORMALMAP");
            playerRenderer.material.DisableKeyword("_MASKMAP");

            foreach (MeshRenderer renderer in gameObject.transform.Find("insanityPlayerModel/PlayerModel/ZortRagdollPlayer/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head").GetComponentsInChildren<MeshRenderer>())
            {
                renderer.forceRenderingOff = true;
            }
        }

        public void LoadModelByPlayerName()
        {
            switch (playerName)
            {
                case "Daphne": case "Daphne Blake":
                    LoadModel("Assets/_Modding/Daphne.prefab");
                    break;
                case "Fred": case "Fred Jones":
                    LoadModel("Assets/_Modding/Fred.prefab");
                    break;
                case "Scooby": case "Scooby Doo":
                    LoadModel("Assets/_Modding/Scooby.prefab");
                    break;
                case "Shaggy": case "Shaggy Rogers":
                    LoadModel("Assets/_Modding/Shaggy.prefab");
                    break;
                case "Velma": case "Velma Dinkley":
                    LoadModel("Assets/_Modding/Velma.prefab");
                    break;
                default:
                    if (replacementPrefab != null)
                    {
                        Destroy(replacementPrefab);
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

            playerRenderer = gameObject.transform.Find("insanityPlayerModel/PlayerModel/ZortRagdollPlayer/Zort_Character_Low").GetComponent<SkinnedMeshRenderer>();
            playerName = gameObject.GetComponent<PlayerController>().PlayerData.username;
        }

        public void OnDestroy()
        {
            CancelInvoke();
            Destroy(replacementPrefab);
        }
    }
}
