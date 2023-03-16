
namespace HW02.Exceptions
{
    public class InvalidOpException : Exception
    {
        public string Operation {get;}
        public InvalidOpException(string operation) 
        {
            Operation = operation;
        }
    }
}
