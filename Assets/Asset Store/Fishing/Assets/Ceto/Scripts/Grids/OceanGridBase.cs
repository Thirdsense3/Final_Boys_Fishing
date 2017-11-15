using UnityEngine;
using System;
using System.Collections.Generic;


namespace Ceto
{

	[DisallowMultipleComponent]
	public abstract class OceanGridBase : OceanComponent
	{


        /// <summary>
        /// If true this will force the mesh to be recreated.
        /// Once recreate it will be set to false again.
        /// </summary>
        public bool ForceRecreate { get; set; }


    }

}

