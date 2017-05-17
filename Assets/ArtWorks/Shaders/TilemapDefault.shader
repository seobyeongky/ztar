Shader "Ztar/Tilemap Default"
{
    Properties
	{
		_MainTex("Base RGBA", 2D) = "white" {}
        _NormalTex("Normal Map", 2D) = "bump" {}
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade nofog nolightmap noinstancing nodynlightmapd
        #pragma target 3.0
        #include "UnityPBSLighting.cginc"

        sampler2D _MainTex;
        sampler2D _NormalTex;

        struct Input {
            float2 uv_MainTex;
        };

        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            fixed4 n = tex2D(_NormalTex, IN.uv_MainTex);
            fixed3 normal;
            normal.xyz = n.xyz * 2 - 1;
            o.Normal = normal; // normalize(normal);
            o.Alpha = c.a;
        }
        ENDCG
    }
}