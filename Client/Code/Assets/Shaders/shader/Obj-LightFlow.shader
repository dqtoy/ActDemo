﻿Shader "Shader/Obj-LightFlow"
{
	Properties
	{
		_Flow ("Light Flow", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Intensity("Intensity", Range(0.0, 10.0)) = 1.0
		_SpeedX("Speed X", Range(-1, 1)) = 0.5
		_SpeedY("Speed Y", Range(-1, 1)) = 0.5
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
		LOD 100
		Blend SrcAlpha One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Flow;
			float4 _Flow_ST;
			sampler2D _Mask;
			float4 _Mask_ST;
			float4 _Color;
			float _Intensity;
			float _SpeedX;
			float _SpeedY;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Flow) + float2(_SpeedX, _SpeedY) * _Time.y;
				o.uv1 = TRANSFORM_TEX(v.uv, _Mask);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_Flow, i.uv) * tex2D(_Mask, i.uv1).a * _Color * _Intensity;
			}
			ENDCG
		}
	}
}
