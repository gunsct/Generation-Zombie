Shader "UI/Particles/Multiply (Soft Mask)" 
{
    Properties 
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
    
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
    
        _ColorMask ("Color Mask", Float) = 15
        // softmask
        _SoftMask("Mask", 2D) = "white" {}
    
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    Category 
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Stencil
        {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp] 
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
        }
    
        ColorMask [_ColorMask]
        Blend Zero SrcColor
        Cull Off Lighting Off ZWrite Off
        ZTest [unity_GUIZTestMode]
        
        SubShader {
            Pass {
            
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #include "UnityCG.cginc"
                #include "UnityUI.cginc"
                // softmask
                #include "Assets/Packages/SoftMask/Shaders/SoftMask.cginc"
                #pragma multi_compile __ UNITY_UI_ALPHACLIP
                // softmask
                #pragma multi_compile __ SOFTMASK_SIMPLE SOFTMASK_SLICED SOFTMASK_TILED
    
                sampler2D _MainTex;
                fixed4 _TintColor;
                float4 _ClipRect;
                
                struct appdata_t {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                };
    
                struct v2f {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;
                    // softmask
                    SOFTMASK_COORDS(2)
                };
                
                float4 _MainTex_ST;
    
                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.worldPosition = v.vertex;
                    o.vertex = UnityObjectToClipPos(float4(v.vertex));
                    //o.vertex = UnityObjectToClipPos(v.vertex);
                    o.color = v.color;
                    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

                    // softmask
                    SOFTMASK_CALCULATE_COORDS(o, v.vertex);
                    return o;
                }
                
                fixed4 frag (v2f i) : SV_Target
                {
                    half4 prev = i.color * tex2D(_MainTex, i.texcoord);
                    fixed4 col = lerp(half4(1,1,1,1), prev, prev.a * SOFTMASK_GET_MASK(i));
    
                    col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                    // softmask lerp ������� ��
                    //col.a *= SOFTMASK_GET_MASK(i);
                    return col;
                }
                ENDCG 
            }
        }
    }
}
