Shader "Dani/PlatformCylinder"
{
	Properties
	{
		_TintColor("Tint Color",Color) = (1,1,1,1)
		_MainTex("Main Texture",2D) = "white"
		_EffectTallness("Effect Tallness",float) = 8
		_NoiseAmplitute("Noise amplitute",float) = 0.1
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
			half4 _TintColor;
			float _EffectTallness;
			float _NoiseAmplitute;

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
				half4 finalCol = _TintColor;
				finalCol.a = saturate(finalCol.a - noise.b * .3);
				finalCol.a *= (1 - i.uv.y * _EffectTallness);
				finalCol.a += smoothstep(0.6, 0.65, noise.b) * smoothstep(_NoiseAmplitute,0,i.uv.y) * 0.5;
				finalCol.a = saturate(finalCol.a);
				return finalCol;
			}
			ENDCG
		}
	}

	FallBack "VertexLit"
}
