// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dani/SimpleBlurVertical" {
	Properties{
		_Size("Size", float) = 0
		_MainTex("Tint Color (RGB)", 2D) = "white" {}
	}
		SubShader{
	Pass{
		Cull Off
		Lighting Off
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		struct appdata_t {
			float4 vertex : POSITION;
			float2 texcoord: TEXCOORD0;
		};

		struct v2f {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata_t v) {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		float _Size;
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		half4 frag(v2f i) : COLOR {

			half4 sum = half4(0,0,0,0);

			#define GRABPIXEL(weight,kernely) tex2D( _MainTex, float2(i.uv.x, i.uv.y + _MainTex_TexelSize.y * kernely*_Size)) * weight

			sum += GRABPIXEL(0.05, -4.0);
			sum += GRABPIXEL(0.09, -3.0);
			sum += GRABPIXEL(0.12, -2.0);
			sum += GRABPIXEL(0.15, -1.0);
			sum += GRABPIXEL(0.18,  0.0);
			sum += GRABPIXEL(0.15, +1.0);
			sum += GRABPIXEL(0.12, +2.0);
			sum += GRABPIXEL(0.09, +3.0);
			sum += GRABPIXEL(0.05, +4.0);

			return sum;
		}
		ENDCG
	}
	}
}