using System;
using System.Runtime.InteropServices;

namespace ArmsFW.Services.UI
{
    public class WinApi
    {
        [DllImport("Gdi32.dll", EntryPoint ="CreateRoundRectRgn")]
        public static extern IntPtr CriarRetangulo(int v1, int v2, int width, int heigth, int cantos1, int cantos2);
    }
}