Shader "Dani/GreyScaleImageEfect"
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white" {}
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 main = tex2D(_MainTex,i.uv);
				//half3 invertedGreyScale = dot(1 - main.rgb, float3(0.3, 0.59, 0.11));
				main.rgb = dot(main.rgb, float3(0.3, 0.59, 0.11));
				//main.rgb = invertedGreyScale;
				half3 red = main.rgb * float3(1, 0, 0);
				half4 second = half4(red.r, red.g, red.b, 1);
				float dst = sqrt(pow(i.uv.x - 0.5, 2) + pow(i.uv.y - 0.5, 2));
				return lerp(main,second,dst * 0.5);
			}
			ENDCG
		}
	}
}
