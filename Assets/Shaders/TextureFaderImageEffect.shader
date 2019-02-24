Shader "Dani/TextureFaderImageEffect"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_SecondTex("Second Texture",2D) = "white" {}
		_T("T Value",Range(0,1)) = 0
		_GreyScale("GreyScale",int) = 0
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
			};

			sampler2D _MainTex;
			sampler2D _SecondTex;
			float _T;
			int _GreyScale;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 fTex = tex2D(_MainTex,i.uv);
				half4 sTex = tex2D(_SecondTex, i.uv);
				sTex.rgb = lerp(sTex.rgb, dot(sTex.rgb, float3(0.3, 0.59, 0.11)), _GreyScale);
				return lerp(fTex, sTex, _T);
			}
			ENDCG
		}
	}
}
