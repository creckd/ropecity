Shader "Dani/UnlitTextureUIMaskWrite"
{
	Properties
	{
		_TintColor("Tint Color",Color) = (0,0,0,0)
		_T("Time",Range(0,1))= 0
		_Seed("Seed",int) = 1
		_UVFlipped("UV is flipped",int) = 0
		_MainTex("Main Texture",2D) = "white"
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

		Stencil {
			Ref 1
			Comp Always
			Pass Replace
		}

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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _TintColor;
			float _T;
			int _Seed;
			int _UVFlipped;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float rand(float r) {
				r = frac(r * 341.12);
				r = dot(float2(abs(r-12.415)*r, frac(r*r)*r), float2(r*213.6, r + 23.341));
				return frac(r*(r + 116.12));
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			i.uv.x = lerp(i.uv.x,1 - i.uv.x,_UVFlipped);
			float2 modifiedUV = float2(frac(i.uv.x), frac(i.uv.y * 10));
			float id = floor(i.uv.y * 10) + 1;
			modifiedUV.x += rand(id + (_Seed * 20));
			_T = lerp(0.2, 1, _T);
			float t = _T * 1.3;
			modifiedUV.x += t;
			modifiedUV.x *= t;
			modifiedUV.y = 1 - (abs(0.5 - modifiedUV.y) * 2);
			float size = 0.75;
			size = size - smoothstep(1, 2, modifiedUV.x);
			float aa = 0.055;
			float lines = smoothstep(size / (modifiedUV.x * 1), (size + aa) / (modifiedUV.x * 1),modifiedUV.y);
			clip((1-lines)-0.01);
			return half4(0,0,0,0);
			}
			ENDCG
		}
	}
}
