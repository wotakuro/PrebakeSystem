using UnityEngine;
using UnityEditor;
using System.Collections;

public class PolygonCreate  {

    [MenuItem("Tools/PolygonCreate")]
    public static void Create()
    {
        Mesh mesh = new Mesh();


        Rect rect = new Rect(-1.0f, -1.0f, 1.0f, 1.0f);

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
    		0, 1, 2,
	    	0, 2, 3,
	    };

        Vector3[] vertices = new Vector3[vertOrigin.Length * 4];
        Vector2[] uv1 = new Vector2[uvOrigin.Length * 4];
        Vector2[] uv2 = new Vector2[uv1.Length];

        mesh.vertices = vertices;
        mesh.uv = uv1;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        AssetDatabase.CreateAsset(mesh, "Assets/mesh.asset");
    }
}
