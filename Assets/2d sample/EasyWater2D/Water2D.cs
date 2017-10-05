using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EasyWater2D
{


#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    public class Water2D : MonoBehaviour
    {

#if UNITY_EDITOR
        public bool drawDebugShapes;
#endif

        public Vector2 size;

        [Range(1, 100)]
        public int segments;

        [Range(0, 100)]
        public int softness;

        [Range(0, 30f)]
        public float speedMultiplier, accelerationMultiplier;

        [Range(0, 10f)]
        public float waveHeight;

        [Range(0, 0.999f)]
        public float randomness;


        [HideInInspector]
        [SerializeField]
        int vertexCount, softVertexCount, softFacesPerSegment, softSegments;


        [HideInInspector]
        [SerializeField]
        float lowerLimit, upperLimit;


        [HideInInspector]
        [SerializeField]
        Vector3[] vertices;

        [HideInInspector]
        [SerializeField]
        List<VertexTransform> vertexTransforms;

        VertexTransform v1, v2;

        Mesh mesh;

        void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;

        }



        void Update()
        {
            for (int i = 0; i < vertexCount; i++)
            {
                v1 = vertexTransforms[i];

                IntegrateVertex(v1);

                vertices[i * softFacesPerSegment * 2].y = v1.position;

            }

            for (int i = 0; i < softVertexCount; i++)
            {
                if (i % softFacesPerSegment == 0)
                {

                }
                else
                {
                    vertices[i * 2].y = GetCurvedValue(vertexTransforms[i / softFacesPerSegment].position, vertexTransforms[i / softFacesPerSegment + 1].position, (i % softFacesPerSegment) / (float)softFacesPerSegment);
                }
            }

            mesh.vertices = vertices;

        }



#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (drawDebugShapes)
            {
                Gizmos.color = Color.cyan;

                if (size.x < 0)
                    size.x = 0;

                if (size.y < 0)
                    size.y = 0;

                Vector2 ul = new Vector2(-size.x / 2, size.y / 2) + (Vector2)transform.position,
                        ur = new Vector2(size.x / 2, size.y / 2) + (Vector2)transform.position,
                        ll = new Vector2(-size.x / 2, -size.y / 2) + (Vector2)transform.position,
                        lr = new Vector2(+size.x / 2, -size.y / 2) + (Vector2)transform.position;


                Gizmos.DrawLine(ul, ur);
                Gizmos.DrawLine(ur, lr);
                Gizmos.DrawLine(lr, ll);
                Gizmos.DrawLine(ll, ul);

                Gizmos.color = Color.magenta;

                ul.y = transform.position.y + size.y / 2 + upperLimit * waveHeight; ur.y = ul.y;

                ll.y = transform.position.y + size.y / 2 + lowerLimit * waveHeight; lr.y = ll.y;

                Gizmos.DrawLine(ul, ur);
                Gizmos.DrawLine(ll, lr);

            }
        }
#endif

        void IntegrateVertex(VertexTransform v)
        {

            if (v.position < size.y / 2 + lowerLimit * waveHeight && v.velocity < 0)
            {
                v.acceleration = Random.Range(1 - randomness, 1 + randomness);
            }
            else if (v.position > size.y / 2 + upperLimit * waveHeight && v.velocity > 0)
            {
                v.acceleration = -Random.Range(1 - randomness, 1 + randomness);
            }

            if ((v.acceleration < 0 && v.velocity > -speedMultiplier) || (v.acceleration > 0 && v.velocity < speedMultiplier))
            {
                v.velocity += v.acceleration * Time.deltaTime * accelerationMultiplier;
            }



            v.position += v.velocity * Time.deltaTime * speedMultiplier;
        }


        float GetCurvedValue(float first, float second, float alpha)
        {
            alpha *= 2;

            if (alpha > 1)
            {
                alpha = 1 - (2 - alpha) * (2 - alpha) * 0.5f;
            }
            else
            {
                alpha = alpha * alpha * 0.5f;
            }

            return Mathf.Lerp(first, second, alpha);
        }



        public void Setup()
        {
            MeshFilter filter = GetComponent<MeshFilter>();

            if (filter == null)
            {
                Debug.LogError("Water 2D - No MeshFilter component attached to the water object.");
                return;
            }

            softFacesPerSegment = softness + 1;

            softSegments = segments * softFacesPerSegment;

            lowerLimit = -1;
            upperLimit = 1;

            vertexTransforms = new List<VertexTransform>();

            vertexCount = segments + 1;
            softVertexCount = softSegments + 1;

            Mesh mesh = new Mesh();

            vertices = new Vector3[softVertexCount * 2];
            Vector3[] normals = new Vector3[softVertexCount * 2];
            Vector2[] uv = new Vector2[softVertexCount * 2];
            int[] triangles = new int[softSegments * 6];


            float currentX = -size.x / 2,
                  y = size.y / 2,
                  softSegmentLength = size.x / softSegments;

            for (int i = 0; i < vertexCount; i++)
            {
                vertexTransforms.Add(new VertexTransform(size.y, randomness));
            }

            for (int i = 0; i < softVertexCount; i++)
            {
                vertices[i * 2] = new Vector3(currentX, y, 0);
                vertices[i * 2 + 1] = new Vector3(currentX, -y, 0);

                normals[i * 2] = Vector3.back;
                normals[i * 2 + 1] = Vector3.back;

                uv[i * 2] = Vector2.right * ((float)i / (softVertexCount - 1)) + Vector2.up;
                uv[i * 2 + 1] = Vector2.right * ((float)i / (softVertexCount - 1));

                currentX += softSegmentLength;
            }


            for (int i = 0; i < softSegments; i++)
            {
                triangles[i * 6] = i * 2;
                triangles[i * 6 + 1] = i * 2 + 3;
                triangles[i * 6 + 2] = i * 2 + 1;
                triangles[i * 6 + 3] = i * 2;
                triangles[i * 6 + 4] = i * 2 + 2;
                triangles[i * 6 + 5] = i * 2 + 3;
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.triangles = triangles;

            this.mesh = mesh;

            if (Application.isPlaying)
                filter.mesh = mesh;
            else
                filter.sharedMesh = mesh;

        }


    }



    [System.Serializable]
    public class VertexTransform
    {
        public float position, velocity, acceleration;

        public VertexTransform(float y, float randomness)
        {
            position = (y / 2);
            velocity = (Random.Range(0, 2) == 0 ? (Random.Range(-1 - randomness, -1 + randomness)) : (Random.Range(1 - randomness, 1 + randomness))) * 0.6f;
            acceleration = (Random.Range(0, 2) % 2 == 0 ? (Random.Range(-1 - randomness, -1 + randomness)) : (Random.Range(1 - randomness, 1 + randomness)));
        }

    }



#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Water2D))]
    public class Water2DEditor : Editor
    {

        Water2D w;

        void OnEnable()
        {
            w = (Water2D)target;
            w.Setup();
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            GUILayout.Space(5);

            if (GUI.changed)
            {
                w.Setup();
            }
        }


    }

#endif

}