Shader "Dani/TextureFaderImageEffect"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_SecondTex("Second Texture",2D) = "white" {}
		_T("T Value",Range(0,1)) = 0
		_UseMask("Should we use fake DOF mask.",int) = 0
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
			sampler2D _FakeDOF;
			float _T;
			int _GreyScale;
			int _UseMask;

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
				half4 maskTex = tex2D(_FakeDOF,i.uv);
				sTex.rgb = lerp(sTex.rgb, dot(sTex.rgb, float3(0.3, 0.59, 0.11)), _GreyScale);
				sTex.rgb -= _UseMask * 0.1;
				//sTex.rgb *= saturate((1-_UseMask) + 0.75);
				return lerp(fTex,sTex,_T * saturate((1-ceil(maskTex.r)) + (1-_UseMask)));
			}
			ENDCG
		}
	}
}
