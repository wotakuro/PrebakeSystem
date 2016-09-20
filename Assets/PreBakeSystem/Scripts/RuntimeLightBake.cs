using UnityEngine;
using System.Collections;

namespace RuntimeLightBake
{
    /// <summary>
    /// Runtimeでライトをベイクするための仕組み
    /// </summary>
    public class RuntimeLightBake 
    {
        public enum EUvSelect
        {
            Unknown,
            Uv1,
            Uv2,
            Uv3,
            Uv4,
        }
        private readonly string[] ShaderKeyWords = new string[] { "WRITE_TO_UV1", "WRITE_TO_UV2", "WRITE_TO_UV3", "WRITE_TO_UV4" };

        private RenderTexture renderTarget;
        private Material drawMaterial;

        /// <summary>
        /// "_LightA" Shader Id Cache
        /// </summary>
        private int[] shaderIdLightPos = new int[3];

        /// <summary>
        /// "_LightParam" Shader Id Cache
        /// </summary>
        private int shaderIdLightParam;

        private Vector4[] lightPositions = new Vector4[3];
        private Vector4 lightParameter;

        /// <summary>
        /// constructor
        /// </summary>
        public RuntimeLightBake()
        {
            // Shader Id Get
            this.shaderIdLightPos[0] = Shader.PropertyToID("_LightA");
            this.shaderIdLightPos[1] = Shader.PropertyToID("_LightB");
            this.shaderIdLightPos[2] = Shader.PropertyToID("_LightC");
            this.shaderIdLightParam = Shader.PropertyToID("_LightParam");

            // RenderingTexutre Create
            this.renderTarget = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
            this.renderTarget.Create();
        }

        public RenderTexture GetRenderTargetTexture()
        {
            return this.renderTarget;
        }

        public void RenderingToTarget(Mesh mesh,EUvSelect uvSelect, Vector3 pos, Quaternion rot, Vector3 size)
        {
            Matrix4x4 matrix = Matrix4x4.TRS(pos, rot, size );
            this.RenderingToTarget(mesh, uvSelect, ref matrix);
        }

        /// <summary>
        /// Rendering light 
        /// </summary>
        /// <param name="meshFilter"> set MeshFilter </param>
        /// <param name="uvSelect">use uv</param>
        public void RenderingToTarget(MeshFilter meshFilter, EUvSelect uvSelect) {
            Mesh mesh = meshFilter.sharedMesh;
            var trans = meshFilter.transform;
            Matrix4x4 matrix = Matrix4x4.TRS(trans.position, trans.rotation, trans.lossyScale );
            this.RenderingToTarget(mesh, uvSelect, ref matrix);
        }

        public void RenderingToTarget(Mesh mesh, EUvSelect uvSelect, ref Matrix4x4 matrix)
        {
            this.SetShaderKeyWordToMaterial(uvSelect);
            Graphics.SetRenderTarget(renderTarget);
            this.drawMaterial.SetPass(0);
            Graphics.DrawMeshNow(mesh, matrix);
            Graphics.SetRenderTarget(null);
        }

        /// <summary>
        /// LightParameter Set
        /// </summary>
        /// <param name="lightIndex"></param>
        /// <param name="position">light position</param>
        /// <param name="range">light range</param>
        /// <param name="intensity">light intensity</param>
        public void SetLightParameter(int lightIndex,Vector3 position , float range,float intensity )
        {
            this.lightPositions[lightIndex] = new Vector4(position.x, position.y, position.z, range);
            switch (lightIndex)
            {
                case 0:
                    this.lightParameter.x = intensity;
                    break;
                case 1:
                    this.lightParameter.x = intensity;
                    break;
                case 2:
                    this.lightParameter.x = intensity;
                    break;
            }
        }

        /// <summary>
        /// Reset Light Paramter
        /// </summary>
        public void ResetLightParameter()
        {
            int length = this.lightPositions.Length;
            for (int i = 0; i < length; ++i) {
                this.lightPositions[i] = Vector4.zero;
            }
            this.lightParameter = Vector4.zero;
        }

        /// <summary>
        /// Set Light Parameter To Material
        /// </summary>
        private void SetLightParameterToMaterial()
        {
            int length = shaderIdLightPos.Length;
            for (int i = 0; i < length; ++i)
            {
                drawMaterial.SetVector(shaderIdLightPos[i],this.lightPositions[i]);
            }
            drawMaterial.SetVector(this.shaderIdLightParam, this.lightParameter);
        }

        /// <summary>
        /// Set Shader Keyword to material
        /// </summary>
        /// <param name="uvSelect">use uv</param>
        private void SetShaderKeyWordToMaterial(EUvSelect uvSelect) {
            int param = -1;
            switch (uvSelect)
            {
                case EUvSelect.Uv1:
                    param = 0;
                    break;
                case EUvSelect.Uv2:
                    param = 1;
                    break;
                case EUvSelect.Uv3:
                    param = 2;
                    break;
                case EUvSelect.Uv4:
                    param = 3;
                    break;
            }
            int length = ShaderKeyWords.Length;
            for (int i = 0; i < length; ++i)
            {
                if (i == param)
                {
                    this.drawMaterial.EnableKeyword(ShaderKeyWords[i]);
                }
                else
                {
                    this.drawMaterial.DisableKeyword(ShaderKeyWords[i]);
                }
                
            }
        }

    }
}
