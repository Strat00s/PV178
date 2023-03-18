/* Exception for when either product or category is not found (during add, delete, ...)
 * Stores opcode, entity id and wheter entity is category or product (for better logging and printing)
 */

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
