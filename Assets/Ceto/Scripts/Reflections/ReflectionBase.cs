using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ceto
{
    /// <summary>
    /// Abstract base class for handling reflection rendering.
    /// </summary>
	[DisallowMultipleComponent]
	public abstract class ReflectionBase : OceanComponent
	{

        /// <summary>
        /// Render the reflections for the gameobject.
        /// </summary>
		public abstract void RenderReflection(GameObject go);

        /// <summary>
        /// Assign to provide your custom reflection rendering method.
        /// The function must have the following signature...
        /// 
        /// RenderTexture YourReflectionMethod(GameObject go);
        /// 
        /// The gameobjects transform contains the position of the 
        /// object requiring the reflections.
        /// 
        /// The returned render texture is the reflections rendered for the current camera.
        /// Ceto will not modify this texture or keep a reference to it.
        /// 
        /// </summary>
		public Func<GameObject, RenderTexture> RenderReflectionCustom;

	}

}