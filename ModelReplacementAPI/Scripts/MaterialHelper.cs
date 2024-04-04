using System.Linq;
using UnityEngine;

namespace UnityModelReplacement.Scripts
{
    public class MaterialHelper
    {
        // Shaders with any of these prefixes won't be automatically converted.
        private static readonly string[] shaderPrefixWhitelist =
        {
            "HDRP/",
            "GUI/",
            "Sprites/",
            "UI/",
            "Unlit/",
            "Toon",
            "lilToon",
            "Shader Graphs/",
            "Hidden/"
        };

        public virtual Material GetReplacementMaterial(Material gameMaterial, Material modelMaterial) // Get a replacement material based on the original game material, and the material found on the replacing model.
        {

            if (shaderPrefixWhitelist.Any(prefix => modelMaterial.shader.name.StartsWith(prefix)))
            {
                return modelMaterial;
            }
            else
            {
                UnityModelReplacement.Instance.Logger.LogInfo($"Creating replacement material for material {modelMaterial.name} / shader {modelMaterial.shader.name}");

                Material replacementMat = new Material(gameMaterial);
                replacementMat.color = modelMaterial.color;
                replacementMat.mainTexture = modelMaterial.mainTexture;
                replacementMat.mainTextureOffset = modelMaterial.mainTextureOffset;
                replacementMat.mainTextureScale = modelMaterial.mainTextureScale;
                replacementMat.EnableKeyword("_EMISSION");
                replacementMat.EnableKeyword("_NORMALMAP");
                replacementMat.EnableKeyword("_SPECGLOSSMAP");
                replacementMat.SetFloat("_NormalScale", 0);

                // HDMaterial.ValidateMaterial(replacementMat);

                return replacementMat;
            }
        }
    }
}
