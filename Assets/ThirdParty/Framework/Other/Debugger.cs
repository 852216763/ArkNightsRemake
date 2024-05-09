namespace Framework
{
    public class Debugger
    {
        public static void Log(object o)
        {
            UnityEngine.Debug.Log(o.ToString());
        }

        public static void LogWarning(object o)
        {
            UnityEngine.Debug.LogWarning(o);
        }

        public static void LogError(object o)
        {
            UnityEngine.Debug.LogError(o.ToString());
        }
    }

}
