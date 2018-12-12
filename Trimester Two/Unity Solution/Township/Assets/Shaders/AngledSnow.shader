// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/AngledSnow" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Bump("Normal", 2D) = "bump" {}
		_Snow("Snow Level", Range(0,.33)) = 0
		_SnowColor("Snow Color", Color) = (1.0,1.0,1.0,1.0)
		_SnowDirection("Snow Direction", Vector) = (0,1,0)
		_SnowDepth("Snow Depth", Range(0,0.2)) = 0.1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows	vertex:vert 
			//#pragma surface surf Lambert vertex:vert

			sampler2D _MainTex;
			sampler2D _Bump;
			float _Snow;
			float4 _SnowColor;
			float4 _SnowDirection;
			float _SnowDepth;
			float _Breakage;
			float _Gravity;

			struct Input {
				float2 uv_MainTex;
				float2 uv_Bump;
				float3 worldNormal;
				INTERNAL_DATA
			};

			void vert(inout appdata_full v) {
				//Convert the normal to world coortinates
				float3 snormal = normalize(_SnowDirection.xyz);
				float3 sn = mul((float3x3)unity_WorldToObject, snormal).xyz;

				if (dot(v.normal, sn) >= lerp(1,-1, (_Snow * 2) / 3))
				{
				   v.vertex.xyz += normalize(sn + v.normal) * _SnowDepth * _Snow;
				}

				v.vertex.xyz += _Breakage * v.normal;
				v.vertex.z -= _Gravity;
			}

			//float rand(vec2 co) {
			//	return fract(sin(dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
			//}

			void surf(Input IN, inout SurfaceOutputStandard o) {
				half4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_Bump));
				//if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) >= 0.34)
				if (dot(WorldNormalVector(IN, o.Normal), _SnowDirection.xyz) >= lerp(1,-1,_Snow))
				{
					o.Albedo = _SnowColor.rgb * lerp(.65, 1, _Snow / 0.33);
					//o.Albedo = lerp(c.rgb, _SnowColor.rgb, _Snow / 0.33));
				}
				else {
					o.Albedo = c.rgb;
				}
				o.Metallic = 0;
				o.Smoothness = 0.2;
				o.Alpha = 1;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
