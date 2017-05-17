Shader "Ztar/Tilemap Nice"
{
    Properties
	{
        _MainTex("Base RGBA", 2D) = "white" {}
        _NormalTex("Normal Map", 2D) = "bump" {}
        _SpecTex("Specular Map", 2D) = "black" {}
        _LightPatternTex("Screen Light Pattern ", 2D) = "light pattern" {}
        _Color("Diffuse Material Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _SpecColor("Specular Material Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Shininess("Shininess", Float) = 5
		_SpecRamp("Specular ramp", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
        
        CGPROGRAM
        #pragma surface surf SimpleSpecular noshadow nolightmap nofog noinstancing nodynlightmap noambient novertexlights nolppv

        struct Input {
            float2 uv_MainTex;
            float4 screenPos;
        };

        struct SurfaceOutputCustom {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            half Specular;
            fixed Gloss;
            fixed Alpha;
            fixed3 Specularar;
        };

        sampler2D _MainTex;
        sampler2D _NormalTex;
        sampler2D _LightPatternTex;
        sampler2D _SpecTex;
		sampler2D _SpecRamp;
        float3 _Color;
        float _Shininess;

        half4 LightingSimpleSpecular (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten)
        {
            half3 h = normalize (lightDir + viewDir);

            half diff = max (0, dot (s.Normal, lightDir));

            float nh = max (0, dot (s.Normal, h));
			float spec = tex2D(_SpecRamp, float2(nh, 0.5)).a;

            half4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.Specular * s.Specularar) * atten;
            c.a = s.Alpha;
            return c;
        }

        void surf (Input IN, inout SurfaceOutputCustom o)
        {
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            half4 texColor = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = _Color * texColor.rgb;
            o.Normal = UnpackNormal(tex2D (_NormalTex, IN.uv_MainTex));
            o.Specular = _Shininess * tex2D(_SpecTex, IN.uv_MainTex).r;
            o.Specularar = tex2D (_LightPatternTex, screenUV).rgb;
            o.Alpha = texColor.a;
        }
        ENDCG
    }
}