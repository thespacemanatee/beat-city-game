namespace MoreMountains.TopDownEngine
{
    /// <summary>
    ///     A simple static class that keeps track of layer names, holds ready to use layermasks for most common layers and
    ///     layermasks combinations
    ///     Of course if you happen to change the layer order or numbers, you'll want to udpate this class.
    /// </summary>
    public static class LayerManager
    {
        private static readonly int ObstaclesLayer = 8;
        private static readonly int GroundLayer = 9;
        private static readonly int PlayerLayer = 10;
        private static readonly int EnemiesLayer = 13;
        private static readonly int HoleLayer = 15;
        private static readonly int MovingPlatformLayer = 16;
        private static readonly int FallingPlatformLayer = 17;
        private static readonly int ProjectileLayer = 18;

        public static int ObstaclesLayerMask = 1 << ObstaclesLayer;
        public static int GroundLayerMask = 1 << GroundLayer;
        public static int PlayerLayerMask = 1 << PlayerLayer;
        public static int EnemiesLayerMask = 1 << EnemiesLayer;
        public static int HoleLayerMask = 1 << HoleLayer;
        public static int MovingPlatformLayerMask = 1 << MovingPlatformLayer;
        public static int FallingPlatformLayerMask = 1 << FallingPlatformLayer;
        public static int ProjectileLayerMask = 1 << ProjectileLayer;
    }
}