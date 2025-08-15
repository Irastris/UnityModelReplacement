using System.Collections;
using UnityEngine;

namespace UnityModelReplacement;

public class BingBongReplacer : MonoBehaviour
{
    public SkinnedMeshRenderer characterRenderer;

    public IEnumerator UpdateBingBong()
    {
        while (true)
        {
            if (UnityModelReplacement.ModBundle != null)
            {
                Mesh stewieMesh = UnityModelReplacement.ModBundle.LoadAsset<Mesh>("Assets/_Modding/StewieGriffin.mesh");
                Texture2D stewieTex = UnityModelReplacement.ModBundle.LoadAsset<Texture2D>("Assets/_Modding/StewieGriffin.png");

                foreach (MeshRenderer renderer in this.transform.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (renderer.transform.name == "Cube")
                    {
                        MeshFilter meshFilter = renderer.transform.GetComponent<MeshFilter>();
                        meshFilter.sharedMesh = stewieMesh;
                        renderer.material.SetTexture("_MainTex", stewieTex);
                    }
                    else
                    {
                        renderer.forceRenderingOff = true;
                    }
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void Awake()
    {
        StartCoroutine(UpdateBingBong());
    }
}
