Shader "Dani/Vignette"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_Size("Size",Range(0.1,20)) = 1
	}
	SubShader
	{
	Pass
	{
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
			float _Size;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			float dst = sqrt(pow(i.uv.x - 0.5,2) + pow(i.uv.y - 0.5,2));
			dst = pow(dst, 2);
			return half4(0,0,0,dst * _Size);
			}
			ENDCG
		}
	}
}
