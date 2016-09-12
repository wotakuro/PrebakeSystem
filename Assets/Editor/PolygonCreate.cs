using UnityEngine;
using UnityEditor;
using System.Collections;

public class PolygonCreate  {

    [MenuItem("Tools/PolygonCreate")]
    public static void Create()
    {
        Mesh mesh = new Mesh();


        Rect rect = new Rect(-1.0f, -1.0f, 2.0f, 2.0f);

        Vector3[] vertOrigin = new Vector3[] {
		    new Vector3(rect.xMin, 0,rect.yMin),
		    new Vector3(rect.xMax, 0,rect.yMin),
		    new Vector3(rect.xMax, 0,rect.yMax),
		    new Vector3(rect.xMin, 0,rect.yMax),
	    };
        Vector2[] uvOrigin = new Vector2[]{
		    new Vector2(0, 0),
		    new Vector2(1, 0),
	    	new Vector2(1, 1),
    		new Vector2(0, 1),
        };


        int[] triangleOrigin = new int[] {
    		0, 2, 1,
	    	0, 3, 2,
	    };

        Vector3[] vertices = new Vector3[vertOrigin.Length * 4];
        Vector2[] uv1 = new Vector2[uvOrigin.Length * 4];
        Vector2[] uv2 = new Vector2[uv1.Length];
        int[] triangles = new int[triangleOrigin.Length * 4];


        for(int i = 0 ; i < 4; ++ i ){
            WriteToUv1(i, uv1, uvOrigin);
            WriteToUv2( i , uv2,uvOrigin);
            WriteToTriangle(i, triangles, triangleOrigin);
            WriteToVertex(i, GetVertexmatrix(i), vertices, vertOrigin);
        }

        mesh.vertices = vertices;
        mesh.uv = uv1;
        mesh.uv2 = uv2;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        AssetDatabase.CreateAsset(mesh, "Assets/mesh.asset");
    }

    private static void WriteToUv1(int idx, Vector2[] data, Vector2[] origin)
    {
        int originLength = origin.Length;
        for (int i = 0; i < originLength; ++i)
        {
            data[i + originLength * idx] = origin[i];
        }
    }

    private static void WriteToUv2(int idx, Vector2[] data, Vector2[] uv1Origin)
    {
        int originLength = uv1Origin.Length;
        Vector2 offset = new Vector2( (idx /2 ) * 0.5f , (idx %2) * 0.5f );
        for (int i = 0; i < originLength; ++i)
        {
            data[i + originLength * idx] = offset + uv1Origin[i] * 0.5f;
        }
    }

    private static void WriteToTriangle(int idx, int[] data, int[] origin)
    {
        int originLength = origin.Length;
        for (int i = 0; i < originLength; ++i)
        {
            data[i + originLength * idx] = origin[i] + 4 * idx;
        }
    }

    private static Matrix4x4 GetVertexmatrix(int idx)
    {
        Matrix4x4 mat = Matrix4x4.identity;
        Vector3 size = new Vector3(10.0f, 10.0f, 10.0f);

        switch (idx)
        {
            case 0:
                mat.SetTRS(Vector3.down * size.y, Quaternion.identity, size);
                break;
            case 1:
                mat.SetTRS(Vector3.forward * size.z , Quaternion.Euler(90.0f, 180.0f, 0.0f), size);
                break;
            case 2:
                mat.SetTRS(Vector3.back * size.z, Quaternion.Euler(-90.0f, 180.0f, 0.0f), size);
                break;
            case 3:
                mat.SetTRS(Vector3.left * size.x, Quaternion.Euler(0.0f, 0.0f, -90.0f), size);
                break;
        }
        return mat;
    }

    private static void WriteToVertex(int idx, Matrix4x4 mat, Vector3[] data, Vector3[] origin)
    {
        int originLength = origin.Length;
        for (int i = 0; i < originLength; ++i)
        {
            data[i + originLength * idx] = mat * new Vector4(origin[i].x,origin[i].y,origin[i].z,1.0f);
        }
    }
}
