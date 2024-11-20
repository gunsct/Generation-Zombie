Shader "Hidden/UI/UIShiny_SoftMask (SoftMaskable)"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		_SoftMask("Mask", 2D) = "white" {}

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		_ParamTex ("Parameter Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ SOFTMASK_SIMPLE SOFTMASK_SLICED SOFTMASK_TILED

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#define UI_SHINY 1
			#include "Assets/Packages/UIEffect/Resources/UIEffect.cginc"//"UIEffect.cginc"
			//#include "Assets/Packages/UIEffect/Resources/UIEffectSprite.cginc"//"UIEffectSprite.cginc"
			#include "Assets/Packages/SoftMask/Shaders/SoftMask.cginc"


			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			#if EX
				float2	uvMask	: TEXCOORD1;
			#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			#if UI_DISSOLVE || UI_TRANSITION
				half3	eParam	: TEXCOORD2;
			#elif UI_SHINY
				half2	eParam	: TEXCOORD2;
			#else
				half	eParam	: TEXCOORD2;
			#endif
			#if EX
				half4	uvMask	: TEXCOORD3;
			#endif
				SOFTMASK_COORDS(4)
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

#if UI_EFFECT
				OUT.texcoord = UnpackToVec2(IN.texcoord.x) * 2 - 0.5;
#else
				OUT.texcoord = UnpackToVec2(IN.texcoord.x);
#endif

#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
#endif

				OUT.color = IN.color * _Color;

#if UI_DISSOLVE || UI_TRANSITION
				OUT.eParam = UnpackToVec3(IN.texcoord.y);
#elif UI_SHINY
				OUT.eParam = UnpackToVec2(IN.texcoord.y);
#else
				OUT.eParam = IN.texcoord.y;
#endif

#if EX
				OUT.uvMask = half4(UnpackToVec2(IN.uvMask.x), UnpackToVec2(IN.uvMask.y));
#endif

				SOFTMASK_CALCULATE_COORDS(OUT, IN.vertex);

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				color = ApplyShinyEffect(color, IN.eParam);
				color.a *= IN.color.a * SOFTMASK_GET_MASK(IN);

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
}
