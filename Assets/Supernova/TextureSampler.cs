using UdonSharp;
using UnityEngine;

namespace Supernova
{
    /**
     * Samples a portion of a video render texture to a texture image every so often and hands to TextureReader scripts
     */
    public class TextureSampler : UdonSharpBehaviour
    {

        /**
         * Pixel information from the render texture is input to this texture2D for processing
         */
        [SerializeField, Tooltip("Simple white PNG that must have the same height as the video player / render texture")]
        private Texture2D texture2D;

        /**
         * The "X" in "X by Y" of screen dimensions where the pixel sampling will begin 
         */
        [SerializeField, Tooltip("X (width) position value of pixel column that will be sampled")]
        private int xPixelColumnSamplePosition;
        
        /**
         * The "Y" in "X by Y" of screen dimensions where the pixel sampling will begin 
         */
        [SerializeField, Tooltip("Y (height) position value of pixel column that will be sampled")]
        private int yPixelColumnSamplePosition;
        
        /**
         * The height of the column of pixels to sample on the screen 
         */
        [SerializeField, Tooltip("Column height of pixels to sample from given position")]
        private int pixelColumnSampleHeight;

        /**
         * Event processors listed here instead of running an Update() loop on every texture reader
         */
        [SerializeField, Tooltip("Texture readers that will run their own processing whenever a new sample is taken")]
        private TextureReader[] textureReaders;

        /**
         * Flags for if we are currently grabbing pixels from an OnPostRender event and should skip further processing
         */
        private int _lastFrameCalculation;
        private bool _processing;

        /**
         * Event that fires on component creation that determines whether or not we ever handle render loop
         */
        void Start()
        {
            // Make sure that we aren't given bad input on the pixel sampling
            if (texture2D == null
                || textureReaders == null
                || textureReaders.Length == 0
                || texture2D.height < yPixelColumnSamplePosition + pixelColumnSampleHeight
            )
            {
                Debug.LogWarning("TextureSampler cannot work with bad user input, shutting down");
                texture2D = null;
            }
        }

        /**
         * Event that fires every time the target render texture is written to
         */
        void OnPostRender()
        {
            
            // Bail out early if we are working on a past event, there is no texture to process, or we should wait a bit
            if (_processing || texture2D == null || Time.frameCount - _lastFrameCalculation < 10)
            {
                return;
            }

            // Set flag to show that we are working an event in case another occurs before we are done
            _processing = true;
            _lastFrameCalculation = Time.frameCount;

            // Read a pixel column from the render texture into our image texture
            
            var sampleSpace = new Rect(
                xPixelColumnSamplePosition,
                yPixelColumnSamplePosition,
                1,
                pixelColumnSampleHeight
            );
            
            texture2D.ReadPixels(sampleSpace, 0, 0, false);
            var textureData = texture2D.GetPixels(0, 0, 1, pixelColumnSampleHeight);

            // Alert all of our texture readers that there is a new pixel intensity to process

            foreach (var textureReader in textureReaders)
            {

                // Skip any bad inputs in the script list
                if (textureReader == null)
                {
                    continue;
                }

                // Skip any input requesting a pixel that we don't have
                var samplePixelIndex = textureReader.GetSamplePixelIndex();
                if (samplePixelIndex < 0 || samplePixelIndex >= textureData.Length)
                {
                    continue;
                }

                // Send the TextureReader an intensity value for it to process
                var intensity = textureData[samplePixelIndex].grayscale;
                textureReader.ToggleChildren(intensity);
            }

            // Set flag to show that we are done working this event and can process newer events
            _processing = false;
        }

    }
}
