Shader "Dani/Transition"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_Mask("Mask Texture",2D) = "white" {}
		_T("_T",Range(0,1)) = 0
		_UVFlipped("Flipped",int) = 0
		_Seed("Seed",int) = 2
	}
	SubShader
	{
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
			sampler2D _Mask;
			float _T;
			int _UVFlipped;
			int _Seed;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float rand(float r) {
				r = frac(r * 341.12);
				r = dot(float2(r,r*r),float2(r*213.6, r + 23.341));
				return frac(r*(r + 116.12));
				//return frac(abs(frac(frac(pow(r*0.4512, 2) / r)) - abs(r - 0.528)));
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			half4 col = tex2D(_MainTex,i.uv);
			i.uv.x = lerp(i.uv.x,1-i.uv.x,_UVFlipped);
			float2 modifiedUV = float2(frac(i.uv.x), frac(i.uv.y * 10));
			float id = floor(i.uv.y * 10) + 1;
			//return rand(id + _Seed);
			modifiedUV.x += rand(id + (_Seed*20));
			_T = lerp(0.2, 1, _T);
			float t =  (_T * 1.5);
			modifiedUV.x += t;
			modifiedUV.x *= t;
			modifiedUV.y = 1 - (abs(0.5 - modifiedUV.y) * 2);
			float size = 0.75;
			size = size - smoothstep(1, 2, modifiedUV.x);
			float aa = 0.055;
			float lines = smoothstep(size / (modifiedUV.x * 1), (size + aa) / (modifiedUV.x * 1),modifiedUV.y);
			col = lerp(half4(0, 0, 0, 0), col, 1 - lines);
			return col;
			}
			ENDCG
		}
	}
}
