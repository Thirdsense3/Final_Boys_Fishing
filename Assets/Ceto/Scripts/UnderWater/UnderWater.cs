using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;

using Ceto.Common.Unity.Utility;

#pragma warning disable 649

namespace Ceto
{

    /// <summary>
    /// Handles the under water settings
    /// </summary>
	[AddComponentMenu("Ceto/Components/UnderWater")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Ocean))]
	public class UnderWater : UnderWaterBase
	{

        public const float MAX_REFRACTION_INTENSITY = 2.0f;
        public const float MAX_REFRACTION_DISORTION = 4.0f;
        public const float MAX_ABSORPTION_INTENSITY = 10.0f;
        public const float MAX_INSCATTER_INTENSITY = 2.0f;

        /// <summary>
        /// These are the events when the command buffer to to grab
        /// pass will run if using USE_DEPTH_BUFFER mode.
        /// The command will grab the depth buffer and screen color.
        /// </summary>
        public const CameraEvent FORWARD_EVENT = CameraEvent.AfterDepthTexture;
        public const CameraEvent DEFERRED_EVENT = CameraEvent.AfterLighting;
        public const CameraEvent LEGACY_DEFERRED_EVENT = CameraEvent.AfterLighting;

        /// <summary>
        /// Settings to modify the absorption.
        /// The absorption is how the light color
        /// changes as in moves through the water.
        /// </summary>
		[Serializable]
		public struct AbsorptionModifier
		{
            //Scales the distance used the absorption is applied to.
			[Range(0.0f, 50.0f)]
			public float scale;

            //Adjust the intensity of the final color.
			[Range(0.0f, MAX_ABSORPTION_INTENSITY)]
			public float intensity;

            //Tint the final color.
			public Color tint;

			public AbsorptionModifier(float scale, float intensity, Color tint)
			{
				this.scale = scale;
				this.intensity = intensity;
				this.tint = tint;
			}
		}

        /// <summary>
        /// Affects how the inscatter is applied.
        /// </summary>
		public enum INSCATTER_MODE { LINEAR = 0, EXP = 1, EXP2 = 2 };

        /// <summary>
        /// Settings to modify the inscatter.
        /// The inscatter represents the color of light that is 
        /// bounced back into the view direction of the camera.
        /// Is just applied using a basic fog style method.
        /// </summary>
		[Serializable]
		public struct InscatterModifier
		{
            //Scales the distance used the inscatter is applied to.
			[Range(0.0f, 5000.0f)]
			public float scale;

            //Adjust the intensity of the final color.
			[Range(0.0f, MAX_INSCATTER_INTENSITY)]
			public float intensity;

            //Method used to apply the inscatter.
			public INSCATTER_MODE mode;

            //The inscatter color.
			public Color color;
			
			public InscatterModifier(float scale, float intensity, Color color, INSCATTER_MODE mode)
			{
				this.scale = scale;
				this.intensity = intensity;
				this.color = color;
				this.mode = mode;
			}
		}

        /// <summary>
        /// The depth the bottom mesh goes to.
        /// It is recommended to have this be
        /// no larger than half the far plane value.
        /// </summary>
		readonly float OCEAN_BOTTOM_DEPTH = 1000.0f;

        /// <summary>
        /// Used as the value the ocean depths are normalized to.
        /// Dont change this.
        /// </summary>
        readonly float MAX_DEPTH_DIST = 500.0f;

        /// <summary>
        /// The under water mode.
        /// ABOVE - only renders the underwater effects on the top side mesh 
        /// ABOVE_AND_BELOW - render the underwater effects on the top mesh, the under
        /// side mesh and as a post effect if post effect script attached to camera.
        /// </summary>
		public UNDERWATER_MODE underwaterMode = UNDERWATER_MODE.ABOVE_ONLY;
		public override UNDERWATER_MODE Mode { get { return underwaterMode; } }

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
        public DEPTH_MODE depthMode = DEPTH_MODE.USE_OCEAN_DEPTH_PASS;
        public override DEPTH_MODE DepthMode { get { return depthMode; } }

        /// <summary>
        /// The layers that will be rendered in the ocean depth buffer.
        /// This is only used if the depth mode is USE_OCEAN_DEPTH_PASS.
        /// </summary>
        public LayerMask oceanDepthsMask = 1;

        /// <summary>
        /// The resolution relative to the screen of the refraction texture.
        /// This is only used if the depth mode is USE_DEPTH_BUFFER.
        /// </summary>
        /*public*/ REFRACTION_RESOLUTION refractionResolution = REFRACTION_RESOLUTION.FULL;

		/// <summary>
		/// The underwater effect can be applied using the objects world y value or its
        /// camera z value. This will just lerp between the to methods. 0 is full 'by world y'
        /// and 1 is full 'by camera z'. 
		/// </summary>
		[Range(0.0f, 1.0f)]
		public float depthBlend = 0.2f;

        /// <summary>
        /// Amount of fade applied where the ocean meets other objects in scene.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float edgeFade = 0.2f;

        /// <summary>
        /// The absorption cof for red light.
        /// a higher value means more red light is lost 
        /// as the light travels through the water.
        /// </summary>
        [Range(0.0f, 1.0f)]
		public float absorptionR = 0.45f;

        /// <summary>
        /// The absorption cof for green light.
        /// a higher value means more green light is lost 
        /// as the light travels through the water.
        /// </summary>
		[Range(0.0f, 1.0f)]
		public float absorptionG = 0.029f;

        /// <summary>
        /// The absorption cof for blue light.
        /// a higher value means more blue light is lost 
        /// as the light travels through the water.
        /// </summary>
		[Range(0.0f, 1.0f)]
		public float absorptionB = 0.018f;

        /// <summary>
        /// Modify the result of applying the absorption cof when above the mesh looking down.
        /// </summary>
		public AbsorptionModifier aboveAbsorptionModifier 
			= new AbsorptionModifier(2.0f, 1.0f, Color.white);

        /// <summary>
        /// Modify the result of applying the absorption cof when below the mesh looking down.
        /// </summary>
        public AbsorptionModifier belowAbsorptionModifier
            = new AbsorptionModifier(0.1f, 1.0f, Color.white);

        /// <summary>
        /// Modify the result of applying the absorption cof to the subsurface scatter.
        /// </summary>
		public AbsorptionModifier subSurfaceScatterModifier 
			= new AbsorptionModifier(10.0f, 1.5f, new Color32(220, 250, 180, 255));

        /// <summary>
        /// Modify the inscatter when above the mesh looking down.
        /// </summary>
		public InscatterModifier aboveInscatterModifier 
			= new InscatterModifier(300.0f, 1.0f, new Color32(0, 19, 30, 255), INSCATTER_MODE.EXP2);

        /// <summary>
        /// Modify the inscatter when below the mesh looking up.
        /// </summary>
		public InscatterModifier belowInscatterModifier 
			= new InscatterModifier(60.0f, 1.0f, new Color32(1, 37, 58, 255), INSCATTER_MODE.EXP);

        /// <summary>
        /// Modify the intensity of the refraction grab color.
        /// </summary>
		[Range(0.0f, MAX_REFRACTION_INTENSITY)]
		public float refractionIntensity = 0.5f;

        /// <summary>
        /// The strength of the distortion applied to the refraction uvs.
        /// </summary>
		[Range(0.0f, MAX_REFRACTION_DISORTION)]
		public float refractionDistortion = 0.5f;

        /// <summary>
        /// If useCmdBufferForGrab is true this is the default
        /// command used for the refraction grab.
        /// </summary>
        IRefractionCommand m_refractionCommand;

        /// <summary>
        /// The bottom mesh that surrounds the player under the water.
		/// Used to render the correct info for the background.
        /// </summary>
		GameObject m_bottomMask;

        /// <summary>
        /// 
        /// </summary>
		[HideInInspector]
        public Shader oceanBottomSdr, oceanDepthSdr, copyDepthSdr;

		[HideInInspector]
		public Shader oceanMaskSdr;

		void Start () 
		{

			try
			{

                m_refractionCommand = new RefractionCommand(copyDepthSdr);

                Mesh mesh = CreateBottomMesh(32, 512);

				//The bottom used to render the masks.
				m_bottomMask = new GameObject("Ceto Bottom Mask Gameobject");

				MeshFilter filter = m_bottomMask.AddComponent<MeshFilter>();
				MeshRenderer renderer = m_bottomMask.AddComponent<MeshRenderer>();
				NotifyOnWillRender willRender = m_bottomMask.AddComponent<NotifyOnWillRender>();

				filter.sharedMesh = mesh;
				renderer.receiveShadows = false;
				renderer.shadowCastingMode = ShadowCastingMode.Off;
				renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
				renderer.material = new Material(oceanBottomSdr);

                willRender.AddAction(m_ocean.RenderWaveOverlays);
                willRender.AddAction(m_ocean.RenderOceanMask);
				willRender.AddAction(m_ocean.RenderOceanDepth);

				m_bottomMask.layer = LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
				m_bottomMask.hideFlags = HideFlags.HideAndDontSave;
            
				UpdateBottomBounds();

                Destroy(mesh);

			}
			catch(Exception e)
			{
				Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}

		
		}

		protected override void OnEnable()
		{

			base.OnEnable();

            try
            {

                Shader.EnableKeyword("CETO_UNDERWATER_ON");

                SetBottomActive(m_bottomMask, true);
            }
            catch (Exception e)
            {
                Ocean.LogError(e.ToString());
                WasError = true;
                enabled = false;
            }
        }
		
		protected override void OnDisable()
		{

			base.OnDisable();

            try
            {
                Shader.DisableKeyword("CETO_UNDERWATER_ON");

                SetBottomActive(m_bottomMask, false);
            }
            catch (Exception e)
            {
                Ocean.LogError(e.ToString());
                WasError = true;
                enabled = false;
            }

        }

        protected override void OnDestroy()
        {

            base.OnDestroy();

            try
            {

                if (m_bottomMask != null)
                {
                    Mesh mesh = m_bottomMask.GetComponent<MeshFilter>().mesh;
                    Destroy(m_bottomMask);
                    Destroy(mesh);
                }

            }
			catch(Exception e)
			{
                Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}


        }

        void Update () 
		{

			try
			{

                //Shader.SetGlobalTexture(Ocean.REFRACTION_GRAB_TEXTURE_NAME, Texture2D.blackTexture);
                //Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
                //Shader.SetGlobalTexture("Ceto_OceanDepth", Texture2D.whiteTexture);

                Vector4 absCof = new Vector4(absorptionR, absorptionG, absorptionB, 1.0f);
				Vector4 sssCof = absCof;
				Vector4 belowCof = absCof;

                absCof.w = Mathf.Max(0.0f, aboveAbsorptionModifier.scale);
				sssCof.w = Mathf.Max(0.0f, subSurfaceScatterModifier.scale);
				belowCof.w = Mathf.Max(0.0f, belowAbsorptionModifier.scale);

				Color absTint = aboveAbsorptionModifier.tint * Mathf.Max(0.0f, aboveAbsorptionModifier.intensity);
				Color sssTint = subSurfaceScatterModifier.tint * Mathf.Max(0.0f, subSurfaceScatterModifier.intensity); 
				Color belowTint = belowAbsorptionModifier.tint * Mathf.Max(0.0f, belowAbsorptionModifier.intensity); 
			
				Shader.SetGlobalVector("Ceto_AbsCof", absCof);
				Shader.SetGlobalVector("Ceto_AbsTint", absTint);

				Shader.SetGlobalVector("Ceto_SSSCof", sssCof);
				Shader.SetGlobalVector("Ceto_SSSTint", sssTint);

				Shader.SetGlobalVector("Ceto_BelowCof", belowCof);
				Shader.SetGlobalVector("Ceto_BelowTint", belowTint);

				Color aboveInscatterCol = aboveInscatterModifier.color;
				aboveInscatterCol.a = Mathf.Clamp01(aboveInscatterModifier.intensity);

				Shader.SetGlobalFloat("Ceto_AboveInscatterScale", Mathf.Max(0.1f, aboveInscatterModifier.scale));
				Shader.SetGlobalVector("Ceto_AboveInscatterMode", InscatterModeToMask(aboveInscatterModifier.mode));
				Shader.SetGlobalVector("Ceto_AboveInscatterColor", aboveInscatterCol);

				Color belowInscatterCol = belowInscatterModifier.color;
				belowInscatterCol.a = Mathf.Clamp01(belowInscatterModifier.intensity);
				
				Shader.SetGlobalFloat("Ceto_BelowInscatterScale", Mathf.Max(0.1f, belowInscatterModifier.scale));
				Shader.SetGlobalVector("Ceto_BelowInscatterMode", InscatterModeToMask(belowInscatterModifier.mode));
				Shader.SetGlobalVector("Ceto_BelowInscatterColor", belowInscatterCol);

				Shader.SetGlobalFloat("Ceto_RefractionIntensity", Mathf.Max(0.0f, refractionIntensity));
				Shader.SetGlobalFloat("Ceto_RefractionDistortion", refractionDistortion * 0.05f);
				Shader.SetGlobalFloat("Ceto_MaxDepthDist", Mathf.Max(0.0f, MAX_DEPTH_DIST));
				Shader.SetGlobalFloat("Ceto_DepthBlend", Mathf.Clamp01(depthBlend));
                Shader.SetGlobalFloat("Ceto_EdgeFade", Mathf.Lerp(20.0f, 2.0f, Mathf.Clamp01(edgeFade)));
		
				if(depthMode == DEPTH_MODE.USE_OCEAN_DEPTH_PASS)
				{
					Shader.EnableKeyword("CETO_USE_OCEAN_DEPTHS_BUFFER");

					if(underwaterMode == UNDERWATER_MODE.ABOVE_ONLY)
					{
						SetBottomActive(m_bottomMask, false);
					}
					else			
					{
						SetBottomActive(m_bottomMask, true);
						UpdateBottomBounds();
					}
				}
				else
				{
					Shader.DisableKeyword("CETO_USE_OCEAN_DEPTHS_BUFFER");

					if(underwaterMode == UNDERWATER_MODE.ABOVE_ONLY)
					{
						SetBottomActive(m_bottomMask, false);
					}
					else			
					{
						SetBottomActive(m_bottomMask, true);
						UpdateBottomBounds();
					}
				}


			}
			catch(Exception e)
			{
				Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}
		}

		void SetBottomActive(GameObject bottom, bool active)
		{
			if(bottom != null)
				bottom.SetActive(active);
		}

        /// <summary>
        /// Convert the inscatter mode enum to a vector mask for the shader.
        /// </summary>
		Vector3 InscatterModeToMask(INSCATTER_MODE mode)
		{
			switch(mode)
			{
			case INSCATTER_MODE.LINEAR:
				return new Vector3(1,0,0);

			case INSCATTER_MODE.EXP:
				return new Vector3(0,1,0);

			case INSCATTER_MODE.EXP2:
				return new Vector3(0,0,1);

			default:
				return new Vector3(0,0,1);
			}

		}

        /// <summary>
        /// Moves the bottom mesh to where the camera is.
        /// </summary>
		void FitBottomToCamera()
		{

			if(!enabled || m_bottomMask == null) return;

			Camera cam = Camera.current;

			Vector3 pos = cam.transform.position;
			//Scale must be greater than the fade used in the fade dist used
			//in the OceanDisplacement.cginc OceanPositionAndDisplacement function
			float far = cam.farClipPlane * 0.85f;

            m_bottomMask.transform.localScale = new Vector3(far, OCEAN_BOTTOM_DEPTH, far);

			float depthOffset = 0.0f;
            m_bottomMask.transform.localPosition = new Vector3(pos.x, -OCEAN_BOTTOM_DEPTH + m_ocean.level - depthOffset, pos.z);

		}

		/// <summary>
		/// Need to make bounds big enough that every camera will
		/// render it. The position of the bottom will then be fitted 
		/// to the cameras position on render.
		/// </summary>
		void UpdateBottomBounds()
		{

			float bigNumber = 1e8f;
			float level = m_ocean.level;

			if(m_bottomMask != null && m_bottomMask.activeSelf)
			{
                Bounds bounds = new Bounds(new Vector3(0.0f, level, 0.0f), new Vector3(bigNumber, OCEAN_BOTTOM_DEPTH, bigNumber));

                m_bottomMask.GetComponent<MeshFilter>().mesh.bounds = bounds;
			}

		}

        /// <summary>
        /// Gets the reflection layer mask from the camera settings 
        /// if provided else use the default mask
        /// </summary>
        LayerMask GetOceanDepthsLayermask(OceanCameraSettings settings)
        {
            return (settings != null) ? settings.oceanDepthsMask : oceanDepthsMask;
        }

        /// <summary>
        /// Gets if this camera should render the reflections.
        /// </summary>
        bool GetDisableUnderwater(OceanCameraSettings settings)
        {
            return (settings != null) ? settings.disableUnderwater : false;
        }

        /// <summary>
        /// Renders the ocean mask. The mask is used in the underwater post effect
        /// shader and contains a 1 or 0 in the rgb channels if the pixel is on the
        /// top of the ocean mesh, on the under side of mesh or below the ocean mesh.
        /// The w channel also contains the meshes depth value as if the normal ocean
        /// material does not write to depth buffer the shader wont be able to get its
        /// depth value.
        /// </summary>
		public override void RenderOceanMask(GameObject go)
        {

			try
			{

	            if (!enabled) return;

	            if (oceanMaskSdr == null) return;

	            if (underwaterMode == UNDERWATER_MODE.ABOVE_ONLY) return;

	            Camera cam = Camera.current;
                if (cam == null) return;

                CameraData data = m_ocean.FindCameraData(cam);

                if (data.mask == null)
	                data.mask = new MaskData();

	            if (data.mask.updated) return;
                
                if (cam.name == "SceneCamera" || cam.GetComponent<UnderWaterPostEffect>() == null || SystemInfo.graphicsShaderLevel < 30 || GetDisableUnderwater(data.settings))
                {
                    //Scene camera should never need the mask so just bind something that wont cause a issue.
                    //If the camera is not using a post effect there is no need for the mask to be rendered.

                    Shader.SetGlobalTexture("Ceto_OceanMask", Texture2D.blackTexture);
                    data.mask.updated = true;
                }
                else
                {

                    CreateMaskCameraFor(cam, data.mask);

                    FitBottomToCamera();

                    NotifyOnEvent.Disable = true;
                    data.mask.cam.RenderWithShader(oceanMaskSdr, "OceanMask");
                    NotifyOnEvent.Disable = false;

                    Shader.SetGlobalTexture("Ceto_OceanMask", data.mask.cam.targetTexture);

                    data.mask.updated = true;
                }

			}
			catch(Exception e)
			{
				Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}
        }

        /// <summary>
        /// Render depth information about a object using a replacement shader.
        /// If the ocean renders into the depth buffer then the shader can not get 
        /// depth info about what has been rendered under it as Unity will have already 
        /// written the ocean mesh into depth buffer by then.
        /// Will also create the refraction grab if needed.
        /// </summary>
		public override void RenderOceanDepth(GameObject go)
        {

			try
			{

                if (!enabled) return;

                Camera cam = Camera.current;
                if (cam == null) return;

                CameraData data = m_ocean.FindCameraData(cam);

                if (data.depth == null)
                {
                    data.depth = new DepthData();
                }

                if (data.depth.updated) return;

                //zero the texture so if there is a issue the underwater effect will not screw up the rest of the rendering.
                Shader.SetGlobalTexture(Ocean.REFRACTION_GRAB_TEXTURE_NAME, Texture2D.blackTexture);
                Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
                Shader.SetGlobalTexture("Ceto_OceanDepth", Texture2D.whiteTexture);
                
                //If this camera has disable the underwater turn it off in the shader and return.
                if (GetDisableUnderwater(data.settings))
                {
                    Shader.DisableKeyword("CETO_UNDERWATER_ON");
                    data.depth.updated = true;
                    return;
                }
                else
                {
                    Shader.EnableKeyword("CETO_UNDERWATER_ON");
                }
                
                if (cam.name == "SceneCamera" || SystemInfo.graphicsShaderLevel < 30)
                {

                    //Adding command to scene camera is causing some problems and may as 
                    //well not use the ocean depth pass so its consistent between modes
                    //so just bind something that wont cause a issue.

                    //If not using the ocean depths all thats needed is the IVP
                    //to extract the world pos from the depth buffer.
                    Matrix4x4 ivp = cam.projectionMatrix * cam.worldToCameraMatrix;
                    Shader.SetGlobalMatrix("Ceto_Camera_IVP", ivp.inverse);

                    data.depth.updated = true;
                }
                else if (depthMode == DEPTH_MODE.USE_DEPTH_BUFFER)
                {

                    //Cam must have depth mode enabled to use depth buffer.
                    cam.depthTextureMode |= DepthTextureMode.Depth;

                    //If not using the ocean depths all thats needed is the IVP
                    //to extract the world pos from the depth buffer.
                    Matrix4x4 ivp = cam.projectionMatrix * cam.worldToCameraMatrix;
                    Shader.SetGlobalMatrix("Ceto_Camera_IVP", ivp.inverse);

                    //Add the command to camera. 
                    //The default implementation will just grab the depth and the screen.
                    CreateRefractionGrab(cam, data.depth);
                   
                    data.depth.updated = true;
                }
                else if (depthMode == DEPTH_MODE.USE_OCEAN_DEPTH_PASS)
                {

                    CreateDepthCameraFor(cam, data.depth);
                    CreateRefractionGrab(cam, data.depth);

                    data.depth.cam.cullingMask = GetOceanDepthsLayermask(data.settings);
                    data.depth.cam.cullingMask = OceanUtility.HideLayer(data.depth.cam.cullingMask, Ocean.OCEAN_LAYER);

                    NotifyOnEvent.Disable = true;
                    data.depth.cam.RenderWithShader(oceanDepthSdr, "RenderType");
                    NotifyOnEvent.Disable = false;

                    Shader.SetGlobalTexture("Ceto_OceanDepth", data.depth.cam.targetTexture);

                    data.depth.updated = true;
                }

			}
			catch(Exception e)
			{
				Ocean.LogError(e.ToString());
				WasError = true;
				enabled = false;
			}
        }

        /// <summary>
        /// Create the camera used for the mask.
        /// </summary>
        void CreateMaskCameraFor(Camera cam, MaskData data)
        {

            if (data.cam == null)
            {

                GameObject go = new GameObject("Ceto Mask Camera: " + cam.name);
                go.hideFlags = HideFlags.HideAndDontSave;
				go.AddComponent<IgnoreOceanEvents>();
				go.AddComponent<DisableFog>();
				go.AddComponent<DisableShadows>();

                data.cam = go.AddComponent<Camera>();

                //data.cam.CopyFrom(cam); //This will cause a recursive culling error in Unity >= 5.4

                data.cam.clearFlags = CameraClearFlags.SolidColor;
                data.cam.backgroundColor = Color.black;
                data.cam.cullingMask = 1 << LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
                data.cam.enabled = false;
                data.cam.renderingPath = RenderingPath.Forward;
                data.cam.targetTexture = null;
				data.cam.useOcclusionCulling = false;
				//data.cam.opaqueSortMode = OpaqueSortMode.NoDistanceSort;
                data.cam.RemoveAllCommandBuffers();
                data.cam.targetTexture = null;

            }

            data.cam.fieldOfView = cam.fieldOfView;
            data.cam.nearClipPlane = cam.nearClipPlane;
            data.cam.farClipPlane = cam.farClipPlane;
            data.cam.transform.position = cam.transform.position;
            data.cam.transform.rotation = cam.transform.rotation;
            data.cam.worldToCameraMatrix = cam.worldToCameraMatrix;
            data.cam.projectionMatrix = cam.projectionMatrix;
            data.cam.orthographic = cam.orthographic;
            data.cam.aspect = cam.aspect;
            data.cam.orthographicSize = cam.orthographicSize;
            data.cam.rect = new Rect(0, 0, 1, 1);

            //If mask far plane is less than double the ocean bottom depth
            //it will contain a semicircle artefact.
            if (data.cam.farClipPlane < OCEAN_BOTTOM_DEPTH * 2)
            {
                data.cam.farClipPlane = OCEAN_BOTTOM_DEPTH * 2;
                data.cam.ResetProjectionMatrix();
            }

            RenderTexture tex = data.cam.targetTexture;

            if (tex == null || tex.width != cam.pixelWidth || tex.height != cam.pixelHeight)
            {
                if (tex != null)
                    RTUtility.ReleaseAndDestroy(tex);

                int width = cam.pixelWidth;
                int height = cam.pixelHeight;
                int depth = 32;

                data.cam.targetTexture = new RenderTexture(width, height, depth, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
				data.cam.targetTexture.filterMode = FilterMode.Point;
				data.cam.targetTexture.hideFlags = HideFlags.DontSave;
                data.cam.targetTexture.name = "Ceto Mask Render Target: " + cam.name;

            }

        }

        /// <summary>
        /// Create the camera used for the ocean depths.
        /// </summary>
        void CreateDepthCameraFor(Camera cam, DepthData data)
        {

            if (data.cam == null)
            {

                GameObject go = new GameObject("Ceto Depth Camera: " + cam.name);
                go.hideFlags = HideFlags.HideAndDontSave;
				go.AddComponent<IgnoreOceanEvents>();
				go.AddComponent<DisableFog>();
				go.AddComponent<DisableShadows>();

                data.cam = go.AddComponent<Camera>();

                //data.cam.CopyFrom(cam); //This will cause a recursive culling error in Unity >= 5.4

                data.cam.clearFlags = CameraClearFlags.SolidColor;
                data.cam.backgroundColor = Color.white;
                data.cam.enabled = false;
                data.cam.renderingPath = RenderingPath.Forward;
                data.cam.targetTexture = null;
				data.cam.useOcclusionCulling = false;
                data.cam.RemoveAllCommandBuffers();
                data.cam.targetTexture = null;

            }

            data.cam.fieldOfView = cam.fieldOfView;
            data.cam.nearClipPlane = cam.nearClipPlane;
            data.cam.farClipPlane = cam.farClipPlane;
            data.cam.transform.position = cam.transform.position;
            data.cam.transform.rotation = cam.transform.rotation;
            data.cam.worldToCameraMatrix = cam.worldToCameraMatrix;
            data.cam.projectionMatrix = cam.projectionMatrix;
            data.cam.orthographic = cam.orthographic;
            data.cam.aspect = cam.aspect;
            data.cam.orthographicSize = cam.orthographicSize;
            data.cam.rect = new Rect(0, 0, 1, 1);
            data.cam.layerCullDistances = cam.layerCullDistances;
            data.cam.layerCullSpherical = cam.layerCullSpherical;

            RenderTexture tex = data.cam.targetTexture;

            if (tex == null || tex.width != cam.pixelWidth || tex.height != cam.pixelHeight)
            {
                if (tex != null)
                    RTUtility.ReleaseAndDestroy(tex);

                int width = cam.pixelWidth;
                int height = cam.pixelHeight;
                int depth = 24;

                RenderTextureFormat format;
                if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGFloat))
                    format = RenderTextureFormat.RGFloat;
                else
                    format = RenderTextureFormat.RGHalf;

                data.cam.targetTexture = new RenderTexture(width, height, depth, format, RenderTextureReadWrite.Linear);
                data.cam.targetTexture.filterMode = FilterMode.Bilinear;
				data.cam.targetTexture.hideFlags = HideFlags.DontSave;
                data.cam.targetTexture.name = "Ceto Ocean Depths Render Target: " + cam.name;

            }

        }

        /// <summary>
        /// Create the refraction command buffer.
        /// </summary>
        void CreateRefractionGrab(Camera cam, DepthData data)
        {

            IRefractionCommand cmd = GetRefractionCommand();
            CameraEvent cmdEvent = GetCommandEvent(cam);

            //The command has already been created.
            if (data.grabCmd != null)
            {

                if (depthMode != DEPTH_MODE.USE_DEPTH_BUFFER ||
                    refractionResolution != cmd.Resolution ||
                    cmdEvent != data.cmdEvent ||
                    !cmd.Matches(cam))
                {
                    //The current cmd settings no longer match camera 
                    //or the depth mode is no longer DEPTH_BUFFER
                    //Remove command and release.
                    cmd.Remove(cam);
                    data.grabCmd = null;
                }
                else
                {
                    //The settings have not changed.
                    return;
                }

            }

            //If the depth mode is USE_DEPTH_BUFFER or useCmdBufferForGrab
            //is true then a command buffer is needed.
            if (depthMode == DEPTH_MODE.USE_DEPTH_BUFFER)
            {
                cmd.Resolution = refractionResolution;
                cmd.Event = cmdEvent;

                data.grabCmd = cmd.Create(cam);
                data.cmdEvent = cmdEvent;
            }
            
        }

        /// <summary>
        /// Returns what camera event the command should run 
        /// at depending on the cameras rendering mode.
        /// </summary>
        CameraEvent GetCommandEvent(Camera cam)
        {

            RenderingPath path = cam.actualRenderingPath;

            if (path == RenderingPath.DeferredShading)
                return DEFERRED_EVENT;
            else if (path == RenderingPath.DeferredLighting)
                return LEGACY_DEFERRED_EVENT;
            else
                return FORWARD_EVENT;

        }

        /// <summary>
        /// If a custom command has been provided use that one.
        /// The custom command must copy the screen and depth buffer
        /// into the correct textures but other than that its is free
        /// to post process the grab in anyway.
        /// </summary>
        IRefractionCommand GetRefractionCommand()
        {

            IRefractionCommand cmd = null;

            if (CustomRefractionCommand != null)
            {
                cmd = CustomRefractionCommand;
                //not using default cmd.
                //make sure it has been removed from cameras.
                m_refractionCommand.RemoveAll();
            }
            else
                cmd = m_refractionCommand;

            return cmd;
        }

        /// <summary>
        /// Create the bottom mesh. Just a radial mesh with the edges pulled up to surface.
        /// </summary>
        Mesh CreateBottomMesh(int segementsX, int segementsY)
        {

            Vector3[] vertices = new Vector3[segementsX * segementsY];
            Vector2[] texcoords = new Vector2[segementsX * segementsY];

            float TAU = Mathf.PI * 2.0f;
            float r;
            for (int x = 0; x < segementsX; x++)
            {
                for (int y = 0; y < segementsY; y++)
                {
                    r = (float)x / (float)(segementsX - 1);

                    vertices[x + y * segementsX].x = r * Mathf.Cos(TAU * (float)y / (float)(segementsY - 1));
                    vertices[x + y * segementsX].y = 0.0f;
                    vertices[x + y * segementsX].z = r * Mathf.Sin(TAU * (float)y / (float)(segementsY - 1));

                    if (x == segementsX - 1)
                    {
                        vertices[x + y * segementsX].y = 1.0f;
                    }
                }
            }

            int[] indices = new int[segementsX * segementsY * 6];

            int num = 0;
            for (int x = 0; x < segementsX - 1; x++)
            {
                for (int y = 0; y < segementsY - 1; y++)
                {
                    indices[num++] = x + y * segementsX;
                    indices[num++] = x + (y + 1) * segementsX;
                    indices[num++] = (x + 1) + y * segementsX;

                    indices[num++] = x + (y + 1) * segementsX;
                    indices[num++] = (x + 1) + (y + 1) * segementsX;
                    indices[num++] = (x + 1) + y * segementsX;

                }
            }

            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = texcoords;
            mesh.triangles = indices;
            mesh.name = "Ceto Bottom Mesh";
            mesh.hideFlags = HideFlags.HideAndDontSave;
            ;

            return mesh;

        }

	}

}








