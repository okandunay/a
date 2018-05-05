using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Port.Bussines.Base
{
    public abstract class BaseRepository
    {
        OperationStatusMessage _result;
        private readonly string _con;

        protected BaseRepository(string con)
        {
            _con = con;
        }

        public virtual async Task<dynamic> QuerySp(Func<IDbConnection, Task<dynamic>> getData)
        {

            using (var con = new SqlConnection(_con))
            {
                await con.OpenAsync();

                return await getData(con);
            }
        }

        public virtual async Task<OperationStatusMessage> ExecuteSp(Func<IDbConnection, Task> executeData)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var con = new SqlConnection(_con))
                {
                    _result = new OperationStatusMessage();
                    await con.OpenAsync();

                    try
                    {
                        await executeData(con);
                        transaction.Complete();
                        _result.Status = (int)StatusType.Ok;
                    }
                    catch (Exception)
                    {
                        _result.Status = (int)StatusType.Nok;
                    }
                }
            }
            return _result;
        }
    }
}
public class OperationStatusMessage
{
    public int Status { get; set; }
}
public enum StatusType {
Nok=0 ,
Ok=1
}