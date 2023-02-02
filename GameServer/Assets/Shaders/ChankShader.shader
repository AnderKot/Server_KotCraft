
Shader "Custom/ChankShader"
{
    
    Properties
    {
        _DirtTex("Dirt", 2D) = "white" {}
        _GrassTopTex("Grass top", 2D) = "darckgreen" {}
        _GrassSideTex("Grass side", 2D) = "green" {}
        _StoneTex("Stone", 2D) = "black" {}
        _Scale("Scale", float) = 1
        [NoScaleOffset] _BumpMap("Normalmap", 2D) = "bump" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 250

        CGPROGRAM
        #pragma surface surf Lambert noforwardadd

        sampler2D _DirtTex;
        sampler2D _StoneTex;
        sampler2D _GrassTopTex;
        sampler2D _GrassSideTex;

        float _Scale;

        struct Input
        {
            float2 uv_DirtTex;
            float3 worldPos;
            float3 worldNormal;
            
        };


        void surf(Input IN, inout SurfaceOutput o)
        {
            float x = IN.worldPos.x * _Scale;
            float y = IN.worldPos.y * _Scale;
            float z = IN.worldPos.z * _Scale;

            float IsUp = abs(IN.worldNormal.y);
            float IsFront = abs(IN.worldNormal.x);
            float IsRight = abs(IN.worldNormal.z);

            
            float2 offset = float2(x * IsUp + z * IsFront + y * IsRight, z * IsUp + y * IsFront + x * IsRight);
            if (IsRight)
                offset = float2(offset.y,offset.x);

            fixed4 c;
            
            if (IN.uv_DirtTex.x > 2) // Трава
            {
                if (IsUp > 0)
                {
                    float4 Green = (0, 0, 0, 0);
                    float4 BaseColor = tex2D(_GrassTopTex, offset);
                    c = BaseColor;// +Green;
                }
                else if (IsFront + IsRight > 0)
                {
                    c = tex2D(_GrassSideTex, offset);
                }
                else
                    c = tex2D(_DirtTex, offset);

                o.Albedo = c.rgb;
                o.Alpha = c.a;
                return;
            }

            if (IN.uv_DirtTex.x > 1) // Камень
            {
                c = tex2D(_StoneTex, offset);
                o.Albedo = c.rgb;
                o.Alpha = c.a;
                return;
            }

            if (IN.uv_DirtTex.x > 0) // Земля
            {
                 c = tex2D(_DirtTex, offset);
                 o.Albedo = c.rgb;
                 o.Alpha = c.a;
                 return;
            }
        }
        ENDCG
    }

    FallBack "Mobile/Diffuse"
}