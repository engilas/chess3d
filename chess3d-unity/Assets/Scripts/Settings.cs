using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Engine;
using UnityEngine;

namespace Assets.Scripts
{
    public enum GameMode
    {
        OfflineAi,
        OfflineMp,

        OnlineAi,
        OnlineQuick,
    }

    public static class Settings
    {
        private const string DifficultyPrefKey = "DifficultyPrefKey";
        private const string FrontViewPrefKey = "FrontViewPrefKey";
        private const string DefaultServerIp = "http://localhost:5000";

        private static int _difficulty;

        public const int DefaultDifficulty = 3;

        public static int Difficulty
        {
            get
            {
                if (_difficulty != 0) return _difficulty;

                if (PlayerPrefs.HasKey(DifficultyPrefKey))
                {
                    var diff = PlayerPrefs.GetInt(DifficultyPrefKey);
                    _difficulty = diff;
                }
                else
                {
                    var diff = DefaultDifficulty;
                    Difficulty = diff;
                }
                return _difficulty;
            }
            set
            {
                PlayerPrefs.SetInt(DifficultyPrefKey, value);
                _difficulty = value;
            }
        }

        public static bool FrontView
        {
            get => PlayerPrefs.GetInt(FrontViewPrefKey) == 1;
            set => PlayerPrefs.SetInt(FrontViewPrefKey, value ? 1 : 0);
        }

        public static GameMode GameMode { get; set; }
        public static ChessPieceColor InitialCameraSide { get; set; }

        public static string ServerIp
        {
            get
            {
                var ip =  PlayerPrefs.GetString(nameof(ServerIp));
                return string.IsNullOrWhiteSpace(ip) ? DefaultServerIp : ip;
            }
            set => PlayerPrefs.SetString(nameof(ServerIp), value);
        }
    }
}