Shader "Dani/GreyScaleImageEfect"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_Glitch("Glitch",2D) = "white" {}
		_GlitchStrength("GlitchStrength",Range(0,1)) = 0
	}
	SubShader
	{
	Pass
	{
		ZWrite Off
		Cull off
		Lighting Off

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
				float2 glitchUV : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _Glitch;
			float _GlitchStrength;
			float4 _Glitch_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.glitchUV = TRANSFORM_TEX(v.uv, _Glitch);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 glitch = tex2D(_Glitch, i.glitchUV);
				half4 main = tex2D(_MainTex,i.uv);
				half4 mainGrey = main;
				mainGrey.rgb = dot(main.rgb, float3(0.3, 0.59, 0.11));
				half4 second = mainGrey;
				second.rgb *= float3(1, 0, 0);
				float dst = sqrt(pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2));
				half4 final = lerp(mainGrey, second, dst * 0.5);
				float whiteness = 1 - saturate(abs(0.4 - _GlitchStrength) / 0.4);
				whiteness *= 0.75;
				final = lerp(main, final, _GlitchStrength);
				final.rgb = lerp(final.rgb, half3(1,1,1), whiteness);
				return final;
			}
			ENDCG
		}
	}
}
