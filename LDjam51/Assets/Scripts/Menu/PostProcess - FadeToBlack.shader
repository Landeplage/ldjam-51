Shader "Custom/PostProcess-FadeToBlack"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Fade ("Fade", Range(0, 1)) = 0.5
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        // We need to set these tags
        Tags { "Queue"="Transparent" }

        Pass
        {
        	// Blend mode
        	Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Fade;
            float _Smoothness;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // Get pixel coordinates (0 to 1 range)
                float2 screenPos = i.uv.xy;

                // TODO: Adjust circle to be uniform
                // _ScreenParams: x is the width of the camera’s target texture in pixels, y is the height of the camera’s target texture in pixels, z is 1.0 + 1.0/width and w is 1.0 + 1.0/height.

                float aspectRatio = _ScreenParams.x / _ScreenParams.y;
                screenPos.x = screenPos.x * aspectRatio;

                // Get distance from pixel to center of screen
                float dis = distance(screenPos, float2(aspectRatio / 2, 0.5));

                // Remap fade-variable
                float newFade = (1.0f - _Fade) * (0.8f + _Smoothness) - _Smoothness;

                //float endFade = smoothstep(0.0f, 1.0f, _Fade * _Fade);

                col.a = smoothstep(newFade, newFade + _Smoothness, dis / (aspectRatio / 2 + 1 - _Smoothness));

                return col;
            }
            ENDCG
        }
    }
}
