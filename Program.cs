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

Box box = new Box(10, 20, 20, 50, Color4.Aqua);
boxList.Add(box);

Game _game = new Game(title, height, width, boxList, syncMode, winState, border);
_game.Run();