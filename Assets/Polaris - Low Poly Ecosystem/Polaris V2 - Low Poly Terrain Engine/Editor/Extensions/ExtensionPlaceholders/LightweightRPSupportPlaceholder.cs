#if GRIFFIN && UNITY_EDITOR && !GRIFFIN_LWRP
using UnityEngine;

namespace Pinwheel.Griffin.GriffinExtension
{
    public static class LightweightRPSupportPlaceholder
    {
        public static string GetExtensionName()
        {
            return "Lightweight Render Pipeline Support";
        }

        public static string GetPublisherName()
        {
            return "Pinwheel Studio";
        }

        public static string GetDescription()
        {
            return "Adding support for LWRP.\n" +
                "Requires Unity 2019.1 or above.";
        }

        public static string GetVersion()
        {
            return "v1.0.0";
        }

        public static void OpenSupportLink()
        {
            GEditorCommon.OpenEmailEditor(
                GCommon.SUPPORT_EMAIL,
                "[Polaris V2] LWRP Support",
                "YOUR_MESSAGE_HERE");
        }

        public static void Button_Download()
        {
            string url = "https://assetstore.unity.com/packages/slug/157782";
            Application.OpenURL(url);
        }
    }
}
#endif
