Shader "LightBake/BakePointLightRGB"
{
	Properties
	{
		_Light0("Light 0", Vector) = (0.0,0.0,0.0,0.0)
		_Light1("Light 1", Vector) = (0.0,0.0,0.0,0.0)
		_Light2("Light 2", Vector) = (0.0,0.0,0.0,0.0)
		_Light2("Light 3", Vector) = (0.0,0.0,0.0,0.0)
		_LightParam("LightParam", Vector) = (0.0,0.0,0.0,0.0)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
        ZWrite Off
		Cull Off
		ZTest Always
		Blend One One
		LOD 100


		Pass
		{

			CGPROGRAM
			#pragma shader_feature WRITE_TO_UV1 WRITE_TO_UV2 WRITE_TO_UV3 WRITE_TO_UV4
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;

				#ifdef WRITE_TO_UV1
				float2 writeToUv : TEXCOORD0;
				#elif WRITE_TO_UV2
				float2 writeToUv : TEXCOORD1;
				#elif WRITE_TO_UV3
				float2 writeToUv : TEXCOORD2;
				#elif WRITE_TO_UV4
				float2 writeToUv : TEXCOORD3;
				#else
				float2 writeToUv : TEXCOORD1;
				#endif

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
			float4 _Light0;
			float4 _Light1;
			float4 _Light2;
			float4 _Light3;
			float4 _LightParam;

			fixed calculateLightEffect(float3 lightPos,float3 vert,float3 normal,float lightDist,float lightIdentity){
				float3 lightVec =  lightPos - vert;
				half len = length( lightVec );
				half param = saturate((lightDist - len) / lightDist );
				param = ( param * param ) * lightIdentity * 0.5;
				half normalParam = dot( normalize( lightVec ) , normalize( normal) ) * 0.5 ;
				return saturate( normalParam * param );
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 nvec   = mul (_Object2World, float4(v.normal,0.0) );
				float4 position = mul (_Object2World, v.vertex);
				
				// UVに書き込みます
				#if UNITY_UV_STARTS_AT_TOP
				o.vertex = float4( (v.writeToUv.x -0.5 )  , (-v.writeToUv.y +0.5)  , 0.0 , 0.5 );
				#else
				o.vertex = float4( (v.writeToUv.x -0.5 )  , (v.writeToUv.y -0.5)  , 0.0 , 0.5 );
				#endif
				o.position = position;

	            o.normal = nvec.xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float4 addCol = float4(0,0,0,0);
				
				addCol.r = calculateLightEffect( _Light0.xyz , i.position.xyz , i.normal, _Light0.w , _LightParam.x );
				addCol.g = calculateLightEffect( _Light1.xyz , i.position.xyz , i.normal, _Light1.w , _LightParam.y );
				addCol.b = calculateLightEffect( _Light2.xyz , i.position.xyz , i.normal, _Light2.w , _LightParam.z );
				addCol.a = calculateLightEffect( _Light3.xyz , i.position.xyz , i.normal, _Light3.w , _LightParam.w );

				//addCol = i.color;
				return addCol;
			}
			ENDCG
		}
	}
}
