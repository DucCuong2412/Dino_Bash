‚:Shader "Sprites/Colorize" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" {}
 _Color ("Tint", Color) = (1,1,1,1)
 MinLevel ("MinLevel", Float) = 0
 MaxLevel ("MaxLevel", Float) = 1
 R_Weight ("R_Weight", Range(0,1)) = 0.2126
 G_Weight ("G_Weight", Range(0,1)) = 0.7152
 B_Weight ("B_Weight", Range(0,1)) = 0.0722
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
 EffectFade ("EffectFade", Float) = 1
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
uniform lowp vec4 _Color;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  tmpvar_3 = tmpvar_1;
  highp vec4 tmpvar_4;
  tmpvar_4 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_4;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform lowp vec4 _Color;
uniform lowp float R_Weight;
uniform lowp float G_Weight;
uniform lowp float B_Weight;
uniform sampler2D _MainTex;
uniform lowp float MinLevel;
uniform lowp float MaxLevel;
uniform lowp float EffectFade;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 oColor_2;
  highp vec4 texel_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  texel_3 = tmpvar_4;
  oColor_2.xyz = mix (texel_3, ((
    ((((R_Weight * texel_3.x) + (G_Weight * texel_3.y)) + (B_Weight * texel_3.z)) * (MaxLevel - MinLevel))
   + MinLevel) * xlv_COLOR), vec4(EffectFade)).xyz;
  oColor_2.w = (texel_3.w * _Color.w);
  tmpvar_1 = oColor_2;
  gl_FragData[0] = tmpvar_1;
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
uniform lowp vec4 _Color;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  tmpvar_3 = tmpvar_1;
  highp vec4 tmpvar_4;
  tmpvar_4 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_4;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform lowp vec4 _Color;
uniform lowp float R_Weight;
uniform lowp float G_Weight;
uniform lowp float B_Weight;
uniform sampler2D _MainTex;
uniform lowp float MinLevel;
uniform lowp float MaxLevel;
uniform lowp float EffectFade;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 oColor_2;
  highp vec4 texel_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture (_MainTex, xlv_TEXCOORD0);
  texel_3 = tmpvar_4;
  oColor_2.xyz = mix (texel_3, ((
    ((((R_Weight * texel_3.x) + (G_Weight * texel_3.y)) + (B_Weight * texel_3.z)) * (MaxLevel - MinLevel))
   + MinLevel) * xlv_COLOR), vec4(EffectFade)).xyz;
  oColor_2.w = (texel_3.w * _Color.w);
  tmpvar_1 = oColor_2;
  _glesFragData[0] = tmpvar_1;
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
uniform lowp vec4 _Color;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_3 = tmpvar_1;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_5;
  highp vec4 pos_6;
  pos_6.zw = tmpvar_4.zw;
  highp vec2 tmpvar_7;
  tmpvar_7 = (_ScreenParams.xy * 0.5);
  pos_6.xy = ((floor(
    (((tmpvar_4.xy / tmpvar_4.w) * tmpvar_7) + vec2(0.5, 0.5))
  ) / tmpvar_7) * tmpvar_4.w);
  gl_Position = pos_6;
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform lowp vec4 _Color;
uniform lowp float R_Weight;
uniform lowp float G_Weight;
uniform lowp float B_Weight;
uniform sampler2D _MainTex;
uniform lowp float MinLevel;
uniform lowp float MaxLevel;
uniform lowp float EffectFade;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 oColor_2;
  highp vec4 texel_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD0);
  texel_3 = tmpvar_4;
  oColor_2.xyz = mix (texel_3, ((
    ((((R_Weight * texel_3.x) + (G_Weight * texel_3.y)) + (B_Weight * texel_3.z)) * (MaxLevel - MinLevel))
   + MinLevel) * xlv_COLOR), vec4(EffectFade)).xyz;
  oColor_2.w = (texel_3.w * _Color.w);
  tmpvar_1 = oColor_2;
  gl_FragData[0] = tmpvar_1;
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
uniform lowp vec4 _Color;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_3 = tmpvar_1;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_5;
  highp vec4 pos_6;
  pos_6.zw = tmpvar_4.zw;
  highp vec2 tmpvar_7;
  tmpvar_7 = (_ScreenParams.xy * 0.5);
  pos_6.xy = ((floor(
    (((tmpvar_4.xy / tmpvar_4.w) * tmpvar_7) + vec2(0.5, 0.5))
  ) / tmpvar_7) * tmpvar_4.w);
  gl_Position = pos_6;
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform lowp vec4 _Color;
uniform lowp float R_Weight;
uniform lowp float G_Weight;
uniform lowp float B_Weight;
uniform sampler2D _MainTex;
uniform lowp float MinLevel;
uniform lowp float MaxLevel;
uniform lowp float EffectFade;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 oColor_2;
  highp vec4 texel_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture (_MainTex, xlv_TEXCOORD0);
  texel_3 = tmpvar_4;
  oColor_2.xyz = mix (texel_3, ((
    ((((R_Weight * texel_3.x) + (G_Weight * texel_3.y)) + (B_Weight * texel_3.z)) * (MaxLevel - MinLevel))
   + MinLevel) * xlv_COLOR), vec4(EffectFade)).xyz;
  oColor_2.w = (texel_3.w * _Color.w);
  tmpvar_1 = oColor_2;
  _glesFragData[0] = tmpvar_1;
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