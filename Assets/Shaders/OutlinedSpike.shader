Shader "Dani/OutlinedSpike"
{
	Properties
	{
		_TintColor("Tint Color",Color) = (0,0,0,0)
		_MainTex("Main Texture",2D) = "white"
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		Stencil {
		Ref 5
		Comp Always
		Pass Replace
		}
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _TintColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 mainCol = tex2D(_MainTex,i.uv);
				mainCol.a = 0;
				return mainCol;
			}
			ENDCG
		}
				Pass
		{

		Stencil {
		Ref 5
		Comp NotEqual
		}
		Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 vertexWorld : TEXCOORD1;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _TintColor;

			v2f vert(appdata v)
			{
				v2f o;
				float4 modVertex = v.vertex;
				//v.normal *= saturate(v.vertex.z * 10);
				modVertex.xyz += v.normal.xyz * 0.005;
				modVertex = UnityObjectToClipPos(modVertex);
				o.vertex = modVertex;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
				o.vertexWorld = v.vertex;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half4 mainCol = tex2D(_MainTex,i.uv);
				float4 outlineColor = _TintColor;
				//outlineColor.rgb -= 1 - saturate(i.vertexWorld.z * 40);
				return outlineColor;
			}
			ENDCG
		}
		
	}
}
