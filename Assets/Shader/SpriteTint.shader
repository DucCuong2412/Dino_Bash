Ø)Shader "Sprites/Tint" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" {}
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
Keywords { "DUMMY" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_3;
  mediump vec2 tmpvar_4;
  tmpvar_4 = tmpvar_2;
  tmpvar_3 = tmpvar_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_3;
  xlv_TEXCOORD0 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2.xyz = (tmpvar_1.xyz + ((2.0 * xlv_COLOR.xyz) - 1.0));
  tmpvar_2.w = (tmpvar_1.w * xlv_COLOR.w);
  gl_FragData[0] = tmpvar_2;
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
uniform highp mat4 glstate_matrix_mvp;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_3;
  mediump vec2 tmpvar_4;
  tmpvar_4 = tmpvar_2;
  tmpvar_3 = tmpvar_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_3;
  xlv_TEXCOORD0 = tmpvar_4;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2.xyz = (tmpvar_1.xyz + ((2.0 * xlv_COLOR.xyz) - 1.0));
  tmpvar_2.w = (tmpvar_1.w * xlv_COLOR.w);
  _glesFragData[0] = tmpvar_2;
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
uniform highp vec4 _ScreenParams;
uniform highp mat4 glstate_matrix_mvp;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_4 = tmpvar_2;
  tmpvar_3 = tmpvar_1;
  highp vec4 pos_6;
  pos_6.zw = tmpvar_5.zw;
  highp vec2 tmpvar_7;
  tmpvar_7 = (_ScreenParams.xy * 0.5);
  pos_6.xy = ((floor(
    (((tmpvar_5.xy / tmpvar_5.w) * tmpvar_7) + vec2(0.5, 0.5))
  ) / tmpvar_7) * tmpvar_5.w);
  gl_Position = pos_6;
  xlv_COLOR = tmpvar_3;
  xlv_TEXCOORD0 = tmpvar_4;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2.xyz = (tmpvar_1.xyz + ((2.0 * xlv_COLOR.xyz) - 1.0));
  tmpvar_2.w = (tmpvar_1.w * xlv_COLOR.w);
  gl_FragData[0] = tmpvar_2;
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
uniform highp vec4 _ScreenParams;
uniform highp mat4 glstate_matrix_mvp;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_3;
  mediump vec2 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_4 = tmpvar_2;
  tmpvar_3 = tmpvar_1;
  highp vec4 pos_6;
  pos_6.zw = tmpvar_5.zw;
  highp vec2 tmpvar_7;
  tmpvar_7 = (_ScreenParams.xy * 0.5);
  pos_6.xy = ((floor(
    (((tmpvar_5.xy / tmpvar_5.w) * tmpvar_7) + vec2(0.5, 0.5))
  ) / tmpvar_7) * tmpvar_5.w);
  gl_Position = pos_6;
  xlv_COLOR = tmpvar_3;
  xlv_TEXCOORD0 = tmpvar_4;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture (_MainTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2.xyz = (tmpvar_1.xyz + ((2.0 * xlv_COLOR.xyz) - 1.0));
  tmpvar_2.w = (tmpvar_1.w * xlv_COLOR.w);
  _glesFragData[0] = tmpvar_2;
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