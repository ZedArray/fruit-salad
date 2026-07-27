Shader "UI/CutoutMask"
{
    // Draws a solid background color (e.g. black) over the full rect,
    // but punches a transparent "hole" wherever the mask sprite has alpha.
    // Animate _Scale to grow/shrink the hole (bigger _Scale = smaller hole,
    // since we're zooming into the mask texture).

    Properties
    {
        _MainTex ("Mask Sprite (hole shape)", 2D) = "white" {}
        _Color ("Background Color", Color) = (0,0,0,1)
        _Scale ("Hole Scale (1 = normal, >1 = smaller hole, <1 = bigger hole)", Range(0.01, 10)) = 1
        _Center ("Mask Center (UV 0-1)", Vector) = (0.5, 0.5, 0, 0)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        // Standard UI properties so this still works inside other Masks/RectMask2D
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
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
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Scale;
            float2 _Center;
            float _Cutoff;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Zoom the mask UV around its center to grow/shrink the hole
                float2 uv = (IN.texcoord - _Center) / _Scale + _Center;

                fixed4 maskSample = tex2D(_MainTex, uv);

                // Outside the 0-1 UV range after zooming = treat as "no hole" (fully background)
                bool outOfBounds = (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0);
                float holeAlpha = outOfBounds ? 0.0 : maskSample.a;

                fixed4 col = IN.color;
                // Where the mask is opaque enough, cut a hole (alpha -> 0)
                col.a *= (holeAlpha > _Cutoff) ? 0.0 : 1.0;

                return col;
            }
            ENDCG
        }
    }
}
