×AShader "Pokoko/mega_ball" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" {}
 _ShadowTex ("Shadow Texture", 2D) = "white" {}
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
 _Rotation ("Rotation", Float) = 0
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="False" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="False" }
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
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Rotation;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  mediump vec2 rot_3;
  lowp vec4 tmpvar_4;
  mediump vec2 tmpvar_5;
  tmpvar_5 = tmpvar_2;
  highp float tmpvar_6;
  highp float cse_7;
  cse_7 = (_Rotation * _Time.x);
  highp float cse_8;
  cse_8 = (_glesMultiTexCoord0.y - 0.5);
  highp float cse_9;
  cse_9 = (_glesMultiTexCoord0.x - 0.5);
  tmpvar_6 = (((cse_9 * 
    cos(cse_7)
  ) - (cse_8 * 
    sin(cse_7)
  )) + 0.5);
  rot_3.x = tmpvar_6;
  highp float tmpvar_10;
  tmpvar_10 = (((cse_8 * 
    cos(cse_7)
  ) + (cse_9 * 
    sin(cse_7)
  )) + 0.5);
  rot_3.y = tmpvar_10;
  tmpvar_4 = tmpvar_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_4;
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = rot_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _ShadowTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_ShadowTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD1);
  lowp vec4 tmpvar_3;
  tmpvar_3.xyz = (tmpvar_2.xyz * mix (vec3(1.0, 1.0, 1.0), tmpvar_1.xyz, tmpvar_1.www));
  tmpvar_3.w = (tmpvar_2.w * xlv_COLOR.w);
  gl_FragData[0] = tmpvar_3;
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
uniform highp float _Rotation;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  mediump vec2 rot_3;
  lowp vec4 tmpvar_4;
  mediump vec2 tmpvar_5;
  tmpvar_5 = tmpvar_2;
  highp float tmpvar_6;
  highp float cse_7;
  cse_7 = (_Rotation * _Time.x);
  highp float cse_8;
  cse_8 = (_glesMultiTexCoord0.y - 0.5);
  highp float cse_9;
  cse_9 = (_glesMultiTexCoord0.x - 0.5);
  tmpvar_6 = (((cse_9 * 
    cos(cse_7)
  ) - (cse_8 * 
    sin(cse_7)
  )) + 0.5);
  rot_3.x = tmpvar_6;
  highp float tmpvar_10;
  tmpvar_10 = (((cse_8 * 
    cos(cse_7)
  ) + (cse_9 * 
    sin(cse_7)
  )) + 0.5);
  rot_3.y = tmpvar_10;
  tmpvar_4 = tmpvar_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_4;
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = rot_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _ShadowTex;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture (_ShadowTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_MainTex, xlv_TEXCOORD1);
  lowp vec4 tmpvar_3;
  tmpvar_3.xyz = (tmpvar_2.xyz * mix (vec3(1.0, 1.0, 1.0), tmpvar_1.xyz, tmpvar_1.www));
  tmpvar_3.w = (tmpvar_2.w * xlv_COLOR.w);
  _glesFragData[0] = tmpvar_3;
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
uniform highp float _Rotation;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  mediump vec2 rot_3;
  lowp vec4 tmpvar_4;
  mediump vec2 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_5 = tmpvar_2;
  highp float tmpvar_7;
  highp float cse_8;
  cse_8 = (_Rotation * _Time.x);
  highp float cse_9;
  cse_9 = (_glesMultiTexCoord0.y - 0.5);
  highp float cse_10;
  cse_10 = (_glesMultiTexCoord0.x - 0.5);
  tmpvar_7 = (((cse_10 * 
    cos(cse_8)
  ) - (cse_9 * 
    sin(cse_8)
  )) + 0.5);
  rot_3.x = tmpvar_7;
  highp float tmpvar_11;
  tmpvar_11 = (((cse_9 * 
    cos(cse_8)
  ) + (cse_10 * 
    sin(cse_8)
  )) + 0.5);
  rot_3.y = tmpvar_11;
  tmpvar_4 = tmpvar_1;
  highp vec4 pos_12;
  pos_12.zw = tmpvar_6.zw;
  highp vec2 tmpvar_13;
  tmpvar_13 = (_ScreenParams.xy * 0.5);
  pos_12.xy = ((floor(
    (((tmpvar_6.xy / tmpvar_6.w) * tmpvar_13) + vec2(0.5, 0.5))
  ) / tmpvar_13) * tmpvar_6.w);
  gl_Position = pos_12;
  xlv_COLOR = tmpvar_4;
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = rot_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _ShadowTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture2D (_ShadowTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD1);
  lowp vec4 tmpvar_3;
  tmpvar_3.xyz = (tmpvar_2.xyz * mix (vec3(1.0, 1.0, 1.0), tmpvar_1.xyz, tmpvar_1.www));
  tmpvar_3.w = (tmpvar_2.w * xlv_COLOR.w);
  gl_FragData[0] = tmpvar_3;
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
uniform highp float _Rotation;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec2 tmpvar_2;
  tmpvar_2 = _glesMultiTexCoord0.xy;
  mediump vec2 rot_3;
  lowp vec4 tmpvar_4;
  mediump vec2 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_5 = tmpvar_2;
  highp float tmpvar_7;
  highp float cse_8;
  cse_8 = (_Rotation * _Time.x);
  highp float cse_9;
  cse_9 = (_glesMultiTexCoord0.y - 0.5);
  highp float cse_10;
  cse_10 = (_glesMultiTexCoord0.x - 0.5);
  tmpvar_7 = (((cse_10 * 
    cos(cse_8)
  ) - (cse_9 * 
    sin(cse_8)
  )) + 0.5);
  rot_3.x = tmpvar_7;
  highp float tmpvar_11;
  tmpvar_11 = (((cse_9 * 
    cos(cse_8)
  ) + (cse_10 * 
    sin(cse_8)
  )) + 0.5);
  rot_3.y = tmpvar_11;
  tmpvar_4 = tmpvar_1;
  highp vec4 pos_12;
  pos_12.zw = tmpvar_6.zw;
  highp vec2 tmpvar_13;
  tmpvar_13 = (_ScreenParams.xy * 0.5);
  pos_12.xy = ((floor(
    (((tmpvar_6.xy / tmpvar_6.w) * tmpvar_13) + vec2(0.5, 0.5))
  ) / tmpvar_13) * tmpvar_6.w);
  gl_Position = pos_12;
  xlv_COLOR = tmpvar_4;
  xlv_TEXCOORD0 = tmpvar_5;
  xlv_TEXCOORD1 = rot_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _ShadowTex;
in lowp vec4 xlv_COLOR;
in mediump vec2 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = texture (_ShadowTex, xlv_TEXCOORD0);
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_MainTex, xlv_TEXCOORD1);
  lowp vec4 tmpvar_3;
  tmpvar_3.xyz = (tmpvar_2.xyz * mix (vec3(1.0, 1.0, 1.0), tmpvar_1.xyz, tmpvar_1.www));
  tmpvar_3.w = (tmpvar_2.w * xlv_COLOR.w);
  _glesFragData[0] = tmpvar_3;
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