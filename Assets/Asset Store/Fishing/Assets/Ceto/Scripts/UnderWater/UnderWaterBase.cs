using UnityEngine;
using System.Collections;


namespace Ceto
{

    /// <summary>
    /// Abstract base class for underwater effects.
    /// </summary>
	[DisallowMultipleComponent]
	public abstract class UnderWaterBase : OceanComponent
	{

        /// <summary>
        /// The under water mode.
        /// ABOVE - only renders the underwater effects on the top side mesh 
        /// ABOVE_AND_BELOW - render the underwater effects on the top mesh, the under
        /// side mesh and as a post effect if post effect script attached to camera.
        /// </summary>
		public abstract UNDERWATER_MODE Mode { get; }

        /// <summary>
        /// If 'USE_OCEAN_DEPTH_PASS' this will render a separate depth pass and use that information
        /// to apply the underwater effect. This means you will get more draw calls
        /// but allows the depth info to be accessible when it otherwise would not be. 
        /// 
        /// If 'USE_DEPTH_BUFFER' the depth data for the underwater effect will come 
        /// from a copy of the depth buffer. This is faster to do but only works if
        /// the ocean is in the transparent queue as the depths need to be copied from
        /// the _CameraDepthTexture using a command buffer at the AfterSkyBox event.
        /// The reason a copy is needed is because if sampling from the depth buffer and 
        /// writing to it in certain set ups (dx9/Deferred) this does not work correctly on
        /// certain graphics cards (Nivida).
        /// </summary>
        public abstract DEPTH_MODE DepthMode { get; }

        /// <summary>
        /// Renders the mask used in the post effect shader.
        /// Only gets created if mode is ABOVE_AND_BELOW.
        /// </summary>
		public abstract void RenderOceanMask(GameObject go);

        /// <summary>
        /// Renders the depth information used to apply
        /// the underwater effect.
        /// </summary>
		public abstract void RenderOceanDepth(GameObject go);

        /// <summary>
        /// Assign to provide you own command buffer to do the refraction grab pass.
        /// </summary>
        public IRefractionCommand CustomRefractionCommand { get; set; }

    }

}
