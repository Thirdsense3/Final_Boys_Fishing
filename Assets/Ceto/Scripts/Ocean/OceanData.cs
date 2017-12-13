using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;

namespace Ceto
{
    /// <summary>
    /// This is the common settings like enum and data classes used through out Ceto.
    /// </summary>
	
    /// <summary>
    /// The resolution settings for the ocean mesh.
    /// </summary>
	public enum MESH_RESOLUTION {  LOW, MEDIUM, HIGH, ULTRA, EXTREME  };

	/// <summary>
	/// The number of meshes the projected grid is split into.
	/// A higher group means more meshes and draw calls but
	/// since they are smaller they can the culled more easily.
	/// What ever the grouping the actual number of verts is the same
	/// for a given mesh resolution.
	/// </summary>
	public enum GRID_GROUPS { SINGLE = 0, LOW = 1, MEDIUM = 2, HIGH = 3, EXTREME = 4 };
	
    /// <summary>
    /// The resolution of the reflection render target texture.
    /// </summary>
	public enum REFLECTION_RESOLUTION { FULL, HALF, QUARTER };

    /// <summary>
    /// The resolution of the refraction render target texture.
    /// </summary>
    public enum REFRACTION_RESOLUTION { FULL, HALF, QUARTER };

	/// <summary>
	/// The size of the Fourier transform and if the displacement data runs on the GPU or CPU.
	/// </summary>
	public enum FOURIER_SIZE { LOW_32_CPU, LOW_32_GPU, MEDIUM_64_CPU, MEDIUM_64_GPU, HIGH_128_CPU, HIGH_128_GPU, ULTRA_256_GPU , EXTREME_512_GPU };

    /// <summary>
    /// Type of spectrum used to generate the waves.
    /// </summary>
    public enum SPECTRUM_TYPE {  UNIFIED, PHILLIPS, UNIFIED_PHILLIPS, CUSTOM };

    /// <summary>
    /// 
    /// </summary>
    public enum SPECTRUM_DISTRIBUTION {  LINEAR, GAUSSIAN };

    /// <summary>
    /// If the underwater effect is only applied when seen from above or when seen from above and below the waves.
    /// </summary>
    public enum UNDERWATER_MODE { ABOVE_ONLY, ABOVE_AND_BELOW };

    /// <summary>
    /// Shader pass for the overlay rendering.
    /// </summary>
	public enum OVERLAY_PASS { HEIGHT = 0, NORMAL = 1, FOAM = 2, CLIP = 3 };

	/// <summary>
	/// What the overlay mask is applied to.
	/// </summary>
	public enum OVERLAY_MASK_MODE { WAVES = 0, OVERLAY = 1, WAVES_AND_OVERLAY = 2, WAVES_AND_OVERLAY_BLEND = 3 };

    /// <summary>
    /// The size of the overlay map in relation to the screen size.
    /// </summary>
	public enum OVERLAY_MAP_SIZE { DOUBLE, FULL_HALF, FULL, HALF, QUARTER };

    /// <summary>
    /// The wave query mode.
    /// </summary>
    public enum QUERY_MODE { POSITION, DISPLACEMENT, CLIP_TEST };

    /// <summary>
    /// The method for acquiring the depth data for the underwater effect.
    /// </summary>
    public enum DEPTH_MODE {  USE_OCEAN_DEPTH_PASS, USE_DEPTH_BUFFER };

    /// <summary>
    /// Used to apply the foam textures to the ocean script from editor.
    /// </summary>
    [Serializable]
    public class FoamTexture
    {
        public Texture tex;
        public Vector2 scale = Vector2.one;
		public float scrollSpeed = 1.0f;
    }

	/// <summary>
	/// Settings to scale the grid when query sampling.
	/// </summary>
	public class QueryGridScaling
	{
		public Vector4 invGridSizes;
		public float scaleY;
		public Vector4 choppyness;
        public Vector3 offset;
        public int numGrids;
        public float[] tmp;
	}

	/// <summary>
	/// If a overlay contains a given point
	/// this is the result that contains the
	/// overlay and the points uv position.
	/// </summary>
	public struct QueryableOverlayResult
	{
		public WaveOverlay overlay;
		public float u;
		public float v;
	}

    /// <summary>
    /// Holds the reflection cam and if the 
    /// reflections have been updated this frame.
    /// </summary>
    public class ReflectionData
    {
        public bool updated;
        public Camera cam;
		public RenderTexture tex;
    }

    /// <summary>
    /// Holds the mask cam and if the 
    /// mask has been updated this frame.
    /// </summary>
    public class MaskData
    {
        public bool updated;
        public Camera cam;
    }

    /// <summary>
    /// Holds the depth cam and if the 
    /// depths have been updated this frame.
    /// </summary>
	public class DepthData
    {
        public bool updated;
        public Camera cam;
		public CommandBuffer grabCmd;
        public CameraEvent cmdEvent;
    }

    /// <summary>
    /// Holds the overlay maps and if
    /// overlays have been updated this frame.
    /// </summary>
	public class WaveOverlayData
	{
		public bool updated;
		public RenderTexture normal;
		public RenderTexture height;
		public RenderTexture foam;
		public RenderTexture clip;
	}

	/// <summary>
	/// Projection data.
	/// </summary>
	public class ProjectionData
	{
		public bool updated;
		public Matrix4x4 projectorVP;
		public Matrix4x4 interpolation;
	}

    /// <summary>
    /// Holds all the data for a camera. 
    /// Each camera rendering the ocean has its own copy.
    /// </summary>
	public class CameraData
    {
        public OceanCameraSettings settings;
        public MaskData mask;
        public DepthData depth;
		public WaveOverlayData overlay;
		public ProjectionData projection;
        public ReflectionData reflection;
    }
	

}







