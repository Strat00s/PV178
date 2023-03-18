/* Apparently it is a good practice to have enums in their own files. So here it is
 */

namespace HW02
{
    public enum OpCode
    {
        EXIT = 0,
        HELP,

        GET_BY_CATG,

        ADD_PROD,
        UPD_PROD,
        DEL_PROD,
        LST_PROD,

        ADD_CATG,
        UPD_CATG,
        DEL_CATG,
        LST_CATG,

        NONE = 99,
        OP_ERR
    }
}
