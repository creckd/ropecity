// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/Sine"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_WheelTint("Wheel Tint Color",Color) = (0,0,0,0)
		_TintColor("Tint Color", Color) = (0,0,0,0)
		_T("_T",Range(0,1)) = 0
	}
	SubShader
	{
	Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
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
				float4 localVertex : TEXCOORD1;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _TintColor;
			float4 _WheelTint;
			float _T;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv =  v.uv;
				o.color = v.color;
				o.localVertex = v.vertex;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float s = (sin(i.uv.x * 6 + _Time.y * 5) + 1) / 2;
				s *= .5 + ( abs(0.5 - frac(_Time.x * 2)) * .5);
				int mask = step(i.uv.y, s);
				half4 fin = mask * _TintColor;
				clip(fin.a - 0.01);
				return fin;
			}
			ENDCG
		}
	}
}
