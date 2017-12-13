using UnityEngine;
using System.Collections;

namespace Ceto
{

	public class ImageBlur 
	{

		public enum BLUR_MODE { OFF = 0, NO_DOWNSAMPLE = 1, DOWNSAMPLE_2 = 2, DOWNSAMPLE_4 = 4 };

		public BLUR_MODE BlurMode { get; set; }
		
		/// Blur iterations - larger number means more blur.
		public int BlurIterations { get; set; }
		
		/// Blur spread for each iteration. Lower values
		/// give better looking blur, but require more iterations to
		/// get large blurs. Value is usually between 0.5 and 1.0.
		public float BlurSpread { get; set; }


		public Material m_blurMaterial;

		public ImageBlur(Shader blurShader)
		{

			BlurIterations = 1;
			BlurSpread = 0.6f;
			BlurMode = BLUR_MODE.DOWNSAMPLE_2;

			if(blurShader != null)
				m_blurMaterial = new Material(blurShader);

		}

		public void Blur(RenderTexture source)
		{

			int blurDownSample = (int)BlurMode;

			if(BlurIterations > 0 && m_blurMaterial != null && blurDownSample > 0)
			{
				int rtW = source.width / blurDownSample;
				int rtH = source.height / blurDownSample;

				RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0, source.format, RenderTextureReadWrite.Default);
	
				// Copy source to the 4x4 smaller texture.
				DownSample4x(source, buffer);
				
				// Blur the small texture
				for (int i = 0; i < BlurIterations; i++)
				{
					RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format, RenderTextureReadWrite.Default);
					FourTapCone(buffer, buffer2, i);
					RenderTexture.ReleaseTemporary(buffer);
					buffer = buffer2;
				}
				
				Graphics.Blit(buffer, source);
				RenderTexture.ReleaseTemporary(buffer);
			}

		}

		// Performs one blur iteration.
		void FourTapCone (RenderTexture source, RenderTexture dest, int iteration)
		{
			float off = 0.5f + iteration*BlurSpread;
			Graphics.BlitMultiTap (source, dest, m_blurMaterial,
			                       new Vector2(-off, -off),
			                       new Vector2(-off,  off),
			                       new Vector2( off,  off),
			                       new Vector2( off, -off)
			                       );
		}
		
		// Downsamples the texture to a quarter resolution.
		void DownSample4x (RenderTexture source, RenderTexture dest)
		{
			float off = 1.0f;
			Graphics.BlitMultiTap (source, dest, m_blurMaterial,
			                       new Vector2(-off, -off),
			                       new Vector2(-off,  off),
			                       new Vector2( off,  off),
			                       new Vector2( off, -off)
			                       );
		}

	}

}
