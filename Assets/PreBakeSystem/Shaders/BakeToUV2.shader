Shader "LightBake/BakeToUV2"
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
				float2 writeToUv : TEXCOORD1;
	            float3 normal : NORMAL;
	            fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 position : TEXCOORD0; // world座標入れ先
	            float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
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
				float4 nvec   = mul (_Object2World, float4(v.normal,0.0) );
				float4 position = mul (_Object2World, v.vertex);
				
				// UV2に書き込みます
				o.vertex = float4( (v.writeToUv.x -0.5 )  , (-v.writeToUv.y +0.5)  , 0.0 , 0.5 );
				o.position = position;

	            o.normal = nvec.xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 addCol = float4(0,0,0,0);
				
				addCol.r = calculateLightEffect( _LightA.xyz , i.position.xyz , i.normal, _LightA.w , _LightParam.x );
				addCol.g = 0.0;
				addCol.b = 0.0;
				addCol.a = 1.0;

				//addCol = i.color;
				return addCol;
			}
			ENDCG
		}
	}
}
