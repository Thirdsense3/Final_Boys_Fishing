using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Ceto.Common.Threading.Tasks;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{

	public class FourierTask : ThreadedTask
	{

		FourierCPU m_fourier;

		WaveSpectrumBufferCPU m_buffer;

        int m_numGrids;

		int m_index;

		IList<Vector4[]> m_data;

		Color[] m_results;

		bool m_doublePacked;

		public FourierTask(WaveSpectrumBufferCPU buffer, FourierCPU fourier, int index, int numGrids) 
            : base(true)
		{

			if(m_index == -1)
				throw new InvalidOperationException("Index can be -1. Fourier for multiple buffers is not being used");

			m_buffer = buffer;

			m_fourier = fourier;

			m_index = index;

            m_numGrids = numGrids;

			var b = m_buffer.GetBuffer(m_index);
			
			m_data = b.data;
			m_results = b.results;
			m_doublePacked = b.doublePacked;

		}

		public override void Start()
		{
			base.Start();
		}
		
		public override IEnumerator Run()
		{

			PerformSingleFourier();

			FinishedRunning();
			return null;
		}
		
		public override void End()
		{

			base.End();

			m_buffer.PackData(m_index);

		}

		void PerformSingleFourier()
		{

			//Always start writing at buffer index 0 and the end read buffer should always end up at index 1.
			const int write = 0;
			int read = -1;

			if(m_doublePacked)
				read = m_fourier.PeformFFT_DoublePacked(write, m_data, this);
			else
				read = m_fourier.PeformFFT_SinglePacked(write, m_data, this);

            if (Cancelled) return;
			
			if(read != WaveSpectrumBufferCPU.READ)
				throw new InvalidOperationException("Fourier transform did not result in the read buffer at index " + WaveSpectrumBufferCPU.READ);

			//If threading of data processing not disabled do it here.
			if (!Ocean.DISABLE_PROCESS_DATA_MULTITHREADING)
				m_buffer.ProcessData(m_index, m_results, m_data[read], m_numGrids);
				
		}

	}

}














