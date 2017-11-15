using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

using Ceto.Common.Threading.Tasks;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{
	
	public class FindRangeTask : ThreadedTask
	{

		IList<InterpolatedArray2f> m_displacements;

		WaveSpectrumBase m_spectrum;

		Vector4 m_max;

		Vector4 m_choppyness;

		Vector2 m_gridScale;

		public FindRangeTask(WaveSpectrumBase spectrum) : base(true)
		{

			m_spectrum = spectrum;
			m_choppyness = spectrum.Choppyness;
			m_gridScale = new Vector2(spectrum.GridScale, spectrum.GridScale);

			IDisplacementBuffer buffer = spectrum.DisplacementBuffer;
			buffer.CopyAndCreateDisplacements(out m_displacements);

		}

		public override void Reset()
		{

			base.Reset();

			m_choppyness = m_spectrum.Choppyness;
			m_gridScale = new Vector2(m_spectrum.GridScale, m_spectrum.GridScale);

            IDisplacementBuffer buffer = m_spectrum.DisplacementBuffer;
			buffer.CopyDisplacements(m_displacements);

		}

		public override IEnumerator Run()
		{

			m_max = QueryDisplacements.MaxRange(m_displacements, m_choppyness, m_gridScale, this);

			FinishedRunning();
			return null;
		}

		public override void End ()
		{

			m_spectrum.MaxDisplacement = new Vector2(Mathf.Max(m_max.x, m_max.z), m_max.y);

			base.End();

		}

	}

}










