#version 330 core
out vec4 outColor;

in VertexData {
    vec2 UV;
} i;

uniform sampler2D uColor;
uniform sampler2D uDepth;
uniform vec2 uResolution;
uniform vec2 uTexel;

uniform float uFocalLength; //focal length in mm
uniform float uFstop; //f-stop value https://en.wikipedia.org/wiki/F-number
uniform bool uDebug;
uniform bool uEnableVignetting;

// TODO: might chnage but shouldnt
const float Z_NEAR = 0.1; //camera clipping start
const float Z_FAR = 100.0; //camera clipping end

const int samples = 3; //samples on the first ring
const int rings = 3; //ring count

const float PI = 3.14159265359;
const vec2 FOCUS = vec2(0.5,0.5); 
const float CoC = 0.03;//circle of confusion size in mm (35mm film = 0.03mm)
const float MAX_BLUR = 1;
const float N_AMOUNT = 0.0001; //dither amount

const float THRESHOLD = 0.5; //highlight threshold;
const float GAIN = 2.0; //highlight gain;

const float BIAS = 0.5; //bokeh edge bias
const float FRINGE = 0.7; //bokeh chromatic aberration/fringing

// Vignetting
const float VIGN_OUT = 0.5; //vignetting outer border
const float VIGN_IN = 0.1; //vignetting inner border
const float VIGN_FADE = 30.0; //f-stops till vignete fades

//Chromatic aberration
const float CHR_AB_INTENSITY = 0.002; //The smaller the less intense

float linearize(float depth);
vec2 rand(vec2 coord); //generating noise/pattern texture for dithering
vec3 preprocess(vec2 coords,float blur);
vec3 debugFocus(vec3 col, float blur, float depth);
float vignette();
vec3 chromaticAberration();

void main() {
    float depth = linearize(texture2D(uDepth, i.UV).x);
    float fDepth = linearize(texture2D(uDepth, FOCUS).x);
    
    float f = uFocalLength; //focal length in mm
    float d = fDepth*1000.0; //focal plane in mm
    float o = depth*1000.0; //depth in mm
    
    float a = (o*f)/(o-f); 
    float b = (d*f)/(d-f); 
    float c = (d-f)/(d*uFstop*CoC); 
    
    float blur = abs(a-b)*c;
    blur = clamp(blur,0.0,1.0);
    
    vec2 noise = rand(i.UV) * N_AMOUNT * blur;
    // Blur x and y step factor
    float w = (1.0 / uResolution.x) * blur * MAX_BLUR + noise.x;
    float h = (1.0 / uResolution.y) * blur * MAX_BLUR + noise.y;

	vec3 color = chromaticAberration();
    if(blur > 0.05) {
        float s = 1.0;
        int ringsamples;
        
        for (int j = 1; j <= rings; j += 1) {   
            ringsamples = j * samples;
            
            for (int j = 0 ; j < ringsamples ; j += 1) {
                float step = PI*2.0 / float(ringsamples);
                float pw = (cos(float(j)*step)*float(j));
                float ph = (sin(float(j)*step)*float(j));
                float p = 1.0;
                color += preprocess(i.UV + vec2(pw*w,ph*h),blur)*mix(1.0,(float(j))/(float(rings)),BIAS)*p;  
                s += 1.0*mix(1.0,(float(j))/(float(rings)),BIAS)*p;
            }
        }
        color /= s; //divide by sample count
    }
    
    if (uDebug)  {
        color = debugFocus(color, blur, depth);
    }
    
    if (uEnableVignetting) {
        color *= vignette();
    }
    
    outColor = vec4(color, 1);
}

float vignette() {
	float dist = distance(i.UV, vec2(0.5,0.5));
	dist = smoothstep(VIGN_OUT + (uFstop/VIGN_FADE), VIGN_IN + (uFstop/VIGN_FADE), dist);
	return clamp(dist,0.0,1.0);
}

vec3 chromaticAberration(){
	vec2 dist = (i.UV - vec2(0.5,0.5)) * CHR_AB_INTENSITY;
	float red = texture2D(uColor, i.UV + dist).r;
	float green = texture2D(uColor, i.UV).g;
	float blue = texture2D(uColor, i.UV - dist).b;
    return vec3(red, green, blue);
}

vec3 preprocess(vec2 coords,float blur) {
	vec3 color = vec3(0.0);
	
	color.r = texture2D(uColor, coords + vec2(0.0,1.0)*uTexel*FRINGE*blur).r;
	color.g = texture2D(uColor, coords + vec2(-0.866,-0.5)*uTexel*FRINGE*blur).g;
	color.b = texture2D(uColor, coords + vec2(0.866,-0.5)*uTexel*FRINGE*blur).b;
	
	vec3 lumcoeff = vec3(0.299,0.587,0.114);
	float lum = dot(color.rgb, lumcoeff);
	float thresh = max((lum-THRESHOLD)*GAIN, 0.0);
	return color+mix(vec3(0.0),color,thresh*blur);
}

vec2 rand(vec2 coord) {
	float noiseX = ((fract(1.0-coord.s*(uResolution.x/2.0))*0.25)+(fract(coord.t*(uResolution.y/2.0))*0.75))*2.0-1.0;
	float noiseY = ((fract(1.0-coord.s*(uResolution.x/2.0))*0.75)+(fract(coord.t*(uResolution.y/2.0))*0.25))*2.0-1.0;
	
    noiseX = clamp(fract(sin(dot(coord ,vec2(12.9898,78.233))) * 43758.5453),0.0,1.0)*2.0-1.0;
    noiseY = clamp(fract(sin(dot(coord ,vec2(12.9898,78.233)*2.0)) * 43758.5453),0.0,1.0)*2.0-1.0;
    
	return vec2(noiseX, noiseY);
}

float linearize(float depth) {
	return -Z_FAR * Z_NEAR / (depth * (Z_FAR - Z_NEAR) - Z_FAR);
}

vec3 debugFocus(vec3 col, float blur, float depth) {
	float edge = 0.002*depth; //distance based edge smoothing
	float m = clamp(smoothstep(0.0,edge,blur),0.0,1.0);
	float e = clamp(smoothstep(1.0-edge,1.0,blur),0.0,1.0);
	
	col = mix(col,vec3(1.0,0.5,0.0),(1.0-m)*0.6);
	col = mix(col,vec3(0.0,0.5,1.0),((1.0-e)-(1.0-m))*0.2);

	return col;
}