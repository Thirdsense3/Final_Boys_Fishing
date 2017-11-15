using UnityEngine;
using System;
using System.Collections.Generic;

using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Containers.Interpolation;

namespace Ceto
{

	public class DisplacementBufferCPU : WaveSpectrumBufferCPU, IDisplacementBuffer
	{
		
		const int NUM_BUFFERS = 3;

		IList<InterpolatedArray2f[]> m_displacements;

		public DisplacementBufferCPU(int size, Scheduler scheduler) : base(size, NUM_BUFFERS, scheduler)
		{

			int GRIDS = QueryDisplacements.GRIDS;
			int CHANNELS = QueryDisplacements.CHANNELS;

			m_displacements = new List<InterpolatedArray2f[]>(2);

			m_displacements.Add( new InterpolatedArray2f[GRIDS] );
			m_displacements.Add( new InterpolatedArray2f[GRIDS] );

			for (int i = 0; i < GRIDS; i++)
			{
				m_displacements[0][i] = new InterpolatedArray2f(size, size, CHANNELS, true);
				m_displacements[1][i] = new InterpolatedArray2f(size, size, CHANNELS, true);
			}

		}

		protected override void Initilize(WaveSpectrumCondition condition, float time)
		{

			InterpolatedArray2f[] displacements = GetWriteDisplacements();

			displacements[0].Clear();
			displacements[1].Clear();
			displacements[2].Clear();
			displacements[3].Clear();

            if (m_initTask == null)
            {
                m_initTask = condition.GetInitSpectrumDisplacementsTask(this, time);
            }
            else if(m_initTask.SpectrumType != condition.Key.SpectrumType || m_initTask.NumGrids != condition.Key.NumGrids)
            {
                m_initTask = condition.GetInitSpectrumDisplacementsTask(this, time);
            }
            else
            {
                m_initTask.Reset(condition, time);
            }
			
		}

        public InterpolatedArray2f[] GetWriteDisplacements()
		{
			return m_displacements[WRITE];
		}

		public InterpolatedArray2f[] GetReadDisplacements()
		{
			return m_displacements[READ];
		}

		public override void Run(WaveSpectrumCondition condition, float time)
		{
			SwapDisplacements();
			base.Run(condition, time);
		}

		public void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] source = GetReadDisplacements();
			QueryDisplacements.CopyAndCreateDisplacements(source, out displacements);
		}

		public void CopyDisplacements(IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] source = GetReadDisplacements();
			QueryDisplacements.CopyDisplacements(source, displacements);
		}

		void SwapDisplacements()
		{

			InterpolatedArray2f[] tmp = m_displacements[0];
			m_displacements[0] = m_displacements[1];
			m_displacements[1] = tmp;

		}

        /// <summary>
        /// When the fourier task is finished 
        /// PackData is called to load the results 
        /// into the map texture. 
        /// </summary>
        public override void PackData(int index)
		{

            //If processing data on fourier thread disabled 
            //do it here on main thread.
			if (Ocean.DISABLE_PROCESS_DATA_MULTITHREADING)
            {
                IList<IList<Vector4[]>> data = GetData(index);
                IList<Color[]> results = GetResults(index);

                for (int i = 0; i < results.Count; i++)
                {
                    Color[] result = results[i];
                    Vector4[] readData = data[i][READ];

                    int INDEX = (index == -1) ? i : index;

                    ProcessData(INDEX, result, readData, m_initTask.NumGrids);
                }
            }

            base.PackData(index);

        }

        /// <summary>
        /// After the fourier tasks creates the data this function is 
        /// called which allows the buffer to further process the results if needed.
        /// Used to sort the results into the displacement buffers that are 
        /// sampled from for the wave queries and copy into the result array
        /// which gets copied into the textures. This is needed as the packing of 
        /// the data thats optimal for the FFT is not the same as what optimal for 
        /// sampling from the textures and displacement buffer.
        /// This maybe called from the fourier task thread or the main thread
        /// so must be thread safe.
        /// </summary>
        public override void ProcessData(int index, Color[] result, Vector4[] data, int numGrids)
        {

            //TODO - move me into fouier task script.

            int CHANNELS = QueryDisplacements.CHANNELS;
            int size = Size;

            InterpolatedArray2f[] displacements = GetWriteDisplacements();

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int j = x + y * size;
                    int IDX = j * CHANNELS;

                     if (numGrids == 1)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = 0.0f;
                        result[j].a = 0.0f;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                        }
                    }
                    else if (numGrids == 2)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = data[j].z;
                        result[j].a = data[j].w;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                            displacements[1].Data[IDX + 1] = result[j].g;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                            displacements[1].Data[IDX + 0] += result[j].b;
                            displacements[1].Data[IDX + 2] += result[j].a;
                        }
                    }
                    else if (numGrids == 3)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = data[j].z;
                        result[j].a = data[j].w;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                            displacements[1].Data[IDX + 1] = result[j].g;
                            displacements[2].Data[IDX + 1] = result[j].b;
                            displacements[3].Data[IDX + 1] = result[j].a;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                            displacements[1].Data[IDX + 0] += result[j].b;
                            displacements[1].Data[IDX + 2] += result[j].a;
                        }
                        else if (index == 2)
                        {
                            displacements[2].Data[IDX + 0] += result[j].r;
                            displacements[2].Data[IDX + 2] += result[j].g;
                        }
                    }
                    else if (numGrids == 4)
                    {

                        result[j].r = data[j].x;
                        result[j].g = data[j].y;
                        result[j].b = data[j].z;
                        result[j].a = data[j].w;

                        if (index == 0)
                        {
                            displacements[0].Data[IDX + 1] = result[j].r;
                            displacements[1].Data[IDX + 1] = result[j].g;
                            displacements[2].Data[IDX + 1] = result[j].b;
                            displacements[3].Data[IDX + 1] = result[j].a;
                        }
                        else if (index == 1)
                        {
                            displacements[0].Data[IDX + 0] += result[j].r;
                            displacements[0].Data[IDX + 2] += result[j].g;
                            displacements[1].Data[IDX + 0] += result[j].b;
                            displacements[1].Data[IDX + 2] += result[j].a;
                        }
                        else if (index == 2)
                        {
                            displacements[2].Data[IDX + 0] += result[j].r;
                            displacements[2].Data[IDX + 2] += result[j].g;
                            displacements[3].Data[IDX + 0] += result[j].b;
                            displacements[3].Data[IDX + 2] += result[j].a;
                        }
                    }
                    else
                    {
                        result[j].r = 0.0f;
                        result[j].g = 0.0f;
                        result[j].b = 0.0f;
                        result[j].a = 0.0f;
                    }
		
                }

            }

        }

        public Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale)
		{

			InterpolatedArray2f[] displacements = GetReadDisplacements();

			return QueryDisplacements.MaxRange(displacements, choppyness, gridScale, null);

		}

		public void QueryWaves(WaveQuery query, QueryGridScaling scaling)
		{

			int enabled = EnabledBuffers();

			//If no buffers are enabled there is nothing to sample.
			if(enabled == 0) return;

			InterpolatedArray2f[] displacements = GetReadDisplacements();
			
			QueryDisplacements.QueryWaves(query, enabled, displacements, scaling);
			
		}

	}

}











