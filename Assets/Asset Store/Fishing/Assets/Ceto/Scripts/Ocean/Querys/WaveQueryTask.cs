using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Ceto.Common.Threading.Tasks;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{
	
	public class WaveQueryTask : ThreadedTask
	{

		IList<InterpolatedArray2f> m_displacements;

		IEnumerable<WaveQuery> m_querys;

		int m_enabled;

		Action<IEnumerable<WaveQuery>> m_callBack;

		float m_level;

		QueryGridScaling m_scaling;

		public WaveQueryTask(WaveSpectrumBase spectrum, float level, Vector3 offset, IEnumerable<WaveQuery> querys, Action<IEnumerable<WaveQuery>> callBack) : base(true)
		{

			IDisplacementBuffer buffer = spectrum.DisplacementBuffer;

			buffer.CopyAndCreateDisplacements(out m_displacements);

			m_querys = querys;
			m_callBack = callBack;
			m_enabled = buffer.EnabledBuffers();
			m_level = level;

            Vector4 invGridSizes = new Vector4();
            invGridSizes.x = 1.0f / (spectrum.GridSizes.x * spectrum.GridScale);
            invGridSizes.y = 1.0f / (spectrum.GridSizes.y * spectrum.GridScale);
            invGridSizes.z = 1.0f / (spectrum.GridSizes.z * spectrum.GridScale);
            invGridSizes.w = 1.0f / (spectrum.GridSizes.w * spectrum.GridScale);

            m_scaling = new QueryGridScaling();
			m_scaling.invGridSizes = invGridSizes;
			m_scaling.choppyness = spectrum.Choppyness * spectrum.GridScale;
			m_scaling.scaleY = spectrum.GridScale;
            m_scaling.offset = offset;
            //tmp buffer to reduce memory allocations.
            m_scaling.tmp = new float[QueryDisplacements.CHANNELS];

			
		}
		
		public override void Start()
		{
			
			base.Start();
			
		}
		
		public override IEnumerator Run()
		{
            RunQueries();

            FinishedRunning();
			return null;
		}
		
		public override void End()
		{
			
			base.End();

			m_callBack(m_querys);

		}

        void RunQueries()
        {
            var e = m_querys.GetEnumerator();
            while (e.MoveNext())
            {

                if (Cancelled) return;

                WaveQuery query = e.Current;

                query.result.Clear();

                //Only these modes are relevant to this code.
                if (query.mode == QUERY_MODE.DISPLACEMENT || query.mode == QUERY_MODE.POSITION)
                {
                    QueryDisplacements.QueryWaves(query, m_enabled, m_displacements, m_scaling);
                }

                query.result.height += m_level;
            }
        }

	}
}




