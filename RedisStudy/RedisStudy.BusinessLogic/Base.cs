using RedisStudy.BusinessLogic.Logger;
using RedisStudy.Data;

namespace RedisStudy.BusinessLogic
{
    public  class BaseService
    {
        protected  ILog _logger;

        public BaseService(ILog logger)
        {
            _logger = logger;
           
        }
    }
}
