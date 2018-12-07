Shader "Custom/Terrain_Snow" {
	Properties{
		_Tess("Tessellation", Range(1,32)) = 4
		_Splat("Splat Map", 2D) = "black" {}
		_Displacement("Displacement", Range(0, 1.0)) = 0.3

		_SnowColor("Snow Color", Color) = (1.0,1.0,1.0,1.0)
		_RecordHeights("RecordHeights", Float) = 1
		testTexture("Texture", 2D) = "white"{}
		testScale("Scale", Float) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows	vertex:disp tessellate:tessDistance

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 4.6

			// ----------------------------------------------------------------------------------------------

			#include "Tessellation.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			float _Tess;
			sampler2D _Splat;
			float _Displacement;
			float _RecordHeights;
			float _SnowColor;

			float4 tessDistance(appdata v0, appdata v1, appdata v2) {
				float minDist = 10.0;
				float maxDist = 2500.0;
				return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
			}


			void disp(inout appdata v)
			{
				float d = tex2Dlod(_Splat, float4(v.texcoord.xy,0,0)).r * _Displacement;
				//v.vertex.xyz -= v.normal * d;*/
				//if(v.vertex.y - _Displacement > maxHeight / 2)

				v.vertex.y += _Displacement;
				v.vertex.y -= d;
				//v.vertex.y += _Displacement;
			}

			// ----------------------------------------------------------------------------------------------

			const static int maxLayerCount = 8;
			const static float epsilon = 1E-4;

			int layerCount;
			float3 baseColours[maxLayerCount];
			float baseStartHeights[maxLayerCount];
			float baseBlends[maxLayerCount];
			float baseColourStrength[maxLayerCount];
			float baseTextureScales[maxLayerCount];

			float minHeight;
			float maxHeight;

			sampler2D testTexture;
			float testScale;

			UNITY_DECLARE_TEX2DARRAY(baseTextures);

			struct Input {
				float3 worldPos;
				float3 worldNormal;
				float2 uv_Splat;
			};

			float inverseLerp(float a, float b, float value) {
				return saturate((value - a) / (b - a));
			}

			float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
				float3 scaledWorldPos = worldPos / scale;
				float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
				float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
				float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
				return xProjection + yProjection + zProjection;
			}

			void surf(Input IN, inout SurfaceOutputStandard o) {
				if (_RecordHeights == 1) {


					float heightPercent = inverseLerp(minHeight, maxHeight, IN.worldPos.y);
					float3 blendAxes = abs(IN.worldNormal);
					blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

					for (int i = 0; i < layerCount; i++) {
						float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, heightPercent - baseStartHeights[i]);

						float3 baseColour = baseColours[i] * baseColourStrength[i];
						float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColourStrength[i]);

						o.Albedo = o.Albedo * (1 - drawStrength) + (baseColour + textureColour) * drawStrength;
					}

					fixed3 n = o.Albedo;
					fixed3 d = _SnowColor;

					// if we are above water level form white snow, else form grey ice
					if (_Displacement > 0)
					{
						if (IN.worldPos.y - _Displacement > 1.4)
						{
							o.Albedo = lerp(n, float4(1, 1, 1, 1), _Displacement);

							fixed3 newN = o.Albedo;

							// lerp between light and dark snow depending if the current snow is depressed or not
							half amount = tex2Dlod(_Splat, float4(IN.uv_Splat, 0, 0)).r;
							//fixed3 c = lerp(newN, float4(0.35, 0.35, 0.35, 0.35), amount * _Displacement);
							fixed3 c = lerp(newN, (n * float4(.35, .35,.35, .35)), amount * _Displacement);
							o.Albedo = c;
						}
						else o.Albedo = lerp(n, float4(.85, .85, .85, 1), _Displacement);
					}
				}

			

			}


			ENDCG
		}
			FallBack "Diffuse"
}