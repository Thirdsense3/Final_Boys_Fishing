
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;


namespace Ceto
{

    /// <summary>
    /// Base implementation of the refraction command.
    /// Inherit from do reuse base methods.
    /// </summary>
    public abstract class RefractionCommandBase : IRefractionCommand
    {

        /// <summary>
        /// Used to store info about the created command.
        /// </summary>
        public class CommandData
        {
            public CommandBuffer command;
            public int width;
            public int height;
        }

        /// <summary>
        /// The camera event when the command should be executed.
        /// </summary>
        public CameraEvent Event { get; set; }

        /// <summary>
        /// Resolution relative to screen of refraction grab.
        /// </summary>
        public REFRACTION_RESOLUTION Resolution { get; set; }

        /// <summary>
        /// All the cameras the cmd has been added to.
        /// </summary>
        protected Dictionary<Camera, CommandData> m_data;


        /// <summary>
        /// 
        /// </summary>
        public RefractionCommandBase()
        {

            m_data = new Dictionary<Camera, CommandData>();

        }

        /// <summary>
        /// Return a new command buffer.
        /// This will be called the first time
        /// the mesh is rendered for each camera 
        /// that renders the ocean.
        /// </summary>
        public abstract CommandBuffer Create(Camera cam);

        /// <summary>
        /// Remove the command buffer from the camera.
        /// </summary>
        public virtual void Remove(Camera cam)
        {
            if (m_data.ContainsKey(cam))
            {
                CommandData data = m_data[cam];

                cam.RemoveCommandBuffer(Event, data.command);
                m_data.Remove(cam);
            }
        }

        /// <summary>
        /// Remove the command buffer from all the cameras.
        /// </summary>
        public virtual void RemoveAll()
        {

            if (m_data.Count == 0) return;

            var e = m_data.GetEnumerator();

            while (e.MoveNext())
            {
                Camera cam = e.Current.Key;
                CommandBuffer cmd = e.Current.Value.command;

                cam.RemoveCommandBuffer(Event, cmd);
            }

            m_data.Clear();

        }

        /// <summary>
        /// Does the cmd created for this camera match the current camera settings.
        /// </summary>
        public virtual bool Matches(Camera cam)
        {
            if (!m_data.ContainsKey(cam)) return false;

            CommandData data = m_data[cam];

            if (data.width != cam.pixelWidth) return false;
            if (data.height != cam.pixelHeight) return false;

            return true;
        }


        /// <summary>
        /// Convert the setting enum to a meaning full number.
        /// </summary>
        protected virtual int ResolutionToNumber(REFRACTION_RESOLUTION resolution)
        {

            switch (resolution)
            {
                case REFRACTION_RESOLUTION.FULL:
                    return 1;

                case REFRACTION_RESOLUTION.HALF:
                    return 2;

                case REFRACTION_RESOLUTION.QUARTER:
                    return 4;

                default:
                    return 2;
            }

        }

    }

}




