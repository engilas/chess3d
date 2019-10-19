namespace Assets.Scripts
{
    public static class PlayerLock
    {
        public static bool GameLock { get; set; }
        public static bool CameraLock { get; set; }
        public static bool MenuLock { get; set; }

        public static bool IsLocked => GameLock || CameraLock || MenuLock;
    }
}
