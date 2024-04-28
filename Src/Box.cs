using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTK_2D_Renderer.Src
{
    public class Box
    {
        public int height;
        public int width;
        public int x;
        public int y;
        public Color4 color;

        public Box(int height, int width, int x, int y, Color4 color)
        {
            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            this.color = color;
        }
    }
}
