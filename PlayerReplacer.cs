using UnityEngine;

namespace UnityModelReplacement
{
    public class PlayerReplacer : MonoBehaviour
    {
        public GameObject replacementPrefab = null;

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

            Transform playerHeadLower = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/mesh_head_bot_sphere");
            Transform replacementHeadLower = replacementPrefab.transform.Find("mesh_head_bot_sphere");
            playerHeadLower.GetComponent<MeshFilter>().mesh = replacementHeadLower.GetComponent<MeshFilter>().mesh;
            playerHeadLower.GetComponent<MeshRenderer>().materials = replacementHeadLower.GetComponent<MeshRenderer>().materials;
            foreach (Material material in playerHeadLower.GetComponent<MeshRenderer>().materials)
            {
                material.shader = Shader.Find("Hurtable/Hurtable");
            }

            Transform playerHeadUpper = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/_____________________________________/ANIM HEAD TOP/code_head_top/mesh_head_top");
            if (!assetPath.Contains("Kenny"))
            {
                Transform replacementHeadUpper = replacementPrefab.transform.Find("mesh_head_top");
                playerHeadUpper.GetComponent<MeshFilter>().mesh = replacementHeadUpper.GetComponent<MeshFilter>().mesh;
                playerHeadUpper.GetComponent<MeshRenderer>().materials = replacementHeadUpper.GetComponent<MeshRenderer>().materials;
                foreach (Material material in playerHeadUpper.GetComponent<MeshRenderer>().materials)
                {
                    material.shader = Shader.Find("Hurtable/Hurtable");
                }
            }
            else
            {
                playerHeadUpper.GetComponent<MeshRenderer>().forceRenderingOff = true;
            }
            
            // BEGIN WALL OF LAZY BULLSHIT

            MeshRenderer playerCrown = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/_____________________________________/ANIM HEAD TOP/code_head_top/ArenaCrown").GetComponent<MeshRenderer>();
            playerCrown.forceRenderingOff = true;

            MeshRenderer playerEyeLeft = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/_____________________________________/ANIM HEAD TOP/code_head_top").GetChild(4).Find("ANIM EYE LEFT/code_eye_left/mesh_eye_l").GetComponent<MeshRenderer>();
            playerEyeLeft.forceRenderingOff = true;

            MeshRenderer playerPupilLeft = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/_____________________________________/ANIM HEAD TOP/code_head_top").GetChild(4).Find("ANIM EYE LEFT/code_eye_left/ANIM PUPIL LEFT/mesh_pupil_l").GetComponent<MeshRenderer>();
            playerPupilLeft.forceRenderingOff = true;

            MeshRenderer playerEyeRight = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/_____________________________________/ANIM HEAD TOP/code_head_top").GetChild(5).Find("ANIM EYE RIGHT/code_eye_right/mesh_eye_r").GetComponent<MeshRenderer>();
            playerEyeRight.forceRenderingOff = true;

            MeshRenderer playerPupilRight = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/_____________________________________/ANIM HEAD TOP/code_head_top").GetChild(5).Find("ANIM EYE RIGHT/code_eye_right/ANIM PUPIL RIGHT/mesh_pupil_r").GetComponent<MeshRenderer>();
            playerPupilRight.forceRenderingOff = true;

            MeshRenderer playerHealth = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/Health Hide/mesh_health").GetComponent<MeshRenderer>();
            playerHealth.forceRenderingOff = true;

            MeshRenderer playerHealthFrame = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/Health Hide/mesh_health/mesh_health frame").GetComponent<MeshRenderer>();
            playerHealthFrame.forceRenderingOff = true;

            MeshRenderer playerHealthShadow = this.transform.parent.Find("Player Visuals/[RIG]/code_lean/code_tilt/ANIM BOT/_____________________________________/ANIM BODY BOT/_____________________________________/ANIM BODY TOP/code_body_top_up/code_body_top_side").GetChild(3).Find("ANIM HEAD BOT/code_head_bot_up/code_head_bot_side/Health Hide/mesh_health/mesh_health shadow").GetComponent<MeshRenderer>();
            playerHealthShadow.forceRenderingOff = true;
        }

        public void LoadModelByPlayerName()
        {
            switch (playerName)
            {
                case "Beaker":
                case "Irastris":
                    LoadModel("Assets/_Modding/Beaker.prefab");
                    break;
                case "Gonzo":
                    LoadModel("Assets/_Modding/Gonzo.prefab");
                    break;
                case "Kermit":
                case "kboykboy":
                    LoadModel("Assets/_Modding/Kermit.prefab");
                    break;
                case "Miss Piggy":
                case "Piggy":
                    LoadModel("Assets/_Modding/MissPiggy.prefab");
                    break;
                case "Swedish Chef":
                case "Chef":
                    LoadModel("Assets/_Modding/SwedishChef.prefab");
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
        }

        public void OnDestroy()
        {
            CancelInvoke();
            Destroy(replacementPrefab);
        }
    }
}