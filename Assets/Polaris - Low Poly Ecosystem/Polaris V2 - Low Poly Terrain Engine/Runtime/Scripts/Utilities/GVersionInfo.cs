namespace Pinwheel.Griffin
{
    /// <summary>
    /// Utility class contains product info
    /// </summary>
    public static class GVersionInfo
    {
        public static float Number
        {
            get
            {
                return 259;
            }
        }

        public static string Code
        {
            get
            {
                return "2.5.9 LTS";
            }
        }

        public static string ProductName
        {
            get
            {
                return "Polaris - Low Poly Terrain Engine";
            }
        }

        public static string ProductNameAndVersion
        {
            get
            {
                return string.Format("{0} v{1}", ProductName, Code);
            }
        }

        public static string ProductNameShort
        {
            get
            {
                return "Polaris";
            }
        }

        public static string ProductNameAndVersionShort
        {
            get
            {
                return string.Format("{0} v{1}", ProductNameShort, Code);
            }
        }
    }
}
