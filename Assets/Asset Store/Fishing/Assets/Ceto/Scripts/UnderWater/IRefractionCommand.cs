
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;


namespace Ceto
{
    /// <summary>
    /// Interface to provide your own command buffer for the
    /// refraction grab pass. This way you can apply custom
    /// effects (like a blur for example) to the grab texture 
    /// before it gets sampled.
    /// </summary>
    public interface IRefractionCommand
    {


        /// <summary>
        /// Resolution relative to screen of refraction grab.
        /// </summary>
        REFRACTION_RESOLUTION Resolution { get; set; }

        /// <summary>
        /// The camera event when the command should be executed.
        /// </summary>
        CameraEvent Event { get; set; }

        /// <summary>
        /// Return a new command buffer.
        /// This will be called the first time
        /// the mesh is rendered for each camera 
        /// that renders the ocean.
        /// </summary>
		CommandBuffer Create(Camera cam);

        /// <summary>
        /// Remove the command buffer from the camera.
        /// </summary>
        void Remove(Camera cam);

        /// <summary>
        /// Remove the command buffer from all the cameras.
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Does the cmd created for this camera match the current camera settings.
        /// </summary>
        bool Matches(Camera cam);


    }

}
