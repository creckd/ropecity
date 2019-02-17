// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/Tube"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_Mask("Mask",2D) = "white" {}
		_Liquid("Liqui",2D) = "white" {}
		_TilingDirection("Tiling Direction",Vector) = (0,0,0,0)
	}
	SubShader
	{
	Tags { "RenderType" = "Transparent" "Queue" = "Geometry" }
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 vertexWorld : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _Mask;
			sampler2D _Liquid;
			float4 _TilingDirection;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex,i.uv);
				float4 mask = tex2D(_Mask, i.uv);
				float4 liquid = tex2D(_Liquid, (i.uv * 3) + float2(_TilingDirection.x * _Time.y * 0.5, _TilingDirection.y * _Time.y * 0.75));
				int transparentPart = saturate((1 - col.a) * 10);
				int final = transparentPart * step(sin((i.vertexWorld.x + i.vertexWorld.y) / 2 * 2 + _Time.y * 5) * 0.02 + 0.5, mask.r);
				//col.rgb -= final * 0.5;
				col.rgb = lerp(col.rgb, liquid.rgb, final);
				return col;
			}
			ENDCG
		}
	}
}
