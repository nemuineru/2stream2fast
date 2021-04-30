using UnityEngine;

namespace Pinwheel.Griffin
{
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class GTicker : MonoBehaviour
    {
        public delegate void TickHandler();
        private event TickHandler Tick;

        private static GTicker instance;
        private static GTicker Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject g = new GameObject("Ticker");
                    DontDestroyOnLoad(g);
                    GTicker ticker = g.AddComponent<GTicker>();
                    instance = ticker;
                }
                return instance;
            }
        }

        public static void AddListener(TickHandler callback)
        {
            Instance.Tick += callback;
        }

        public static void RemoveListener(TickHandler callback)
        {
            Instance.Tick -= callback;
        }

        private void LateUpdate()
        {
            if (Instance == this && Tick != null)
            {
                Tick.Invoke();
            }
        }
    }
}
