Shader "SimplestInstancedShader"
{
    Properties
    {
        _Color1 ("Color", Color) = (1, 1, 1, 1)
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
            #pragma multi_compile_instancing
			#pragma instancing_options procedural:setup
            #include "UnityCG.cginc"
			#include "UnityStandardCore.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			float4 _Color1;

			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				StructuredBuffer<float4> objectPositionsBuffer;
			#endif

			void setup()
			{
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				//unity_ObjectToWorld = objectToWorldBuffer[unity_InstanceID];
				//unity_WorldToObject = unity_ObjectToWorld;

				// Construct an identity matrix
				unity_ObjectToWorld = float4x4(1, 0, 0, 0,
					0, 1, 0, 0,
					0, 0, 1, 0,
					0, 0, 0, 1);
				unity_WorldToObject = unity_ObjectToWorld;

				unity_WorldToObject._14_24_34 *= -1;
				unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
	#endif
			}

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
				 v2f o;
				UNITY_TRANSFER_INSTANCE_ID(v, o); 
              
			  	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
					float3 posW = v.vertex.xyz + objectPositionsBuffer[unity_InstanceID].xyz;
					float4 posWorld = float4(posW.x, posW.y, posW.z, v.vertex.w);
					o.vertex = UnityObjectToClipPos(posWorld);
				#else
					o.vertex = UnityObjectToClipPos(v.vertex);
				#endif
                return o;
            }
           
            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i); 
				return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}