Shader "Chibi Character/Built-in/ColorSwap_Lit"
{
    Properties
    {
        [NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        //the color to replace the reds with
		_RTarget("Red Target", Color)=(1, 0, 0, 1)
		
		//the color to replace the greens with
		_GTarget("Green Target", Color)=(0, 1, 0, 1)
		
		//the color to replace the blues with
		_BTarget("Blue Target", Color)=(0, 0, 1, 1)

		_Tolerance("Tolerance", Range(0.001, 1))=0.5

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        //contains a function to replace colors
        #include "replaceRGBColors.cginc"


        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        //gain access to the values in the properties
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        fixed4 _RTarget;
		fixed4 _GTarget;
		fixed4 _BTarget;
        float _Tolerance;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Get the color of the texture
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

            //function is in replaceRGBColors.cginc. Function definition looks like this:
            //fixed4 replaceRGBColors (fixed4 col, fixed4 rTarget, fixed4 gTarget, fixed4 bTarget, float tolerance)
            //it replaces the colors in c with the target colors
            c=replaceRGBColors(c, _RTarget, _GTarget, _BTarget, _Tolerance);

            // Tint the color
            c = c * _Color;

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
