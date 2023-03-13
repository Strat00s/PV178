
namespace HW02.Exceptions
{
    public class EntityNotFound : Exception
    {
        public int Id {get;}
        public OpCode OpCode {get; }
        public EntityNotFound(OpCode opCode, int id) 
        {
            OpCode = opCode;
            Id = id;
        }
    }
}
