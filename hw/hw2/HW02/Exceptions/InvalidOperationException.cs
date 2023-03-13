
namespace HW02.Exceptions
{
    public class InvalidOperationException : Exception
    {
        public string Op {get;}
        public InvalidOperationException(string op) 
        {
            Op = op;
        }
    }
}
