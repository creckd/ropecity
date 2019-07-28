Shader "Dani/PlatformCylinder"
{
	Properties
	{
		_TintColor("Tint Color",Color) = (1,1,1,1)
		_MainTex("Main Texture",2D) = "white"
		_HexaGrid("Hexa Grid",2D) = "white"
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }


		Pass
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

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
			sampler2D _HexaGrid;
			half4 _TintColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 noise = tex2D(_MainTex,i.uv + float2(_Time.x,0));
				half4 hexaGrid = tex2D(_HexaGrid, i.uv * float2(2,1) + float2(0, _Time.x));
				half4 finalCol = _TintColor;
				finalCol.a = saturate(finalCol.a - noise.b * .3);
				finalCol.a *= (1 - i.uv.y * 2);
				//finalCol.a *= smoothstep(0.1, 0, i.uv.y);
				finalCol.a += smoothstep(0.6, 0.65, noise.b) * smoothstep(0.1,0,i.uv.y) * 0.5;
				float diff = 0.1;
				finalCol.a += smoothstep(diff, diff - 0.1, 1 - noise.b) * ((1-i.uv.y) * 0.1) * finalCol.a * 10;
				finalCol.a = saturate(finalCol.a);
				return finalCol;
			}
			ENDCG
		}
	}

	FallBack "VertexLit"
}
