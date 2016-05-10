﻿Shader "Custom/VertexBlend" {

	Properties {
		_WaveOrigin ("WaveOrigin", Vector) = (0,0,0,0)
		_WaveDistance ("WaveDistance", Float) = 1
		_WaveWidth ("WaveWidth", Float) = 3
		_Texture1 ("Texture1", 2D) = ""
		_Texture2 ("Texture2", 2D) = ""
		_WaveTexture ("WaveTexture", 2D) = ""
		//_Bump ("Bumpmap", 2D) = ""
	}

	Subshader {
		Tags { "RenderType"="Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard keepalpha vertex:vert fullforwardshadows
		#pragma target 3.0
		struct Input {
			float2 uv_Texture1;
			float2 uv_Texture2;
			float4 vertexColor;
		};

		//struct v2f {
		//	float4 pos : SV_POSITION;
		//	fixed4 color : COLOR;
		//};

		float _WaveOrigin;
		float _WaveProgress;
		sampler2D _WaveTexture;

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT (Input, o);
			//v.vertex.y += (1-_WaveProgress) * distance (_WaveOrigin, v.vertex);
			v.vertex.y += tex2Dlod (_WaveTexture, float4 (v.texcoord.xy, 0, 0)).r;
			o.vertexColor = v.color;
		}

		sampler2D _Texture1;
		sampler2D _Texture2;

		//sampler2D _Bump;

		void surf (Input IN, inout SurfaceOutputStandard o) {
	
			fixed4 tex1 = tex2D (_Texture1, IN.uv_Texture1);
			fixed4 tex2 = tex2D (_Texture2, IN.uv_Texture2);
			fixed alpha = IN.vertexColor.a;

			o.Albedo = lerp (tex1, tex2, alpha);
			//o.Normal = UnpackNormal(tex2D (_Bump, IN.uv_Texture1));
		}
		ENDCG
	}
	Fallback "Diffuse"
}