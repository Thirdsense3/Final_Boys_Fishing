
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;


namespace Ceto
{

    /// <summary>
    /// Default implementation of the refraction command.
    /// Grabs the screen and depth buffer.
    /// </summary>
    public class RefractionCommand : RefractionCommandBase
    {


        /// <summary>
        /// The name the refraction texture will be bound to.
        /// </summary>
        public string GrabName { get; private set; }

        /// <summary>
        /// The name the refraction texture will be bound to.
        /// </summary>
        public string DepthName { get; private set; }

        /// <summary>
        /// Material to copy the depth buffer into a texture.
        /// </summary>
        public Material m_copyDepthMat;

        /// <summary>
        /// 
        /// </summary>
        public RefractionCommand(Shader copyDepth)
        {
			GrabName = Ocean.REFRACTION_GRAB_TEXTURE_NAME;

            DepthName = Ocean.DEPTH_GRAB_TEXTURE_NAME;

            m_copyDepthMat = new Material(copyDepth);

            m_data = new Dictionary<Camera, CommandData>();
        }

        /// <summary>
        /// Return a new command buffer.
        /// This will be called the first time
        /// the mesh is rendered for each camera 
        /// that renders the ocean.
        /// </summary>
        public override CommandBuffer Create(Camera cam)
        {

            CommandBuffer cmd = new CommandBuffer();
            cmd.name = "Ceto DepthGrab Cmd: " + cam.name;

            //int width = cam.pixelWidth;
            //int height = cam.pixelHeight;

            //int scale = ResolutionToNumber(Resolution);
            //width /= scale;
            //height /= scale;

            RenderTextureFormat format;

            //screen grab currently disabled.
            /*
            if (cam.hdr)
                format = RenderTextureFormat.ARGBHalf;
            else
                format = RenderTextureFormat.ARGB32;

            //Copy screen into temporary RT.
            int grabID = Shader.PropertyToID("Ceto_GrabCopyTexture");
			cmd.GetTemporaryRT(grabID, width, height, 0, FilterMode.Bilinear, format, RenderTextureReadWrite.Default);
			cmd.Blit(BuiltinRenderTextureType.CurrentActive, grabID);
			cmd.SetGlobalTexture(GrabName, grabID);
            
            */
            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat))
                format = RenderTextureFormat.RFloat;
            else
                format = RenderTextureFormat.RHalf;

            //Copy depths into temporary RT.
            int depthID = Shader.PropertyToID("Ceto_DepthCopyTexture");
            cmd.GetTemporaryRT(depthID, cam.pixelWidth, cam.pixelHeight, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
            cmd.Blit(BuiltinRenderTextureType.CurrentActive, depthID, m_copyDepthMat, 0);
            cmd.SetGlobalTexture(DepthName, depthID);

            cam.AddCommandBuffer(Event, cmd);

            CommandData data = new CommandData();

            data.command = cmd;
            data.width = cam.pixelWidth;
            data.height = cam.pixelHeight;

            if (m_data.ContainsKey(cam))
                m_data.Remove(cam);

            m_data.Add(cam, data);

            return cmd;
        }

    }

}




