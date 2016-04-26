Shader "Custom/VertexBlend" {

Properties {
	_Tint ("Tint", Color) = (1,1,1,1)
	_Texture1 ("Texture1", 2D) = ""
	_Texture2 ("Texture2", 2D) = ""
}

Subshader {
	BindChannels {
		Bind "vertex", vertex
		Bind "texcoord", texcoord
		Bind "color", color
		Bind "normal", normal
	}

	Pass{SetTexture[_Texture1] {combine texture * primary} }

	Pass {
		Blend One One

		SetTexture[_Texture2] {Combine previous Lerp (primary) texture }
		SetTexture[_] {Combine previous * one - primary}
	}

	Pass {
		Blend DstColor SrcColor

		Material {
			Ambient [_Tint]
			Diffuse [_Tint]
		}

		Lighting On
	}
}

}