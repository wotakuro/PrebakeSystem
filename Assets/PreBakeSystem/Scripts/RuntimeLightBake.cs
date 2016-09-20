using UnityEngine;
using System.Collections;

namespace RuntimeLightBake
{
    /// <summary>
    /// Runtimeでライトをベイクするための仕組み
    /// </summary>
    public class RuntimeLightBake 
    {
        /// <summary>
        /// use uv
        /// </summary>
        public enum EUvSelect
        {
            Unknown,
            Uv1,
            Uv2,
            Uv3,
            Uv4,
        }
        /// <summary>
        /// Shader keywords
        /// </summary>
        private readonly string[] ShaderKeyWords = new string[] { "WRITE_TO_UV1", "WRITE_TO_UV2", "WRITE_TO_UV3", "WRITE_TO_UV4" };

        /// <summary>
        /// rendering target
        /// </summary>
        private RenderTexture renderTarget;

        /// <summary>
        /// material to render light texture
        /// </summary>
        private Material drawMaterial;

        /// <summary>
        /// "_LightA" Shader Id Cache
        /// </summary>
        private int[] shaderIdLightPos = new int[3];

        /// <summary>
        /// "_LightParam" Shader Id Cache
        /// </summary>
        private int shaderIdLightParam;

        /// <summary>
        /// light positions
        /// </summary>
        private Vector4[] lightPositions = new Vector4[3];
        /// <summary>
        /// light parameters
        /// </summary>
        private Vector4 lightParameter;

        /// <summary>
        /// constructor
        /// </summary>
        public RuntimeLightBake(int width , int height)
        {
            // Shader Id Get
            this.shaderIdLightPos[0] = Shader.PropertyToID("_Light0");
            this.shaderIdLightPos[1] = Shader.PropertyToID("_Light1");
            this.shaderIdLightPos[2] = Shader.PropertyToID("_Light2");
            this.shaderIdLightParam = Shader.PropertyToID("_LightParam");

            // RenderingTexutre Create
            this.renderTarget = new RenderTexture(width,height, 0, RenderTextureFormat.ARGB32);
            this.renderTarget.Create();
        }

        /// <summary>
        /// release render Texture
        /// </summary>
        public void Release()
        {
            if (this.renderTarget != null)
            {
                this.renderTarget.Release();
            }
            this.renderTarget = null;
        }

        /// <summary>
        /// return Render target texture
        /// </summary>
        /// <returns>return render targetTexture</returns>
        public RenderTexture GetRenderTargetTexture()
        {
            return this.renderTarget;
        }

        /// <summary>
        /// Clear Rendering Target
        /// </summary>
        public void ClearRenderTarget()
        {
            Graphics.SetRenderTarget(this.renderTarget);
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.SetRenderTarget(null);
        }

        /// <summary>
        /// Rendering light
        /// </summary>
        /// <param name="mesh">mesh</param>
        /// <param name="uvSelect">use uv</param>
        /// <param name="pos">position</param>
        /// <param name="rot">rotation</param>
        /// <param name="size">size</param>
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

        /// <summary>
        /// Rendering light
        /// </summary>
        /// <param name="mesh">set mesh</param>
        /// <param name="uvSelect">use uv</param>
        /// <param name="matrix">drawing matrix</param>
        public void RenderingToTarget(Mesh mesh, EUvSelect uvSelect, ref Matrix4x4 matrix)
        {
            this.SetShaderKeyWordToMaterial(uvSelect);
            this.SetLightParameterToMaterial();
            // rendering
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
