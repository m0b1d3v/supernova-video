using UdonSharp;
using UnityEngine;
using VRC.Udon;

namespace Supernova
{
    public class TextureReader : UdonSharpBehaviour
    {

        /**
         * TextureSampler script runner that is always processing render texture changes
         */
        [SerializeField, Tooltip("Video render image script output")]
        private UdonBehaviour textureSampler;
        
        /**
         * Array index for pixels we can grab from the textureSampler above
         */
        [SerializeField, Tooltip("Which pixel to read from the video render image script output for processing")]
        private int samplePixelIndex;

        [SerializeField, Tooltip("Child objects underneath this given parent will be toggled based on parent intensity")]
        private GameObject parentOfChildrenToToggle;
        
        /**
         * Flag for if we are currently processing a pixel update from the event loop and should skip further processing
         */
        private bool _processing;

        void Update()
        {

            // Bail out early if we are working on a past event or there is no texture to process
            if (_processing || textureSampler == null || parentOfChildrenToToggle == null)
            {
                return;
            }
            
            // Set flag to show that we are working an event in case another occurs before we are done
            _processing = true;
            
            // Sample the given pixel intensity and deactivate all children not within 5% of the intensity

            Color[] textureData = (Color[]) textureSampler.GetProgramVariable("textureData");
            if(textureData.Length > samplePixelIndex)
            {
                Debug.Log(textureData[samplePixelIndex].grayscale);
            }

            // Set flag to show that we are done working this event and can process newer events
            _processing = false;
        }
    }
}
