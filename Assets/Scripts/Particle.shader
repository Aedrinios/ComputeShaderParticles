Shader "Custom/Particle" {

	SubShader {
		Pass {
		Tags{ "RenderType" = "Transparent" }
		LOD 200
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag
		#include "UnityCG.cginc"
		#pragma target 5.0

		struct Particle{
			float3 position;
			float3 velocity;
			float life;
		};
		
		struct v2g{
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float life : LIFE;
		};

		struct g2f
		{
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float life : LIFE;
		};
		
		StructuredBuffer<Particle> particleBuffer;
		
		v2g vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
		{
			v2g o = (v2g)0;
			float life = particleBuffer[instance_id].life / 4.0f;
			float intensity = 1.5f;
			o.color = fixed4((1.0f - life) * intensity, life * intensity, 1.0f * intensity, 1.0f) * life;
			o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));
			return o;
		}
		
		[maxvertexcount(4)] // on génère un triangle strip de 4 points
		void geom(point v2g IN[1], inout TriangleStream<g2f> triStream) {
			g2f p;
			float s = 0.05;
			p.life = IN[0].life;
			p.color = IN[0].color;
			p.position = IN[0].position + float4(-s, -s *2, 0, 0);//top left
			triStream.Append(p);
			p.position = IN[0].position + float4(s, -s *2, 0, 0);//top right
			triStream.Append(p);
			p.position = IN[0].position + float4(-s, s *2, 0, 0);//bot left
			triStream.Append(p);
			p.position = IN[0].position + float4(s, s *2, 0, 0);//bot left
			triStream.Append(p);
			triStream.RestartStrip();
		}

		float4 frag(g2f i) : COLOR
		{
			return i.color;
		}


		ENDCG
		}
	}
	FallBack Off
}