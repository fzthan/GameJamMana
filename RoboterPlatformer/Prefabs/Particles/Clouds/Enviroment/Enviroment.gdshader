shader_type spatial;

uniform float normal : hint_range(0.0, 10.0, 0.1);
uniform float roughness : hint_range(0.0, 10.0, 0.1);
uniform float time_scale : hint_range(0.0, 10.0, 0.1);
uniform float uv_scale : hint_range(0.0, 1000.0, 1) = 1.0;

uniform vec3 water_color : source_color;
uniform sampler2D water_normal_noise;

void fragment() {
	
	vec2 _uv = UV * uv_scale;
	
	_uv.x += sin(TIME * time_scale + (_uv.x + _uv.y) * 25.0) * 0.01;
	_uv.y += cos(TIME * time_scale + (_uv.x - _uv.y) * 25.0) * 0.01;
	
	ALBEDO = water_color;
	NORMAL_MAP = texture(water_normal_noise, _uv).rgb;
	NORMAL *= normal;
	ROUGHNESS *= roughness;
	
}
