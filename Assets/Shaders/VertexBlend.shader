Shader "Custom/VertexBlend" {

	Properties {
		_Texture1 ("Texture1", 2D) = ""
		_Texture2 ("Texture2", 2D) = ""
		//_Bump ("Bumpmap", 2D) = ""
	}

	Subshader {
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0
		struct Input {
			float2 uv_Texture1;
			float2 uv_Texture2;
			float4 color : COLOR;
		};

		sampler2D _Texture1;
		sampler2D _Texture2;

		void surf (Input IN, inout SurfaceOutputStandard o) {
	
			fixed4 tex1 = tex2D (_Texture1, IN.uv_Texture1);
			fixed4 tex2 = tex2D (_Texture2, IN.uv_Texture2);
			fixed alpha = IN.color.a;

			o.Albedo = lerp (tex1, tex2, alpha);
		}
		ENDCG
	}
	Fallback "Diffuse"
}