﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/MineLamp"
{
	Properties
	{
		_TintColor("Tint Color", Color) = (0,0,0,0)
		_T("T", Range(0,1)) = 0.01
	}
	SubShader
	{
	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
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

			float4 _TintColor;
			float _T;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv =  v.uv;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float x = i.color.r;
				float t = sin(radians(frac(_Time.x * 5) * 180));
				return lerp(half4(0,0,1,0.1),half4(1,0,0,1),saturate(t-x) * 4);
			}
			ENDCG
		}
	}
}
