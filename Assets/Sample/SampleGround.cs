using UnityEngine;
using System.Collections;

public class SampleGround : MonoBehaviour {

    private RuntimeLightBake.RuntimeLightBake bakeSystem;

    public Light[] r_lights;
    public Light[] g_lights;
    public Light[] b_lights;

	// Use this for initialization
	void Awake () {
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
        Material material = renderer.material;
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();

        bakeSystem = new RuntimeLightBake.RuntimeLightBake(512, 512);
        bakeSystem.ClearRenderTarget();

        SetLightParameterList(meshFilter, 0, r_lights);
        SetLightParameterList(meshFilter, 1, g_lights);
        SetLightParameterList(meshFilter, 2, b_lights);

        material.shader = Shader.Find("Unlit/Texture");
        material.mainTexture = bakeSystem.GetRenderTargetTexture();
	}


    private void SetLightParameterList(MeshFilter meshFilter,int idx, Light[] lights)
    {

        for (int i = 0; i < lights.Length; ++i)
        {
            bakeSystem.ResetLightParameter();
            bakeSystem.SetLightParameter(idx, lights[i].transform.position, lights[i].range, lights[i].intensity);
            bakeSystem.RenderingToTarget(meshFilter, RuntimeLightBake.RuntimeLightBake.EUvSelect.Uv1);
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        bakeSystem.Release();
    }
}
