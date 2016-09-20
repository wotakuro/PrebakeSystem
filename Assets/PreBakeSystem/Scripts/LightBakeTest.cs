using UnityEngine;
using System.Collections;

public class LightBakeTest : MonoBehaviour {
    RenderTexture target;

    public TextMesh text;
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
        Graphics.SetRenderTarget(target.colorBuffer, target.depthBuffer);
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        Graphics.SetRenderTarget(null);
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

            var materials = new Material[targetSkinned.materials.Length];
            for (int i = 0; i < targetSkinned.materials.Length;++i )
            {
                materials[i] = mat2;
            }
            targetSkinned.materials = materials;
        }

        if (mesh.isReadable)
        {
            text.text = "Uv1:" + (mesh.uv.Length) + " Uv2:" + (mesh.uv2.Length) + "\nuv3:" + (mesh.uv3.Length) +
                " Uv4:" + (mesh.uv4.Length);
        }
    }


    void OnPostRender()
    {
        Vector4 lightAParam = new Vector4(light.transform.position.x, light.transform.position.y, light.transform.position.z, light.range);
        Vector4 lightIntensity = new Vector4(light.intensity, 0.0f, 0.0f, light.intensity);


        bakeMaterial.DisableKeyword("WRITE_TO_UV1");
        bakeMaterial.EnableKeyword("WRITE_TO_UV2");

        Graphics.SetRenderTarget(target.colorBuffer, target.depthBuffer);
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        bakeMaterial.SetVector("_Light0", lightAParam);
        bakeMaterial.SetVector("_LightParam", lightIntensity);
        bakeMaterial.SetVector("_Light3", lightAParam);
        bakeMaterial.SetPass(0);
        Matrix4x4 matrix = Matrix4x4.TRS(positionInfo.position, positionInfo.rotation, positionInfo.lossyScale);
        Graphics.DrawMeshNow(mesh, matrix);
        Graphics.SetRenderTarget(null);
    }


    void OnDestroy()
    {
        target.Release();
    }
}
