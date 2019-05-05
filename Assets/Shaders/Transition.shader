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
			//_T*= 0.5;
			half4 col = tex2D(_MainTex,i.uv);
			float bw = 0.001;
			float bww = 0.01;
			float halft = min(_T,0.51);
			float uso = sin(i.uv.x* 20) * 0.01 * _T;
			float bso = sin(i.uv.x* 20) * 0.01 * _T;
			float b = smoothstep(halft-0.01,halft,i.uv.y + uso);
			float bo = smoothstep(halft-(bw+bww),halft-(bww),i.uv.y + uso);
			float inverseT = (1-min(_T,0.51));
			float u = smoothstep(inverseT+0.01,inverseT,i.uv.y + bso);
			float uo = smoothstep(inverseT+(bw + bww),inverseT + (bww),i.uv.y + bso);

			float g = abs(0.5 - i.uv.x) * 2;
			_T = 1-smoothstep(0.5,1,_T);
			float fade = 0.2 * (1-_T);
			uo *= smoothstep(_T,_T-fade,g);
			bo *= smoothstep(_T,_T-fade,g);
			col.rgb *= b * u;
			col.rgb += bo * (1-b) + uo * (1-u);
			return col;
			}
			ENDCG
		}
	}
}
