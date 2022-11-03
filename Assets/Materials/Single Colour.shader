Shader "Unlit/Player" {

    Properties {

	_pColour ("Player Colour", Color) = (1, 1, 1, 1)

    }

    SubShader {
        Pass {
			CGPROGRAM

			half4 _pColour;

			#pragma vertex vert             
			#pragma fragment frag 

			struct VSInput {
     			float4 pos : POSITION;
			};  
 
			struct VSOutput {
     			float4 pos : SV_POSITION;
			}; 

			VSOutput vert(VSInput input) {
     			VSOutput output;
      			output.pos = UnityObjectToClipPos(input.pos);
				return output;
			}
 
			float4 frag (VSOutput output) : COLOR {
				return _pColour;
			} 

            ENDCG
        }
    }
} 