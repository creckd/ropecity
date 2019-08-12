// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Dani/DeathEffect"
{
	Properties
	{
		_LocalYMin("Local Mesh Y min",float) = 0
		_LocalYMax("Local Mesh Y max",float) = 0
		_TintColor("Tint Color",Color) = (1,1,1,1)
		_AnimT("Animated T",Range(0,1)) = 0
		_SecondAnimT("Second Animated T",Range(0,1)) = 0
		_MainTex("Main Texture",2D) = "black"
		_Noise("Noise",2D) = "white"
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On

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
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertexWorld : TEXCOORD1;
				float4 vertexLocal : TEXCOORD2;
				float2 noiseUV : TEXCOORD3;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			sampler2D _Noise;
			float4 _MainTex_ST;
			float4 _Noise_ST;
			half4 _TintColor;
			float _AnimT;
			float _SecondAnimT;
			float _LocalYMin;
			float _LocalYMax;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertexWorld = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertexLocal = v.vertex;
				o.noiseUV = TRANSFORM_TEX(float2(o.vertexWorld.y,o.vertexWorld.x), _Noise);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half4 mainCol = tex2D(_MainTex, i.uv);

				float lines = (frac((i.vertexWorld.y * 1) + _Time.y * .5));
				float lines2 = (frac((i.vertexWorld.y * 4) + _Time.y * 5));
				lines = frac(lines);
				lines = abs(0.5 - lines) * 2;
				lines = saturate(lines);
				float d = dot(normalize(_WorldSpaceCameraPos.xyz - i.vertexWorld.xyz),normalize(i.normal));
				d = (d + 1) / 2;
				d = (1-d)* 2;
				half4 holoCol = _TintColor;
				holoCol.rgb -= mainCol.b * .05;
				holoCol.rgb += d * 1;
				holoCol.a += d * 1;
				holoCol.rgb -= lines * .5;
				holoCol.rgb += lines2 * .15;
				holoCol.a = saturate(holoCol.a) - (((sin(_Time.y * 5) + 1) / 2) * 0.25 );
				holoCol = saturate(holoCol);

				float cutoff = lerp(_LocalYMin, _LocalYMax, _AnimT);
				float firstBorder = step(i.vertexLocal.y, cutoff);
				float localRange = _LocalYMax - _LocalYMin;
				float whiteness = 1 - saturate(abs(cutoff - i.vertexLocal.y) / (0.2 * ((localRange / 2.5) * 0.25)));
				whiteness = smoothstep(0.5, 0.6, whiteness);

				half4 finalCol = lerp(mainCol, holoCol, firstBorder);

				half4 noise = tex2D(_Noise, i.noiseUV);
				finalCol.a = step(noise.b, _SecondAnimT);
				float edge = (1 - saturate(abs(_SecondAnimT - noise.b) / 0.1));
				edge = smoothstep(0.5, 0.6, edge);
				whiteness = saturate(whiteness + edge);

				finalCol = lerp(finalCol, half4(1, 1, 1, 1), whiteness);
				//finalCol.rgb += whiteness;

				return finalCol;
			}
			ENDCG
		}
	}

	FallBack "VertexLit"
}
