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


Box box = new Box(100, 100, 240, 50, color);
boxList.Add(box);

Box box2 = new Box(100, 100, 400, 50, color);
boxList.Add(box2);

Box box3 = new Box(300, 100, 320, 100, color);
boxList.Add(box3);

//Box box4 = new Box(50, 50, 345, 400, color);
//boxList.Add(box4);


Game _game = new Game(title, height, width, boxList, syncMode, winState, border);
_game.Run();