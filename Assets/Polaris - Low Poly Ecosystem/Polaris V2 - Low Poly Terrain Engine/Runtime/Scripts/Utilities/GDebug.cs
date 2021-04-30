using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Pinwheel.Griffin.Debugging
{
    public static class GDebug
    {
        private static Stopwatch st = new Stopwatch();

        public static void StartStopwatch()
        {
            st.Start();
        }

        public static void EndStopwatch()
        {
            st.Stop();
        }

        public static void LogStopwatchTimeMilis()
        {
            Debug.Log(st.ElapsedMilliseconds);
            st.Reset();
        }
    }
}
