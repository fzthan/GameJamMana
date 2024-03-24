shader_type spatial;

uniform float time_scale : hint_range(0.0, 10.0, 0.25) = 1.0;
uniform float uv_scale : hint_range(0.0, 10.0, 0.25) = 1.0;
uniform float normal : hint_range(0.0, 1.0, 0.1) = 0.5;
uniform float roughness : hint_range(0.0, 1.0, 0.1) = 0.5;

uniform vec3 water_color : source_color;

uniform vec2 abberation_r;
uniform vec2 abberation_g;
uniform vec2 abberation_b;

uniform float highlights_time_scale = 1.0;

uniform sampler2D sun_highlights;
uniform sampler2D water_normal_noise;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

vec3 soft_light(vec3 base, vec3 blend){
	vec3 limit = step(0.5, blend);
	return mix(2.0 * base * blend + base *base *(1.0 - 2.0 * blend),
	 sqrt(base) * (2.0 * blend - 1.0) + (2.0 * base) * (1.0 - blend), limit);
}


void fragment() {
	vec2 _uv = UV * uv_scale;
	vec2 _screen_uv = SCREEN_UV;
	
	_uv.x += sin(TIME * time_scale + (_uv.x + _uv.y) * 25.0) * 0.01;
	_uv.y += cos(TIME * time_scale + (_uv.x - _uv.y) * 25.0) * 0.01;
	
	_screen_uv.x += sin(TIME * time_scale + (_screen_uv.x + _screen_uv.y) * 25.0) * 0.01;
	_screen_uv.y += cos(TIME * time_scale + (_screen_uv.x - _screen_uv.y) * 25.0) * 0.01;
	
	float r = texture(sun_highlights, _uv + abberation_r + TIME * highlights_time_scale).r;
	float g = texture(sun_highlights, _uv + abberation_g + TIME * highlights_time_scale).g;
	float b = texture(sun_highlights, _uv + abberation_b + TIME * highlights_time_scale).b;
	
	vec3 blended = soft_light(texture(screen_texture, _screen_uv).rgb, vec3(r,g,b));
	
	ALBEDO = blended;
	NORMAL_MAP = texture(water_normal_noise, _uv).rgb;
	NORMAL *= normal;
	ROUGHNESS = roughness;
	
}