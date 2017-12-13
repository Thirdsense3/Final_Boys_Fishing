using UnityEngine;
using System.Collections.Generic;


namespace Ceto
{

    /// <summary>
    /// Adds a jet of foam in the direction game object is facing.
    /// </summary>
	[AddComponentMenu("Ceto/Overlays/AddFoamThruster")]
    public class AddFoamThruster : AddWaveOverlayBase
    {

        /// <summary>
        /// Rotation mode to apply to the overlays.
        /// NONE - no rotation.
        /// RANDOM - a random rotation 
        /// RELATIVE - The rotation will match the parents 
        /// current y axis rotation when created.
        /// </summary>
        public enum ROTATION { NONE, RANDOM, RELATIVE };

        /// <summary>
        /// The texture to use for the foam.
        /// </summary>
        public Texture foamTexture;

        /// <summary>
        /// Should the global foam texture be applied to
        /// the foam overlays.
        /// </summary>
        public bool textureFoam = true;

        /// <summary>
        /// Rotation mode.
        /// </summary>
        public ROTATION rotation = ROTATION.RANDOM;

        /// <summary>
        /// The curve controls that overlays alpha over
        /// its life time. Allows the overlays to fade in 
        /// when created and then fade out over time.
        /// </summary>
        public AnimationCurve timeLine = DefaultCurve();

        /// <summary>
        /// How long in seconds the overlay will last.
        /// </summary>
        public float duration = 4.0f;

        /// <summary>
        /// The size of the overlay when created.
        /// </summary>
        public float size = 2.0f;

        /// <summary>
        /// Time in milliseconds a new overlay is created.
        /// </summary>
        [Range(16.0f, 1000.0f)]
        public float rate = 128.0f;

        /// <summary>
        /// The amount the overlay will expand over time.
        /// </summary>
        public float expansion = 4.0f;

        /// <summary>
        /// The amount the overlay will move over time.
        /// </summary>
        public float momentum = 10.0f;

        /// <summary>
        /// The amount the overlay will rotate over time.
        /// </summary>
        public float spin = 10.0f;

        /// <summary>
        /// If true the overlays will not be created when
        /// this objects position is above the water level.
        /// </summary>
        public bool mustBeBelowWater = true;

        /// <summary>
        /// The overlays alpha.
        /// </summary>
        [Range(0.0f, 2.0f)]
        public float alpha = 0.8f;

        /// <summary>
        /// Randomizes the spin and expansion values
        /// for each overlay created.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float jitter = 0.2f;

        /// <summary>
        /// 
        /// </summary>
        float m_lastTime;

        /// <summary>
        /// 
        /// </summary>
        float m_remainingTime;

        /// <summary>
        /// 
        /// </summary>
        protected override void Start()
        {

            m_lastTime = Time.time;

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Update()
        {

            UpdateOverlays();
            AddFoam();
            RemoveOverlays();

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            m_lastTime = Time.time;
        }

		/// <summary>
		/// Call to translate the overlays by this amount
		/// </summary>
		public override void Translate(Vector3 amount)
		{
			
			base.Translate(amount);

		}

        /// <summary>
        /// Returns a rotation value depending on the rotation mode.
        /// </summary>
        float Rotation()
        {

            switch (rotation)
            {
                case ROTATION.NONE:
                    return 0.0f;

                case ROTATION.RANDOM:
                    return Random.Range(0.0f, 360.0f);

                case ROTATION.RELATIVE:
                    return transform.eulerAngles.y;
            }

            return 0.0f;

        }

        /// <summary>
        /// Creates new overlays based on the movement from the last position.
        /// </summary>
        void AddFoam()
        {

            //If there is no ocean in scene or if the overlays 
            //duration is less than 0 dont do anything
            if (duration <= 0.0f || Ocean.Instance == null)
            {
                m_lastTime = Time.time;
                return;
            }

            //Clamp size.
            size = Mathf.Max(1.0f, size);

            float h = transform.position.y;
            Vector3 pos = transform.position;
            Vector3 dir = transform.forward;

            //If the overlays are only to be added if the position
            //is below the water line get the wave height.
            if (mustBeBelowWater)
                h = Ocean.Instance.QueryWaves(pos.x, pos.z);
	
            //If the waves are below the position do nothing.
            //If the forward dir is straight up or down do nothing.
            if (h < pos.y || (dir.x == 0.0f && dir.z == 0.0f))
            {
                m_lastTime = Time.time;
                return;
            }

			float delta = Time.time - m_lastTime;
            dir = dir.normalized;
            pos.y = 0.0f;

            Vector3 momentumDir = dir * momentum;

            m_remainingTime += delta;

			//rate in seconds
			float r = rate / 1000.0f;

            float d = 0.0f;
            while (m_remainingTime > r)
            {
                //Find the next overlay pos.
                Vector3 overlayPos = pos + dir * d;

                //Create a new overlay and set is starting values.
                FoamOverlay overlay = new FoamOverlay(overlayPos, Rotation(), size, duration, foamTexture);

                overlay.FoamTex.alpha = 0.0f;
                overlay.FoamTex.textureFoam = textureFoam;
                overlay.Momentum = momentumDir;
                overlay.Spin = (Random.value > 0.5f) ? -spin : spin;
                overlay.Expansion = expansion;

                if (jitter > 0.0f)
                {
                    overlay.Spin *= 1.0f + Random.Range(-1.0f, 1.0f) * jitter;
                    overlay.Expansion *= 1.0f + Random.Range(-1.0f, 1.0f) * jitter;
                }

                //Add to list and add to ocean overlay manager.
                //The overlay manager will render the overlay into the buffer.
                m_overlays.Add(overlay);
                Ocean.Instance.OverlayManager.Add(overlay);

                //Decrement remaining distance by the rate.
                m_remainingTime -= r;
                d += r;
            }

            m_lastTime = Time.time;

        }

        /// <summary>
        /// Need to update the overlays each frame.
        /// </summary>
        void UpdateOverlays()
        {

            var e = m_overlays.GetEnumerator();
            while(e.MoveNext())
            {
                //Set the alpha based on the its age and curve
                float a = e.Current.NormalizedAge;
                e.Current.FoamTex.alpha = timeLine.Evaluate(a) * alpha;
                e.Current.FoamTex.textureFoam = textureFoam;
                //Updates the overlays position, rotation, expansion and its bounding box.
                e.Current.UpdateOverlay();
            }

        }

        /// <summary>
        /// Remove any overlays that have a age longer that there duration.
        /// </summary>
        void RemoveOverlays()
        {

            LinkedList<WaveOverlay> remove = new LinkedList<WaveOverlay>();

            var e1 = m_overlays.GetEnumerator();
            while (e1.MoveNext())
            {

                WaveOverlay overlay = e1.Current;

                if (overlay.Age >= overlay.Duration)
                {
                    remove.AddLast(overlay);
                    //Set kill to true to remove from oceans overlay manager.
                    overlay.Kill = true;
                }
            }

            var e2 = remove.GetEnumerator();
            while (e2.MoveNext())
            {
                m_overlays.Remove(e2.Current);
            }

        }

        void OnDrawGizmos()
        {
            if (!enabled) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 2.0f);
        }

    }

}







