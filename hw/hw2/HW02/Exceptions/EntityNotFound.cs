
namespace HW02.Exceptions
{
    public class EntityNotFound : Exception
    {
        public int OpCode {get;}
        public int EntityId {get;}
        public EntityNotFound(int opCode, int entityId) 
        {
            OpCode = opCode;
            EntityId = entityId;
        }
    }
}
