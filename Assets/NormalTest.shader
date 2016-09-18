Shader "Unlit/NormalTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightA("LightA", Vector) = (0.0,0.0,0.0,0.0)
		_LightB("LightB", Vector) = (0.0,0.0,0.0,0.0)
		_LightParam("LightParam", Vector) = (0.0,0.0,0.0,0.0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Blend One One // Additive

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
	            float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 position : TEXCOORD0;
				float4 vertex : SV_POSITION;
	            fixed4 color : COLOR;
	            float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _LightA;
			float4 _LightB;
			float4 _LightParam;

			
			fixed calculateLightEffect(float3 lightPos,float3 vert,float3 normal,float lightDist,float lightIdentity){
				float3 lightVec =  lightPos - vert;
				half len = length( lightVec );
				half param = saturate((lightDist - len) / lightDist );
				half normalParam = dot( normalize( lightVec ) , normalize( normal) ) * 0.5 ;
				return saturate( normalParam * param );
			}

			
			v2f vert (appdata v)
			{
				v2f o;
				float4 nvec = mul (_Object2World, float4(v.normal,0.0) );
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.position = mul (_Object2World, v.vertex);

				o.normal = nvec.xyz;
				o.color.rgb = normalize( o.normal * 0.5 + 0.5);
				o.color.a = 1.0;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = fixed4(0,0,0,0);
				col.rgb = i.normal;

				
				col.r = calculateLightEffect( _LightA.xyz , i.position.xyz , i.normal, _LightA.w , 1.0 );
				col.g  = 0.0;
				col.b = 0.0;
				col.a = 1.0;

				col = i.color;
				return col;
			}
			ENDCG
		}
	}
}
