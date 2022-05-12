using UdonSharp;
using UnityEngine;

namespace Supernova
{
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
         * Desired pixel colors are saved here after mostly-every render frame for other scripts to pick up and use 
         */
        public Color[] textureData;

        /**
         * Flag for if we are currently grabbing pixels from an OnPostRender event and should skip further processing
         */
        private bool _processing;

        void Start()
        {
            // Make sure that we aren't given bad input on the pixel sampling
            if (texture2D == null
                || texture2D.width < xPixelColumnSamplePosition
                || texture2D.height < yPixelColumnSamplePosition + pixelColumnSampleHeight
            )
            {
                Debug.LogWarning("Pixel sample impossible with given inputs.");
                texture2D = null;
            }
        }

        /**
         * Event that fires every time the target render texture is written to
         */
        void OnPostRender()
        {
            
            // Bail out early if we are working on a past event or there is no texture to process
            if (_processing || texture2D == null)
            {
                return;
            }
            
            // Set flag to show that we are working an event in case another occurs before we are done
            _processing = true;

            // Read a pixel column from the render texture into our image texture
            
            Rect sampleSpace = new Rect(
                xPixelColumnSamplePosition,
                yPixelColumnSamplePosition,
                1,
                pixelColumnSampleHeight
            );
            
            texture2D.ReadPixels(sampleSpace, 0, 0, false);
            textureData = texture2D.GetPixels(0, 0, 1, pixelColumnSampleHeight);

            // Set flag to show that we are done working this event and can process newer events
            _processing = false;
        }
    }
}
