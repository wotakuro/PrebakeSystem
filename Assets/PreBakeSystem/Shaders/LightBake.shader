Shader "Unlit/LightBake"
{
	Properties
	{
		_LightA("LightA", Vector) = (0.0,0.0,0.0,0.0)
		_LightB("LightB", Vector) = (0.0,0.0,0.0,0.0)
		_LightParam("LightParam", Vector) = (0.0,0.0,0.0,0.0)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
        ZWrite Off
		Cull Off
		ZTest Always
		LOD 100

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
	            float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
	            fixed4 color : COLOR;
			};

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
				float3 normal   = UnityObjectToWorldNormal( v.normal );
				float4 position = mul (_Object2World, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.vertex = float4( (v.texcoord.x -0.5 )  , (-v.texcoord.y +0.5)  , 0.0 , 0.5 );

				o.color.x = calculateLightEffect( _LightA.xyz , position.xyz , normal, _LightA.w , 1.0 );
				o.color.y = 0.0;
				o.color.z = 0.0;
				o.color.w = 1.0;
				
				// o.color.xy = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 addCol = i.color;
				return addCol;
			}
			ENDCG
		}
	}
}
