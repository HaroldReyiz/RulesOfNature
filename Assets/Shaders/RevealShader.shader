Shader "Unlit/RevealShader"
{
	Properties
	{
		_MainTex( "Texture", 2D ) = "white" {}
		_Radius( "Radius", Range( 0.005, 0.1 ) ) = 0.005
		_PlayerXPos( "Player X Position", Float ) = 0.0
		_PlayerZPos( "Player Z Position", Float ) = 0.0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		// extra pass that renders to depth buffer only
		Pass 
		{
			ZWrite On
			ColorMask 0
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vertex
			#pragma fragment fragment
			
			#include "UnityCG.cginc"

			struct VertexIn
			{
				float4 vertex : POSITION;
				float2 uv 	  : TEXCOORD0;
			};

			struct VertexOut
			{
				float2 uv 	  : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 	  _MainTex_ST;
			float	  _Radius;
			float	  _PlayerXPos;
			float	  _PlayerZPos;
			
			VertexOut vertex( VertexIn vIn )
			{
				VertexOut vOut;
				vOut.vertex = UnityObjectToClipPos( vIn.vertex );
				vOut.uv     = TRANSFORM_TEX( vIn.uv, _MainTex );
				return vOut;
			}
			
			fixed4 fragment( VertexOut fIn ) : SV_Target
			{
				// Sample the texture.
				fixed4 col = tex2D( _MainTex, fIn.uv );

				if( col.a >= 0.002 ) // Do not factor transparent parts of the texture.
				{
					float dist = distance( fIn.uv, float2( _PlayerXPos, _PlayerZPos ) );
					col.a = max( 0.0, 1.0 - ( dist * dist ) / _Radius );
				}

				return col;
			}
			ENDCG
		}
	}
}
