using UnityEngine;
using System.Collections;

public class NormalTest : MonoBehaviour {

    public Light light;
    private Material mat;
	// Use this for initialization
	void Start () {
        var renderer = this.gameObject.GetComponentInChildren<Renderer>();
        mat = renderer.material;
	}
	
	// Update is called once per frame
	void Update () {

        Vector4 lightAParam = new Vector4(light.transform.position.x, light.transform.position.y, light.transform.position.z, light.range);
        mat.SetVector("_LightA", lightAParam);
	}
}
