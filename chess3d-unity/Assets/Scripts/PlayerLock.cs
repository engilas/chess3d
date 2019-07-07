using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PlayerLock
{
    public static bool GameLock { get; set; }
    public static bool CameraLock { get; set; }
    public static bool MenuLock { get; set; }

    public static bool IsLocked => GameLock || CameraLock || MenuLock;
}
