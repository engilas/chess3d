using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum GameMode
{
    OfflineAi,
    OfflineMp
}

public static class Settings
{
    public const int DefaultDifficulty = 3;

    public static int Difficulty { get; set; }

    public static GameMode GameMode { get; set; }
}
