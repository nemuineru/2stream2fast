using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Griffin
{
    [ExecuteInEditMode]
    public static class GJobSystem
    {
        #region Initialization
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitEditor()
        {
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.update += Tick;
            }
            else if (change == PlayModeStateChange.EnteredPlayMode)
            {
                EditorApplication.update -= Tick;
            }
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            GTicker.AddListener(Tick);
        }
        #endregion

        private static volatile List<GJob> runOnMainThreadJobs;
        private static List<GJob> RunOnMainThreadJobs
        {
            get
            {
                if (runOnMainThreadJobs == null)
                    runOnMainThreadJobs = new List<GJob>();
                return runOnMainThreadJobs;
            }
            set
            {
                runOnMainThreadJobs = value;
            }
        }

        private static volatile List<GJob> scheduleOnMainThreadJobs;
        private static List<GJob> ScheduleOnMainThreadJobs
        {
            get
            {
                if (scheduleOnMainThreadJobs == null)
                    scheduleOnMainThreadJobs = new List<GJob>();
                return scheduleOnMainThreadJobs;
            }
            set
            {
                scheduleOnMainThreadJobs = value;
            }
        }

        public static void RunOnMainThread(Action a)
        {
            lock (RunOnMainThreadJobs)
            {
                GJob j = new GJob();
                j.Action = a;
                RunOnMainThreadJobs.Add(j);
            }
        }

        public static void ScheduleOnMainThread(Action a, int order = 10)
        {
            lock (ScheduleOnMainThreadJobs)
            {
                GJob j = new GJob();
                j.Action = a;
                j.Order = order;
                ScheduleOnMainThreadJobs.Add(j);
            }
        }

        public static void RunOnBackground(Action a)
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    if (a != null)
                        a.Invoke();
                }
                catch (System.Exception e)
                {
                    Debug.Log("Background thread: " + e.ToString());
                }
            });
            t.Priority = System.Threading.ThreadPriority.Highest;
            t.Start();
        }

        private static void Tick()
        {
            try
            {
                if (RunOnMainThreadJobs.Count > 0)
                {
                    RunOnMainThreadJobs.RemoveAll(j => j == null || j.Action == null);
                    for (int i = 0; i < RunOnMainThreadJobs.Count; ++i)
                    {
                        GJob j = RunOnMainThreadJobs[i];
                        j.Run();
                    }
                    RunOnMainThreadJobs.Clear();
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Error on job scheduling: " + e.ToString());
                RunOnMainThreadJobs = new List<GJob>();
            }

            try
            {
                if (ScheduleOnMainThreadJobs.Count > 0)
                {
                    ScheduleOnMainThreadJobs.RemoveAll(j => j == null || j.Action == null);
                    if (ScheduleOnMainThreadJobs.Count > 0)
                    {
                        ScheduleOnMainThreadJobs.Sort((j0, j1) => -j0.Order.CompareTo(j1.Order));
                        int lastJobIndex = ScheduleOnMainThreadJobs.Count - 1;
                        ScheduleOnMainThreadJobs[lastJobIndex].Run();
                        ScheduleOnMainThreadJobs[lastJobIndex] = null;
                    }
                }
            }
            catch (System.Exception e)
            {
                ScheduleOnMainThreadJobs = new List<GJob>();
                Debug.Log("Error on job scheduling: " + e.ToString());
            }
        }

        public static void Sleep(int milis)
        {
            Thread.Sleep(milis);
        }
    }
}
