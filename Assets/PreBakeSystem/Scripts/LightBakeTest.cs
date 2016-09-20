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
        target = new RenderTexture(512, 512, 0 ,RenderTextureFormat.ARGB32);
        target.Create();
    }

    void Start()
    {
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = target;
        changeTextureTarget.material = mat;

        if (targetSkinned != null && targetSkinned.material != null)
        {
            var mat2 = new Material(Shader.Find("Unlit/Uv2Test"));
            mat2.mainTexture = target;
            targetSkinned.material = mat2;
        }
    }


    void OnPostRender()
    {
        Vector4 lightAParam = new Vector4(light.transform.position.x, light.transform.position.y, light.transform.position.z, light.range);
        Vector4 lightIntensity = new Vector4(light.intensity, 0.0f, 0.0f, 0.0f);

        Graphics.SetRenderTarget(target);
        GL.Clear(true, true, new Color(0, 0, 0, 0));
       bakeMaterial.SetVector("_LightA", lightAParam);
       bakeMaterial.SetVector("_LightParam", lightIntensity);
        bakeMaterial.SetPass(0);
        Matrix4x4 matrix = Matrix4x4.TRS(positionInfo.position, positionInfo.rotation, positionInfo.lossyScale);
        Graphics.DrawMeshNow(mesh,matrix);
        Graphics.SetRenderTarget(null);
    }


    void OnDestroy()
    {
        target.Release();
    }
}
