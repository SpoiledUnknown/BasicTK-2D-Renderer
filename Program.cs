using OpenTK.Windowing.Common;
using BasicTK_2D_Renderer;
using BasicTK_2D_Renderer.Src;
using OpenTK.Mathematics;


string title = "BasicTK - 2D Renderer";
int height = 480, width = 854;
VSyncMode syncMode = VSyncMode.On;
WindowState winState = WindowState.Maximized;
WindowBorder border = WindowBorder.Resizable;
List<Box> boxList = new List<Box>();

Color4 color = Color4.Brown;


Box box = CreateSprite(100, 100, 240, 50, color);
Box box1 = CreateSprite(100, 100, 400, 50, color);
Box box2 = CreateSprite(300, 100, 320, 100, color);

Game _game = new Game(title, height, width, boxList, syncMode, winState, border);
_game.Run();

Box CreateSprite(int height, int width, int x, int y, Color4 color)
{
    Box box = new Box(height, width, x, y, color);
    boxList.Add(box);
    return box;
}