// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/Glass"
{
	Properties
	{
		_MainTex("Glass Texture",2D) = "white" {}
		_MainColor("Transparent Color",Color) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Less
		//Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 vertexWorld : TEXCOORD1;
				float4 normal : NORMAL;
			};

			sampler2D _MainTex;
			sampler2D _MainTex_ST;
			float4 _MainColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				float3 w = mul(unity_ObjectToWorld, float4(float3(v.normal.x,v.normal.y,v.normal.z), 0.0)).xyz;
				o.normal.xyz = w;
				o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 glassTex = tex2D(_MainTex,i.uv);
				float3 worldSpaceViewDir = _WorldSpaceCameraPos.xyz - i.vertexWorld.xyz;
				float s = 1 - dot(normalize(worldSpaceViewDir), normalize(i.normal));
				float specular = step(0.25, s) * 0.1;
				specular += step(0.65, s) * 0.1;
				specular += step(0.85, s) * 0.1;
				//specular += step(0.75, s) * 0.2;
				//specular += step(0.85, s) * 0.2;
				//specular += step(0.65, s) * 0.15;
				//specular += step(0.85, s) * 0.15;

				half4 finalColor = lerp(glassTex,_MainColor,1-glassTex.a);

				//finalColor += s * 1;
				//finalColor.a *= s;
				finalColor += specular;
				finalColor = saturate(finalColor);
				return finalColor;
			}
			ENDCG
		}
	}
}
