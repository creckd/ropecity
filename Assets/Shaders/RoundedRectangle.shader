﻿Shader "Dani/RoundedRectangle"
{
	Properties
	{
		_Width("Rect Width", Range(0,1)) = 1
		_Height("Rect Height", Range(0,1)) = 1
		_Radius("Roundness", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

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
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _MainColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			float _Width;
			float _Height;
			float _Radius;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float Width = _Width;
				float Height = _Height;
				float Radius = _Radius;
				Radius = max(min(min(abs(Radius * 2), abs(Width)), abs(Height)), 1e-5);
				float2 uv = abs(i.uv * 2 - 1) - float2(Width, Height) + Radius;
				float d = length(max(0, uv)) / Radius;
				float f = saturate((1 - d) / fwidth(d));
				float4 finalColor;
				finalColor.a = f;
				finalColor.rgb = i.color.rgb;
				finalColor.a *= i.color.a;
				return finalColor;
			}
			ENDCG
		}
	}
}