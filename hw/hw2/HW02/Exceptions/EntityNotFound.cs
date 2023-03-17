
namespace HW02.Exceptions
{
    public class EntityNotFound : Exception
    {
        public int Id {get;}
        public OpCode OpCode {get; }
        public bool IsCategory { get; }
        public EntityNotFound(OpCode opCode, int id, bool isCategory) 
        {
            OpCode = opCode;
            Id = id;
            IsCategory = isCategory;
        }
    }
}
