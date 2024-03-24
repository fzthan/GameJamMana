shader_type spatial;

uniform float time_scale : hint_range(0.0, 10.0, 0.25) = 1.0;
uniform float uv_scale : hint_range(0.0, 10.0, 0.25) = 1.0;

uniform vec3 water_color : source_color;

uniform vec2 abberation_r;
uniform vec2 abberation_g;
uniform vec2 abberation_b;


vec3 soft_light(vec3 base, vec3 blend){
	vec3 limit = step(0.5, blend);
	return mix(2.0 * base * blend + base *base *(1.0 - 2.0 * blend),
	 sqrt(base) * (2.0 * blend - 1.0) + (2.0 * base) * (1.0 - blend), limit);
}

uniform sampler2D sun_highlights;
uniform sampler2D water_normal_noise;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

void fragment() {
	vec2 _uv = UV * uv_scale;
	vec2 _screen_uv = SCREEN_UV;
	
	_uv.x += sin(TIME * time_scale + (_uv.x + _uv.y) * 25.0) * 0.01;
	_uv.y += cos(TIME * time_scale + (_uv.x - _uv.y) * 25.0) * 0.01;
	
	_screen_uv.x += sin(TIME * time_scale + (_screen_uv.x + _screen_uv.y) * 25.0) * 0.01;
	_screen_uv.y += cos(TIME * time_scale + (_screen_uv.x - _screen_uv.y) * 25.0) * 0.01;
	
	float r = texture(sun_highlights, _uv + abberation_r).r;
	float g = texture(sun_highlights, _uv + abberation_g).g;
	float b = texture(sun_highlights, _uv + abberation_b).b;
	
	vec3 blended = soft_light(texture(screen_texture, _screen_uv).rgb, vec3(r,g,b));
	
	ALBEDO = blended;
	NORMAL_MAP = texture(water_normal_noise, _uv).rgb;
	NORMAL *= 0.5;
	ROUGHNESS = 0.2;
	
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
