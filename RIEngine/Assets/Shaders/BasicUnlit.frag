#version 330 core

struct PointLight
{
    vec3 position;
    vec3 color;
    float range;
};
uniform PointLight pointLights[4];

struct DirectionalLight
{
    vec3 direction;
    vec3 color;
};
uniform DirectionalLight directionalLight;

struct PixelInfo
{
    float ambient;
    float diffuse;
    float specular;
};

uniform float ambient;

in vec2 texCoord;
in vec3 norm;
in vec3 fragPos;

out vec4 fragColor;

uniform sampler2D texture1;

float saturate(float a)
{
    return clamp(a, 0.0, 1.0);
}

void main()
{
    vec3 normal = normalize(norm);
    vec4 baseColor = texture(texture1, texCoord);
    vec3 lightDir = normalize(-directionalLight.direction);
    float diffuse = max(dot(lightDir, normal), 0.0);
    
    vec4 result = baseColor * 0.1f;
    result += (diffuse + 0.1) * vec4(baseColor.rgb * directionalLight.color, baseColor.a);
    
    for (int i = 0; i < 4; i++)
    {
        PointLight pl = pointLights[i];
        vec3 lightDir = normalize(-pl.position + fragPos);
        float distance = length(fragPos - pl.position);
        
        float x = (distance * distance) / (pl.range * pl.range);
        float s = saturate(1 - x * x);
        float attenuation = s * s;
        float diffuse = max(dot(-lightDir, normal), 0.0);
        
        result += (attenuation * diffuse) * vec4(baseColor.rgb * pl.color.rgb, baseColor.a);
    }
    
    fragColor = result;
}