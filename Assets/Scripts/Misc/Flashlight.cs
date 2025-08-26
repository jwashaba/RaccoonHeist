using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    
    [SerializeField] private int rayCount = 2;

    [SerializeField] public float angle = 90f;
    [SerializeField] public float fov = 90f;
    
    private MeshFilter _mf;
    private Mesh _mesh;
    private Rigidbody2D _rb;
    private PolygonCollider2D _col;
    
    void Start()
    {
        _mesh = new Mesh();
        _rb = GetComponentInParent<Rigidbody2D>();
        _col = GetComponentInParent<PolygonCollider2D>();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    void Update()
    {
        Vector3 origin = _rb.position;
        float localAngle = angle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 3f;
        
        Vector3[] vertices = new Vector3[rayCount + 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 angleVector = GetVectorFromAngle(localAngle);
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, angleVector, viewDistance, layerMask);

            if (raycastHit2D.collider == null)
            {
                vertex = angleVector * viewDistance;
            }
            else
            {
                vertex = new Vector3(raycastHit2D.point.x, raycastHit2D.point.y, 0) - origin;
            }
            
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
            
                triangleIndex += 3;
            }

            vertexIndex++;
            localAngle -= angleIncrease;
        }
        
        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;
    }

    // get unit vector using angle in degrees
    public static Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Cos(angle * (Mathf.PI/180f)), Mathf.Sin(angle * (Mathf.PI/180f)));
    }
}
