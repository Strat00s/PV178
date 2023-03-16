using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW02
{
    public enum OpCode
    {
        EXIT,
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
