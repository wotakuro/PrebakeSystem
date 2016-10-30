using UnityEngine;
using System.Collections;

public class SampleGround : MonoBehaviour {

    private RuntimeLightBake.RuntimeLightBake bakeSystem;

    public Light[] lights;

	// Use this for initialization
	void Awake () {
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();
        Material material = renderer.material;
        MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();

        bakeSystem = new RuntimeLightBake.RuntimeLightBake(512, 512);
        bakeSystem.ClearRenderTarget();

        for (int i = 0; i < lights.Length; ++i)
        {
            bakeSystem.SetLightParameter(0, lights[i].transform.position, lights[i].range, lights[i].intensity);
            bakeSystem.RenderingToTarget(meshFilter, RuntimeLightBake.RuntimeLightBake.EUvSelect.Uv1);
        }
        material.shader = Shader.Find("Unlit/Texture");
        material.mainTexture = bakeSystem.GetRenderTargetTexture();
	}


    // Update is called once per frame
    void OnDestroy()
    {
        bakeSystem.Release();
    }
}
