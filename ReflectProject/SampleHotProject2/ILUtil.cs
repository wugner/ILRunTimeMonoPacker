
namespace SampleHotProject2
{
    public static class ILUtil
    {
        public static string GetILAIName(this ILAI ai)
        {
            if (ai.Name == null)
                return "AlphaGo";
            else
                return ai.Name;
        }

        public static string GetUnityAIName(this UnityAI ai)
        {
            if (ai.Name == null)
                return "NaN";
            else
                return ai.Name;
        }
    }

    public class ILAI
    {
        public string Name;
    }
}
