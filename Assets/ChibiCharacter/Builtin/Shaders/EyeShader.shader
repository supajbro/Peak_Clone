Shader "Chibi Character/Built-in/Eye"
{
    Properties
    {
        [NoScaleOffset]
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        _Expression ("Expression", Integer) = 1


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
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            //contains a function to replace colors
            #include "replaceRGBColors.cginc"

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };


            
            
            //gain access to Properties
            float _Expression;            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;

            fixed4 _RTarget;
		    fixed4 _GTarget;
		    fixed4 _BTarget;
            float _Tolerance;
            float _Cutoff;
            

            v2f vert (appdata v)
            {
                //holds the number of expressions
                //there are 4 expressions along the x and 4 expressions in the y direction
                float numExpressionsX=4;
                float numExpressionsY=4;

                //convert expression to an offset (I don't know if I got the correct x versus y on num expressions)
                float xOffset = fmod(_Expression-1, numExpressionsX)/numExpressionsX;                
                float yOffset= floor((_Expression-1)/numExpressionsX)*(-1)/numExpressionsY;  

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv+float2(xOffset,yOffset), _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                //discard transparent pixels
                if (col.a<_Cutoff)
                {
                    discard;        
				}

                //replace colors
                //function is in replaceRGBColors.cginc. Function definition looks like this:
                //fixed4 replaceRGBColors (fixed4 col, fixed4 rTarget, fixed4 gTarget, fixed4 bTarget, float tolerance)
                //it replaces the colors in col with the target colors
                col=replaceRGBColors(col, _RTarget, _GTarget, _BTarget, _Tolerance);

                // Tint the color
                col = col * _Color;


                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
