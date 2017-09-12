using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace PartialTest
{
    public interface UnityBaseInterface
    {
        string Name { set; get; }
        int Score { set; get; }
    }

    public class UnityInterface : UnityBaseInterface
    {
        string _name;
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }

        int _score;
        public int Score
        {
            set { _score = value; }
            get { return _score; }
        }
    }


    public partial class PartialClassInMain
    {
        string _abc = "abcde";
        Dictionary<string, string> dic = new Dictionary<string, string>()
        {
            { "12", "34" }
        };

        public static void ABCDE()
        {

        }
    }
}

public class UnityAI
{
    public string Name;

    public string GetUnityAIName()
    {
        return Name;
    }
}

namespace PartialTest
{
    public partial class PartialClassInMain
    {
        public void ABCDEF()
        {
            foreach (var kv in dic)
            {

            }
        }
    }
}