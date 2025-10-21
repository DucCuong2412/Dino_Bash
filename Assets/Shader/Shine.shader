ð<Shader "Pokoko/Shine" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" {}
 _Color ("Tint", Color) = (1,1,1,1)
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
 _Speed ("Speed", Float) = 0
 _LayerOffset ("LayerOffset", Float) = 0
 _LayerOpacity ("LayerOpacity", Range(0,1)) = 1
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Opaque" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Opaque" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "gles " {
Keywords { "DUMMY" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
uniform lowp float _Speed;
uniform lowp float _LayerOffset;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5.y = 0.0;
  tmpvar_5.x = (fract(_Time.x) * _Speed);
  tmpvar_3 = tmpvar_1;
  highp vec2 tmpvar_6;
  tmpvar_6 = (_glesMultiTexCoord0.xy + (tmpvar_5 * _LayerOffset));
  tmpvar_4 = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_7;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

uniform lowp float _LayerOpacity;
uniform sampler2D _MainTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 result_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  lowp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 * mix (1.0, (texture2D (_MainTex, xlv_TEXCOORD1) * xlv_COLOR).x, _LayerOpacity));
  result_1.w = tmpvar_3.w;
  result_1.xyz = (tmpvar_3.xyz * tmpvar_2.w);
  gl_FragData[0] = result_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DUMMY" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
uniform lowp float _Speed;
uniform lowp float _LayerOffset;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5.y = 0.0;
  tmpvar_5.x = (fract(_Time.x) * _Speed);
  tmpvar_3 = tmpvar_1;
  highp vec2 tmpvar_6;
  tmpvar_6 = (_glesMultiTexCoord0.xy + (tmpvar_5 * _LayerOffset));
  tmpvar_4 = tmpvar_6;
  highp vec4 tmpvar_7;
  tmpvar_7 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_7;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform lowp float _LayerOpacity;
uniform sampler2D _MainTex;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 result_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (texture (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  lowp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 * mix (1.0, (texture (_MainTex, xlv_TEXCOORD1) * xlv_COLOR).x, _LayerOpacity));
  result_1.w = tmpvar_3.w;
  result_1.xyz = (tmpvar_3.xyz * tmpvar_2.w);
  _glesFragData[0] = result_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "PIXELSNAP_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 _ScreenParams;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
uniform lowp float _Speed;
uniform lowp float _LayerOffset;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_6;
  tmpvar_6.y = 0.0;
  tmpvar_6.x = (fract(_Time.x) * _Speed);
  tmpvar_3 = tmpvar_1;
  highp vec2 tmpvar_7;
  tmpvar_7 = (_glesMultiTexCoord0.xy + (tmpvar_6 * _LayerOffset));
  tmpvar_4 = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_8;
  highp vec4 pos_9;
  pos_9.zw = tmpvar_5.zw;
  highp vec2 tmpvar_10;
  tmpvar_10 = (_ScreenParams.xy * 0.5);
  pos_9.xy = ((floor(
    (((tmpvar_5.xy / tmpvar_5.w) * tmpvar_10) + vec2(0.5, 0.5))
  ) / tmpvar_10) * tmpvar_5.w);
  gl_Position = pos_9;
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

uniform lowp float _LayerOpacity;
uniform sampler2D _MainTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 result_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  lowp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 * mix (1.0, (texture2D (_MainTex, xlv_TEXCOORD1) * xlv_COLOR).x, _LayerOpacity));
  result_1.w = tmpvar_3.w;
  result_1.xyz = (tmpvar_3.xyz * tmpvar_2.w);
  gl_FragData[0] = result_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "PIXELSNAP_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 _ScreenParams;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
uniform lowp float _Speed;
uniform lowp float _LayerOffset;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_6;
  tmpvar_6.y = 0.0;
  tmpvar_6.x = (fract(_Time.x) * _Speed);
  tmpvar_3 = tmpvar_1;
  highp vec2 tmpvar_7;
  tmpvar_7 = (_glesMultiTexCoord0.xy + (tmpvar_6 * _LayerOffset));
  tmpvar_4 = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_8;
  highp vec4 pos_9;
  pos_9.zw = tmpvar_5.zw;
  highp vec2 tmpvar_10;
  tmpvar_10 = (_ScreenParams.xy * 0.5);
  pos_9.xy = ((floor(
    (((tmpvar_5.xy / tmpvar_5.w) * tmpvar_10) + vec2(0.5, 0.5))
  ) / tmpvar_10) * tmpvar_5.w);
  gl_Position = pos_9;
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform lowp float _LayerOpacity;
uniform sampler2D _MainTex;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 result_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (texture (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  lowp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_2 * mix (1.0, (texture (_MainTex, xlv_TEXCOORD1) * xlv_COLOR).x, _LayerOpacity));
  result_1.w = tmpvar_3.w;
  result_1.xyz = (tmpvar_3.xyz * tmpvar_2.w);
  _glesFragData[0] = result_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "DUMMY" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DUMMY" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "PIXELSNAP_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "PIXELSNAP_ON" }
"!!GLES3"
}
}
 }
}
}