
namespace HW02.Exceptions
{
    public class InvalidOpException : Exception
    {
        public string Op {get;}
        public InvalidOpException(string op) 
        {
            Op = op;
        }
    }
}
