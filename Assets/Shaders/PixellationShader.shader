Shader "Custom/PixellationShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_PixSize("PixelSize", Int) = 5
		_PixAlphaCutoff("PixelAlphaCutoff", Float) = 0.2
		[MaterialToggle] _UseSetColor("UseSetColor", Int) = 0
    }
    SubShader
    {
		Tags{"Queue" = "Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
                return o;
            }

            sampler2D _MainTex;
			int _PixSize;
			float _PixAlphaCutoff;
			int _UseSetColor;

			float4 _MainTex_TexelSize;

            fixed4 frag (v2f i) : SV_Target
            {
				// Transform from 0 - 1 to pixel coordinates
				int coorX = i.uv.x * _MainTex_TexelSize.z;
				int coorY = i.uv.y * _MainTex_TexelSize.w;
				// Get the modulo remainder for x and y
				int moduloX = coorX % _PixSize;
				int moduloY = coorY % _PixSize;
				/*
				// Transform back into scaled coordinates
				float scaledModuloX = moduloX / _MainTex_TexelSize.z;
				float scaledModuloY = moduloY / _MainTex_TexelSize.w;
				*/
				// Get big pixel left down pixel pos
				int leftCoor = coorX - moduloX;
				int downCoor = coorY - moduloY;
				// Get big pixel right top pixel pos
				int rightCoor = leftCoor + _PixSize;
				int topCoor = downCoor + _PixSize;

				float4 col = float4(0, 0, 0, 0);
				UNITY_LOOP
				for (int x = leftCoor; x < rightCoor; x++) {
					UNITY_LOOP
					for (int y = downCoor; y < topCoor; y++) {
						col += tex2D(_MainTex, float2(float(x / _MainTex_TexelSize.z), float(y / _MainTex_TexelSize.w)));
					}
				}

				col /= _PixSize * _PixSize;
				if (col.a > _PixAlphaCutoff) {
					col.a = 1;
				}
				else {
					col.a = 0;
				}

				if (_UseSetColor == 1) {
					return col * col.a * i.color;
				}

				return col * col.a;
            }
            ENDCG
        }
    }
}
