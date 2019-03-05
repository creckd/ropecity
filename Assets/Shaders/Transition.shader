Shader "Dani/Transition"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
		_Mask("Mask Texture",2D) = "white" {}
		_LoadingImage("Loading Image",2D) = "white" {}
		_UseLoadingText("Use Loading Text or not",int) = 0
		_T("_T",Range(0,1)) = 0
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
			sampler2D _LoadingImage;
			int _UseLoadingText;
			float _T;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			half4 col = tex2D(_MainTex,i.uv);
			half4 maskCol = tex2D(_Mask, i.uv);
			half4 loading = tex2D(_LoadingImage, i.uv * 4 - float2(1.5,1.5));
			float quickerT = saturate(pow(1 - _T, 12)) - 0.1;
			float gradient = saturate(saturate(1 - (quickerT / maskCol.r)) / 0.05);
			float lines = saturate( saturate(1 - (_T/maskCol.b)) / 0.1);
			float final = saturate((1-lines) - (1-gradient));
			col.rgb = lerp(col.rgb,loading.rgb * loading.a * _UseLoadingText, final);
			//col.rgb = lerp(col.rgb,loading.rgb * loading.a, step(_T, 0.9));
			return col;
			}
			ENDCG
		}
	}
}
