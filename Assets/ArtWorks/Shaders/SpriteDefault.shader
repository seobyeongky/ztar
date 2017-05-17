Shader "Ztar/SpriteDefault"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_ColorAddToTint("Color add to tint", Color) = (0,0,0,0)
		_Freezed("Freezed", Range(0,1)) = 0
		_FlipX("Flip X", Float) = 1
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			fixed4 _Color;
			fixed4 _ColorAddToTint;
			half4 _MainTex_ST;
			fixed _Freezed;
			fixed _FlipX;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				IN.vertex.x *= sign(_FlipX + 0.1);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				OUT.color.rgb += _ColorAddToTint.rgb * _ColorAddToTint.a * 2;

				OUT.vertex = UnityPixelSnap(OUT.vertex);
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				return color;
			}

			fixed WhiteOrBlack(float3 c){
				c.rgb * float3(0.3, 0.58, 0.11);
				return round(0.3 * (c.r+c.g+c.b));
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				fixed4 bbb = 1.2-WhiteOrBlack(c);
				c.rgb = lerp(c.rgb, dot(c.rgb, (float3(0.3, 0.58, 0.11)) + float3(0.2,0.2,0.2))+float3(0.0,0.1,0.9)*bbb, _Freezed);
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}
