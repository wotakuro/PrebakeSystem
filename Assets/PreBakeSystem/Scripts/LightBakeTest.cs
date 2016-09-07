using UnityEngine;
using System.Collections;

public class LightBakeTest : MonoBehaviour {
    RenderTexture target;

    public Light light;
    public Material bakeMaterial;
    public Mesh mesh;
    public Transform positionInfo;

    public MeshRenderer changeTextureTarget;
    public SkinnedMeshRenderer targetSkinned;

    void Awake()
    {
        target = new RenderTexture(128, 128, 0 ,RenderTextureFormat.ARGB32);
        target.Create();
    }

    void Start()
    {
        var mat = new Material(Shader.Find("Unlit/Texture"));
        mat.mainTexture = target;
        changeTextureTarget.material = mat;

        if (targetSkinned != null && targetSkinned.material != null)
        {
            var mat2 = new Material(Shader.Find("Unlit/Texture"));
            mat2.mainTexture = target;
            targetSkinned.material = mat2;
        }
    }

    void Update()
    {
        Vector4 lightAParam = new Vector4(light.transform.position.x, light.transform.position.y, light.transform.position.z, light.range);

        Graphics.SetRenderTarget(target);
        GL.Clear(true, true, new Color(0,0,0,0) );
        bakeMaterial.SetPass(0);
        bakeMaterial.SetVector("_LightA", lightAParam);
        Graphics.DrawMeshNow(mesh, positionInfo.position, positionInfo.rotation);
        Graphics.SetRenderTarget(null);
    }
    void OnDestroy()
    {
        target.Release();
    }
}
