
Shader "Custom/ChankShader"
{
    
    Properties
    {
        _DirtTex("Dirt", 2D) = "white" {}
        _StoneTex("Stone", 2D) = "black" {}
        _GrassTopTex("Grass top", 2D) = "darckgreen" {}
        _GrassSideTex("Grass side", 2D) = "green" {}
        _Scale("Scale", float) = 1
        [NoScaleOffset] _BumpMap("Normalmap", 2D) = "bump" {}
    }

    SubShader
    {
        //Tags { "RenderType" = "Opaque" }
        //LOD 250
        
        
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
                //float2 uv_BlockID;
                float2 uv2_StoneTex;
                float3 worldPos;
                float3 worldNormal;
            };

            void surf(Input IN, inout SurfaceOutput o)
            {
                float x = IN.worldPos.x * _Scale;
                float y = IN.worldPos.y * _Scale;
                float z = IN.worldPos.z * _Scale;

                float IsUp = IN.worldNormal.y;
                float IsFront = abs(IN.worldNormal.x);
                float IsRight = abs(IN.worldNormal.z);


                float2 offset = float2(x * IsUp + z * IsFront + y * IsRight, z * IsUp + y * IsFront + x * IsRight);


                fixed4 c;

                if (IN.uv_DirtTex.x > 2) // Трава
                {
                    if (IsUp > 0)
                    {
                        c = tex2D(_GrassTopTex, IN.uv2_StoneTex);// +Green;
                    }
                    else if (IsFront + IsRight != 0)
                    {
                        c = tex2D(_GrassSideTex, IN.uv2_StoneTex);
                    }
                    else
                        c = tex2D(_DirtTex, IN.uv2_StoneTex);

                    o.Albedo = c.rgb;
                    o.Alpha = c.a;
                    return;
                }

                if (IN.uv_DirtTex.x > 1) // Камень
                {
                    c = tex2D(_StoneTex, IN.uv2_StoneTex);
                    o.Albedo = c.rgb;
                    o.Alpha = c.a;
                    return;
                }

                if (IN.uv_DirtTex.x > 0) // Земля
                {
                        c = tex2D(_DirtTex, IN.uv2_StoneTex);
                        o.Albedo = c.rgb;
                        o.Alpha = c.a;
                        return;
                }
            }
        ENDCG
        
        /*
        Pass{
            Name "AddGrass"
            Cull Off
            CGPROGRAM
                
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            // -- Структуры --
            struct vertexOutput
            {
                
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 col : COLOR;
            };

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct geometryOutput
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 col : COLOR;
            };

            // -- функции --
            geometryOutput GetVertex(float4 pos, float2 uv, fixed4 col)
            { 
                geometryOutput o;
                o.pos = UnityObjectToClipPos(pos); 
                o.uv = uv;
                o.col = col; 
                return o; 
            }

            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }

            // -- шейдеры --

            vertexOutput vert(vertexInput v)
            { 
                vertexOutput o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.tangent = v.tangent;
                return o; 
            }
            
            fixed4 frag(geometryOutput i) : SV_Target
            {
                //fixed4 gradientMapCol = tex2D(_GradientMap, float2(i.col.x, 0.0)); 
                //fixed4 col = (gradientMapCol + _WindColor * i.col.g) * _Color; 
                geometryOutput o;
                o = i;
                return i.col;
            }

            [maxvertexcount(3)]
            void geom(triangle vertexOutput input[3], inout TriangleStream < geometryOutput > triStream)
            {
                float4 pos = input[0].vertex;

                float3 vNormal = input[0].normal;
                float4 vTangent = input[0].tangent;
                float3 vBinormal = cross(vNormal, vTangent) * vTangent.w;
                float3x3 tangentToLocal = float3x3(
                    vTangent.x, vBinormal.x, vNormal.x,
                    vTangent.y, vBinormal.y, vNormal.y,
                    vTangent.z, vBinormal.z, vNormal.z
                    );
                //float3 pos = IN[0].vertex.xyz;
                //
                float r1 = random(mul(unity_ObjectToWorld, input[0].vertex).xz);
                float r2 = random(mul(unity_ObjectToWorld, input[1].vertex).xz);
                float r3 = random(mul(unity_ObjectToWorld, input[1].vertex).xz);
                //float4 midpoint = (1 - sqrt(r1)) * input[0].vertex + (sqrt(r1) * (1 - r2)) * input[1].vertex + (sqrt(r1) * r2) * input[2].vertex;
                
                //float4 midpoint = input[0].vertex * input[1].vertex * input[2].vertex;

                //float4 worldPos = mul(unity_ObjectToWorld, midpoint);
                //float height = (rand(pos.zyx) * 2 - 1);
                //float width = (rand(pos.xzy) * 2 - 1);
                //float forward = rand(pos.yyz);

                // Update each assignment of o.pos in the geometry shader.
                pos = UnityObjectToClipPos(r1 + mul(tangentToLocal, float3(0.2, 0, 0)));
                triStream.Append(GetVertex(pos, float2(0,0), fixed4(0, 0.1, 0, 1)));

                pos = UnityObjectToClipPos(r2 + mul(tangentToLocal, float3(-0.2, 0, 0)));
                triStream.Append(GetVertex(pos, float2(1, 0), fixed4(0, 0.1, 0, 1)));

                pos = UnityObjectToClipPos(r3 + mul(tangentToLocal, float3(0, 0, 1)));
                triStream.Append(GetVertex(pos, float2(0, 0), fixed4(0, 0.1, 0, 1)));
            }
            ENDCG
        }
        */
        /*
        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            fixed4 _OutlineColor;
            float _OutlineSize;
            struct appdata
            {
                float4 vertex:POSITION;
            };

            struct v2f
            {
                float4 clipPos:SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.clipPos = UnityObjectToClipPos(v.vertex * 1.002);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return float4(0,0.5,0,1);
            }
            ENDCG
        }
        */
    }

    FallBack "Mobile/Diffuse"
}