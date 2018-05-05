using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
namespace Port.Bussines.Base
{
    // ReSharper disable once UnusedTypeParameter
    public class GenericRepository<T> : BaseRepository where T : class 
    {

        public GenericRepository(string con) : base(con)
        {
        }

        public async Task<IEnumerable<dynamic>>QuerySp(string spName, Dictionary<string, object> _params=null)
        {
            var dp = new DynamicParameters();

            if (_params != null)
                foreach (var item in _params)
                {
                    dp.Add(item.Key, item.Value);
                }

            var returnList = await QuerySp(async c => await c.QueryAsync<dynamic>(sql: spName, param: dp, commandType: CommandType.StoredProcedure));

            return returnList;
        }
        public async Task<OperationStatusMessage> ExecuteSp(string spName, Dictionary<string, object> _params)
        {
            var addParams = new DynamicParameters();

            foreach (var item in _params)
            {
                addParams.Add(item.Key, item.Value);
            }

            var status = await ExecuteSp(async c =>
            {
                await Task.Run(() => c.Execute(sql: spName, param: addParams, commandType: CommandType.StoredProcedure));
            });
            return status;
        }
    }
}
