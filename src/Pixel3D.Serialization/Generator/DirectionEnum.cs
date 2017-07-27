namespace Pixel3D.Serialization.Generator
{
    enum Direction
    {
        Serialize = 0,
        Deserialize = 1,
    }

    static class DirectionExtensionMethods
    {
        static string[] names = { "Serialize", "Deserialize" };
        public static string Name(this Direction direction)
        {
            return names[(int)direction];
        }
    }

}
