Shader "Dani/LaptopFace"
{
	Properties
	{
		_BackgroundColor("Screen Color",Color) = (1,1,1,1)
		_FaceColor("Face Color",Color) = (1,1,1,1)
		_HappyFace("Happy Face",2D) = "white"
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry" }


		Pass
		{
			ZWrite On
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

			sampler2D _HappyFace;
			half4 _BackgroundColor;
			half4 _FaceColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 happyFace = tex2D(_HappyFace,i.uv * float2(1,2) - float2(0,0.5));
				happyFace *= _FaceColor;
				half4 bg = _BackgroundColor;
				half4 final = half4(0, 0, 0, 1);
				final.rgb = bg.rgb * (1 - happyFace.a) + happyFace.rgb * happyFace.a;
				return final;
			}
			ENDCG
		}
	}

	FallBack "VertexLit"
}
