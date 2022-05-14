using UdonSharp;
using UnityEngine;

namespace Supernova
{
    
    /**
     * Reads a ProTV AVPro material texture and sends it to a render texture for processing by TextureSampler scripts
     */
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class TextureRenderOutputProTVAvPro : UdonSharpBehaviour
    {

        /**
         * In most of the prefabs this can be found at: Prefab > Screens > Main
         */
        [SerializeField, Tooltip("Mesh renderer holding the AVPro screen component ProTV is displaying to")]
        private Renderer avProScreenRenderer;

        /**
         * This will be the same render texture that the camera and other scripts in the demo scenes use
         */
        [SerializeField, Tooltip("Render texture matching the screen resolution read by a TextureSampler script")]
        private CustomRenderTexture outputTexture;

        /**
         * Materials from script inputs set in Start event
         */
        private Material _avProFetchMaterial;
        private Material _outputMat;

        /**
         * Fetch the materials we will need just once at component startup instead of in a tight Update loop
         */
        void Start()
        {

            // Bail out early if we have bad input
            if (avProScreenRenderer == null || outputTexture == null)
            {
                return;
            }

            _avProFetchMaterial = avProScreenRenderer.material;
            _outputMat = outputTexture.material;
        }

        /**
         * Grab the screen texture when we can
         */
        void LateUpdate()
        {
            
            // Bail out early for bad input
            if (_avProFetchMaterial == null || _outputMat == null)
            {
                return;
            }

            // Grab a texture that we should pass on
            var videoPlayerTex = _avProFetchMaterial.GetTexture("_MainTex");
            if (videoPlayerTex == null)
            {
                return;
            }
            
            // Set the texture to USharpVideo material inputs
            // For whatever reason, setting "_MainTex" on a ProTV material does do what we need
            // I will likely not be investigating that any further as this is a one-off event script
            _outputMat.SetTexture("_SourceTexture", videoPlayerTex);
            _outputMat.SetInt("_IsAVPro", 1);
        }
    }
}
