uniform vec4 color;
uniform float expand;
uniform vec2 center;
uniform float radius;
uniform float windowHeight;

void main()
{
	vec2 centerFromSfml = vec2(center.x, windowHeight - center.y);
	vec2 p = (gl_FragCoord.xy - centerFromSfml) / radius;
	float r = sqrt(dot(p, p));
	if (r < 1.0)
	{
		gl_FragColor = mix(color, gl_Color, (r - expand) / (1 - expand));
	}
	else
	{
		gl_FragColor = gl_Color;
	}
}