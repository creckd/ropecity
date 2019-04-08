// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/CannonWheel"
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
				float t = (sin(_Time.y * 4) + 1)/2;
				float col = tex2D(_MainTex,i.uv);
				half2 dir = normalize(i.localVertex.xy);
				half dotP = dot(dir, float2(-1, 0));
				half lenghts = length(dir);
				float angle = degrees(acos(dotP / lenghts));
				half normalizedAngle = angle / 180;
				half highlight = step(i.color.g, 0.99);
				highlight *= 1 - smoothstep(0.15,0.17, abs(t- normalizedAngle));
				return lerp(col, _TintColor, highlight);
			}
			ENDCG
		}
	}
}
