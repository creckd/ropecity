// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/WorldYTransparencyTexture"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_WorldY("World Y",Float) = 0
		_Falloff("Falloff",Range(0.1,20)) = 1
	}
	SubShader
	{
				Tags { "RenderType" = "Transparent" "Queue" = "Geometry" }

				// Render into depth buffer only
				Pass {
					ColorMask 0
				}
			//Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	Pass
	{
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 vertexWorld : TEXCOORD1;
			};

			sampler2D _MainTex;
			float _WorldY;
			float _Falloff;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertexWorld = mul(unity_ObjectToWorld,v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex,i.uv);
				col.a *= saturate((i.vertexWorld.y - _WorldY) / _Falloff);
				return col;
			}
			ENDCG
		}
	}
}
