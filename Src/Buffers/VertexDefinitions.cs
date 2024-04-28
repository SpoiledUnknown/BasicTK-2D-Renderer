using OpenTK.Mathematics;

namespace BasicTK_2D_Renderer.Src.Buffers
{
    public readonly struct VertextAttribute
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int ComponentCount;
        public readonly int Offset;

        public VertextAttribute(string name, int index, int componentCount, int offset)
        {
            Name = name;
            Index = index;
            ComponentCount = componentCount;
            Offset = offset;
        }
    }

    public sealed class VertextInfo
    {
        public readonly Type Type;
        public readonly int SizeInBytes;
        public readonly VertextAttribute[] VertextAttributes;

        public VertextInfo(Type type, params VertextAttribute[] attributes)
        {
            Type = type;
            SizeInBytes = 0;

            VertextAttributes = attributes;

            for (int i = 0; i < VertextAttributes.Length; i++)
            {
                VertextAttribute attribute = VertextAttributes[i];
                SizeInBytes += attribute.ComponentCount * sizeof(float);
            }
        }
    }
    public readonly struct VertexPositionColor
    {
        public readonly Vector2 Position;
        public readonly Color4 Color;

        public static readonly VertextInfo VertextInfo = new VertextInfo(
            typeof(VertexPositionColor),
            new VertextAttribute("Position", 0, 2, 0),
            new VertextAttribute("Color", 1, 4, 2 * sizeof(float))
        );
        public VertexPositionColor(Vector2 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }

    public readonly struct VertexPositionTexture
    {
        public readonly Vector2 Position;
        public readonly Vector2 TexCoord;

        public static readonly VertextInfo vertextInfo = new VertextInfo(
            typeof(VertexPositionTexture),
            new VertextAttribute("Position", 0, 2, 0),
            new VertextAttribute("TextCoord", 1, 2, 2 * sizeof(float))
        );

        public VertexPositionTexture(Vector2 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }
    }
}