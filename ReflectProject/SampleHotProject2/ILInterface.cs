
namespace SampleHotProject2
{
    public interface ILBaseInterface
    {
        string Name { set; get; }
        string Score { set; get; }
    }

    class ILInterface : ILBaseInterface
    {
        string _name;
        public string Name
        {
            set { _name = value; }
            get { return _name; }
        }

        string _score;
        public string Score
        {
            set { _score = value; }
            get { return _score; }
        }
    }
}
