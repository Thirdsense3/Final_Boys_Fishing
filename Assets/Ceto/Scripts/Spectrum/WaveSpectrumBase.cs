using UnityEngine;
using System.Collections;

namespace Ceto
{

    /// <summary>
    /// Abstract base class for the wave spectrum.
    /// Contains some common properties.
    /// </summary>
	[DisallowMultipleComponent]
	public abstract class WaveSpectrumBase : OceanComponent
	{

        /// <summary>
        /// Get the current spectrum grid sizes.
        /// </summary>
		public abstract Vector4 GridSizes { get; }

        /// <summary>
        /// Get the current wave choppyness.
        /// </summary>
		public abstract Vector4 Choppyness { get; }

        /// <summary>
        /// Get the current grid scale.
        /// </summary>
		public abstract float GridScale { get; }

        /// <summary>
        /// The maximum possible displacement the wave can generate.
        /// The x value is the max horizontal and the y is the max vertical.
        /// </summary>
		public abstract Vector2 MaxDisplacement { get; set; }

        /// <summary>
        /// If the read backs form the GPU has been disabled.
        /// </summary>
		public abstract bool DisableReadBack { get; }

        /// <summary>
        /// Get a interface to the displacement buffer.
        /// The actually displacement buffer may be a CPU or GPU buffer.
        /// </summary>
		public abstract IDisplacementBuffer DisplacementBuffer { get; }

        /// <summary>
        /// Query the waves for the wave height at a location.
        /// </summary>
		public abstract void QueryWaves(WaveQuery query);

        /// <summary>
        /// Used to add a custom spectrum type.
        /// If this is not null and the spectrum type has been set to CUSTOM
        /// this interface will be used to create the required data.
        /// </summary>
        public ICustomWaveSpectrum CustomWaveSpectrum { get; set; }

	}

}
