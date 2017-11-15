using System;

namespace Ceto.Common.Containers.Interpolation
{
	
	/// <summary>
	/// A Interpolated 2 dimensional array.
	/// The array can be sampled using a float and the sampling
	/// will be performed using bilinear filtering.
	/// </summary>
	public interface IInterpolatedArray2
	{
		
		/// <summary>
		/// Size on the x dimension.
		/// </summary>
		int SX { get; }

		/// <summary>
		/// Size on the y dimension.
		/// </summary>
		int SY { get; }

		/// <summary>
		/// Number of channels.
		/// </summary>
		int Channels { get; }

        /// <summary>
        /// Should the interpolation be done with a 
        /// half pixel offset.
        /// </summary>
        bool HalfPixelOffset { get; set; }

        /// <summary>
        /// Should the sampling of the array be wrapped or clamped.
        /// </summary>
        bool Wrap { get; set; }
		
		/// <summary>
		/// Get a value from the data array using normal indexing.
		/// </summary>
		float this[int x, int y, int c] { get; set; }

		/// <summary>
		/// Clear the data in array to 0.
		/// </summary>
		void Clear();

		/// <summary>
		/// Copy the specified data.
		/// </summary>
		void Copy(Array data);

		/// <summary>
		/// Set a channels.
		/// </summary>
		void Set(int x, int y, int c, float v);

		/// <summary>
		/// Set all channels from array
		/// </summary>
		void Set(int x, int y, float[] v);
		
		/// <summary>
		/// Get a channel
		/// </summary>
		float Get(int x, int y, int c);

		/// <summary>
		/// Get all channels into array
		/// </summary>
		void Get(int x, int y, float[] v);
		
		/// <summary>
		/// Get a value from the data array using bilinear filtering.
		/// </summary>
		void Get(float x, float y, float[] v);

        /// <summary>
        /// Get a value from the data array using bilinear filtering.
        /// </summary>
        float Get(float x, float y, int c);
		
	}
	
	
}

