using System;
using UdonSharp;
using UnityEngine;

namespace Supernova
{
    /**
     * Toggles children components based on given pixel intensities from a TextureSampler script 
     */
    public class TextureReader : UdonSharpBehaviour
    {

        /**
         * Array index for pixels that are given to us from a TextureSampler script
         */
        [SerializeField, Tooltip("Which pixel to read from the video render image script output for processing")]
        private int samplePixelIndex = 5;

        /**
         * Variance bracket range for intensity activating an object
         */
        [SerializeField, Tooltip("How much pixel intensity range will allocate to activating each child, from 1 to 100")]
        private int intensityRangeForEachChild;

        /**
         * Flag for if we are currently processing a pixel update from the event loop and should skip further processing
         */
        private bool _processing;

        /**
         * Called by TextureSampler scripts to determine if it should call ToggleChildren() 
         */
        public int GetSamplePixelIndex()
        {
            return samplePixelIndex;
        }

        /**
         * Called from TextureSampler script on every render frame
         */
        public void ToggleChildren(float intensity)
        {
            // Bail out early if we are working on a past event
            if (_processing)
            {
                return;
            }

            // Set flag to show that we are working an event in case another occurs before we are done
            _processing = true;
            
            // Determine which child object (if any) should be activated and others deactivated based on intensity

            var activatedChild = -1;

            if (intensityRangeForEachChild > 0 && intensityRangeForEachChild <= 100)
            {
                intensity = Math.Max(0, intensity);
                intensity = Math.Min(1, intensity);
                var percentage = (int) (intensity * 100);
                activatedChild = percentage / intensityRangeForEachChild;
            }

            // Sample the given pixel intensity and deactivate all children not within given intensity range

            var children = GetComponentsInChildren<Transform>(true);
            for (var childIndex = 0; childIndex < children.Length; childIndex++)
            {

                // Skip this if we're running on the parent (Unity's "getChildren" is a liar) or no change is necessary 
                
                var child = children[childIndex].gameObject;
                var desiredChildState = childIndex == activatedChild;
                
                if (child.activeSelf == desiredChildState || child.GetInstanceID() == gameObject.GetInstanceID())
                {
                    continue;
                }
                
                // Toggle activation state of objects
                child.gameObject.SetActive(desiredChildState);
            }

            // Set flag to show that we are done working this event and can process newer events
            _processing = false;
        }
    }
}