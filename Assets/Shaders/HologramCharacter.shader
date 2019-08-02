// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/HologramCharacter"
{
	Properties
	{
		_TintColor("Tint Color",Color) = (1,1,1,1)
		_MainTex("Main Texture",2D) = "white"
	}
	SubShader
	{
		Tags { "RenderType"="Geometry" "Queue" = "Geometry" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertexWorld : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _TintColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 mainCol = tex2D(_MainTex, i.uv);

				float lines = (frac((i.vertexWorld.y * 1) + _Time.y * .5));
				float lines2 = (frac((i.vertexWorld.y * 4) + _Time.y * 5));
				lines = frac(lines);
				lines = abs(0.5 - lines) * 2;
				lines = saturate(lines);
				float d = dot(normalize(_WorldSpaceCameraPos.xyz - i.vertexWorld.xyz),normalize(i.normal));
				d = (d + 1) / 2;
				d = (1-d)* 2;
				half4 finalCol = _TintColor;
				finalCol.rgb -= mainCol.b * .05;
				finalCol.rgb += d * 1;
				finalCol.a += d * 1;
				finalCol.rgb -= lines * .5;
				finalCol.rgb += lines2 * .15;
				finalCol.a = saturate(finalCol.a) - (((sin(_Time.y * 5) + 1) / 2) * 0.25 );
				return saturate(finalCol);
			}
			ENDCG
		}
	}

	FallBack "VertexLit"
}
