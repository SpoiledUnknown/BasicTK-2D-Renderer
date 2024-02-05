using OpenTK.Windowing.Common;
using OpenTKProject;

string title = "OpenTK Project";
int height = 480, width = 854;
VSyncMode syncMode = VSyncMode.On;
WindowState winState = WindowState.Normal;

Game _game = new Game(title,height,width,syncMode,winState);
_game.Run();