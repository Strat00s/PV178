/*I probably missunderstood how this class is suppose to work...*/

using HW02.BussinessContext;
using HW02.LoggerContext.DB;

namespace HW02
{
    public class LoggerListener
    {

        private readonly LoggerDBContext _db;
        public LoggerListener(LoggerDBContext loggerDBContext)
        {
            _db = loggerDBContext;
        }

        public void HandleEvent(OpCode opCode, bool status, Category? entity = null, string? msg = null)
        {
            string log = DateTime.Now.ToString("[MM/dd/yyyy HH:mm:ss]") + " ";

            //write appropriate command
            switch (opCode)
            {
                case OpCode.EXIT: log += "Exit"; _db.WriteLog(log); return;
                case OpCode.HELP: log += "Help"; _db.WriteLog(log); return;
                
                case OpCode.GET_BY_CATG:
                case OpCode.LST_PROD: log += "Get; Product; ";    break;
                case OpCode.ADD_PROD: log += "Add; Product; ";    break;
                case OpCode.DEL_PROD: log += "Delete; Product; "; break;
                case OpCode.UPD_PROD: log += "Update; Product; "; break;
                
                case OpCode.ADD_CATG: log += "Add; Category; ";    break;
                case OpCode.DEL_CATG: log += "Delete; Category; "; break;
                case OpCode.UPD_CATG: log += "Update; Category; "; break;
                case OpCode.LST_CATG: log += "Get; Category; ";    break;

                default: log += "Other; " + msg ?? ""; _db.WriteLog(log); return;
            }

            //write message on fail
            if (!status)
            {
                log += "Failure; " + msg ?? "";
                _db.WriteLog(log);
                return;
            }

            log += "Success; ";
            
            //get commands don't have entity
            if (entity == null)
            {
                _db.WriteLog(log);
                return;
            }

            log += entity.Id + "; " + entity.Name + "; ";

            //check which entity we got
            if (entity is Product product)
                log += product.CategoryId;
            else
                log += entity.Id;

            _db.WriteLog(log);
        }
    }
}
