Shader "tk2d/BlendVertexColor" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader { 
    LOD 110
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    
    Pass {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        
        GLSLPROGRAM
        
        #ifdef VERTEX
        attribute vec4 _glesVertex;
        attribute vec4 _glesColor;
        attribute vec4 _glesMultiTexCoord0;
        uniform highp mat4 glstate_matrix_mvp;
        varying lowp vec4 xlv_COLOR;
        varying highp vec2 xlv_TEXCOORD0;
        
        void main()
        {
            gl_Position = glstate_matrix_mvp * _glesVertex;
            xlv_COLOR = _glesColor;
            xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
        }
        #endif
        
        #ifdef FRAGMENT
        uniform sampler2D _MainTex;
        varying lowp vec4 xlv_COLOR;
        varying highp vec2 xlv_TEXCOORD0;
        
        void main()
        {
            gl_FragColor = texture2D(_MainTex, xlv_TEXCOORD0) * xlv_COLOR;
        }
        #endif
        
        ENDGLSL
    }
}

SubShader { 
    LOD 100
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
    
    Pass {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        
        BindChannels {
            Bind "vertex", Vertex
            Bind "color", Color
            Bind "texcoord", TexCoord
        }
        
        ZWrite Off
        Cull Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        
        SetTexture [_MainTex] { 
            combine texture * primary 
        }
    }
}

Fallback "Transparent/VertexLit"
}