using UnityEngine;

namespace UnityModelReplacement
{
    public class CinematicReplacer : MonoBehaviour
    {
        public GameObject replacementPrefab = null;

        public void LoadPrefab(string assetPath)
        {
            if (replacementPrefab != null)
            {
                Destroy(replacementPrefab);
            }

            replacementPrefab = Instantiate(UnityModelReplacement.AssetBundle.LoadAsset<GameObject>(assetPath));
            replacementPrefab.SetActive(false);
        }

        public void Awake()
        {
            if (UnityModelReplacement.AssetBundle == null)
            {
                Destroy(this);
            }

            if (gameObject.name != "GirisSinematik" || gameObject.scene.name != "ZortaganeMapOlan")
            {
                Destroy(this);
            }

            LoadPrefab("Assets/_Modding/MysteryGangCinematicA.prefab");

            SkinnedMeshRenderer p1Renderer = gameObject.transform.Find("Player1/Zort_Character_Low").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer p2Renderer = gameObject.transform.Find("Player2/Zort_Character_Low").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer p3Renderer = gameObject.transform.Find("Player3/Zort_Character_Low").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer p4Renderer = gameObject.transform.Find("Player4/Zort_Character_Low").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer[] pRenderers = [p1Renderer, p2Renderer, p3Renderer, p4Renderer];

            SkinnedMeshRenderer mg1Renderer = replacementPrefab.transform.Find("Fred/Model").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer mg2Renderer = replacementPrefab.transform.Find("Daphne/Model").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer mg3Renderer = replacementPrefab.transform.Find("Shaggy/Model").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer mg4Renderer = replacementPrefab.transform.Find("Velma/Model").GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer[] mgRenderers = [mg1Renderer, mg2Renderer, mg3Renderer, mg4Renderer];

            for (int i = 0; i < pRenderers.Length; i++)
            {
                pRenderers[i].sharedMesh = mgRenderers[i].sharedMesh;

                pRenderers[i].material.SetTexture("_BaseColorMap", mgRenderers[i].material.GetTexture("_BaseColorMap"));
                pRenderers[i].material.DisableKeyword("_NORMALMAP");
                pRenderers[i].material.DisableKeyword("_MASKMAP");

                pRenderers[i].transform.parent.Find("Armature/MainRoot/Root/Spine/Spine1/Spine2/Neck/Head/GasMask").gameObject.SetActive(false);
            }
        }

    public void OnDestroy()
        {
            CancelInvoke();
            Destroy(replacementPrefab);
        }
    }
}
