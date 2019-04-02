using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;



namespace Common.Redis
{
    [Serializable]
    public class BaseEntity
    {
        public long ID { get; set; }
    }


    /// <summary>
    /// 排序方式
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 从小到大排序
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// 从大到小排序
        /// </summary>
        Descending = 1
    }

    /// <summary>
    /// 排序类型
    /// </summary>
    public enum SortByType
    {

        Numeric = 0,

        Alphabetic = 1
    }

    public enum RedisCommandFlags
    {
        //
        // 摘要:
        //     Default behaviour.
        None = 0,
        //
        // 摘要:
        //     This operation should be performed on the master if it is available, but read
        //     operations may be performed on a slave if no master is available. This is the
        //     default option.
        PreferMaster = 0,
        //
        // 摘要:
        //     From 2.0, this flag is not used
        HighPriority = 1,
        //
        // 摘要:
        //     The caller is not interested in the result; the caller will immediately receive
        //     a default-value of the expected return type (this value is not indicative of
        //     anything at the server).
        FireAndForget = 2,
        //
        // 摘要:
        //     This operation should only be performed on the master.
        DemandMaster = 4,
        //
        // 摘要:
        //     This operation should be performed on the slave if it is available, but will
        //     be performed on a master if no slaves are available. Suitable for read operations
        //     only.
        PreferSlave = 8,
        //
        // 摘要:
        //     This operation should only be performed on a slave. Suitable for read operations
        //     only.
        DemandSlave = 12,
        //
        // 摘要:
        //     Indicates that this operation should not be forwarded to other servers as a result
        //     of an ASK or MOVED response
        NoRedirect = 64,
        //
        // 摘要:
        //     Indicates that script-related operations should use EVAL, not SCRIPT LOAD + EVALSHA
        NoScriptCache = 512
    }

    /// <summary>
    /// Redis 助手
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// 获取 Redis 连接对象
        /// </summary>
        /// <returns></returns>
        public IConnectionMultiplexer GetConnectionRedisMultiplexer()
        {
            if (_connMultiplexer == null || !_connMultiplexer.IsConnected)
                lock (Locker)
                {
                    if (_connMultiplexer == null || !_connMultiplexer.IsConnected)
                    {

                        _connMultiplexer = ConnectionMultiplexer.Connect(ConnectionString);
                    }
                }

            return _connMultiplexer;
        }

        #region 其它

        public ITransaction GetTransaction()
        {
            return _db.CreateTransaction();
        }



        #endregion 其它

        #region private field

        /// <summary>
        /// 连接字符串
        /// </summary>
        private static string ConnectionString;

        /// <summary>
        /// redis 连接对象
        /// </summary>
        private static IConnectionMultiplexer _connMultiplexer;

        /// <summary>
        /// 默认的 Key 值（用来当作 RedisKey 的前缀）
        /// </summary>
        private static readonly string DefaultKey = "STUDENTS";

        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// 数据库
        /// </summary>
        private readonly IDatabase _db;

        #endregion private field

        #region 构造函数



        public RedisHelper(string connectstring, int db = 0)
        {
            ConnectionString = connectstring;
            _connMultiplexer = GetConnectionRedisMultiplexer();
            _db = _connMultiplexer.GetDatabase(db);
        }

        #endregion 构造函数

        #region String 操作

        /// <summary>
        /// 设置 key 并保存字符串（如果 key 已存在，则覆盖值）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet(string redisKey, string redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.StringSet(redisKey, redisValue, expiry);
        }

        /// <summary>
        /// 保存多个 Key-value
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public bool StringSet(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var pairs = keyValuePairs.Select(x => new KeyValuePair<RedisKey, RedisValue>(AddKeyPrefix(x.Key), x.Value));
            return _db.StringSet(pairs.ToArray());
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public string StringGet(string redisKey, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.StringGet(redisKey);
        }

        /// <summary>
        /// 存储一个对象（该对象会被序列化保存）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet<T>(string redisKey, T redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            var json = Serialize(redisValue);
            return _db.StringSet(redisKey, json, expiry);
        }

        /// <summary>
        /// 获取一个对象（会进行反序列化）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public T StringGet<T>(string redisKey, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(_db.StringGet(redisKey));
        }

        #region async

        /// <summary>
        /// 保存一个字符串值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(string redisKey, string redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.StringSetAsync(redisKey, redisValue, expiry);
        }

        /// <summary>
        /// 保存一组字符串值
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var pairs = keyValuePairs.Select(x => new KeyValuePair<RedisKey, RedisValue>(AddKeyPrefix(x.Key), x.Value));
            return await _db.StringSetAsync(pairs.ToArray());
        }

        /// <summary>
        /// 获取单个值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<string> StringGetAsync(string redisKey, string redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.StringGetAsync(redisKey);
        }

        /// <summary>
        /// 存储一个对象（该对象会被序列化保存）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> StringSetAsync<T>(string redisKey, T redisValue, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            var json = Serialize(redisValue);
            return await _db.StringSetAsync(redisKey, json, expiry);
        }

        /// <summary>
        /// 获取一个对象（会进行反序列化）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<T> StringGetAsync<T>(string redisKey, TimeSpan? expiry = null)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(await _db.StringGetAsync(redisKey));
        }

        #endregion async

        #endregion String 操作

        #region Hash 操作

        /// <summary>
        /// 判断该字段是否存在 hash 中
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public bool HashExists(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashExists(redisKey, hashField);
        }

        /// <summary>
        /// 从 hash 中移除指定字段
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public bool HashDelete(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashDelete(redisKey, hashField);
        }

        /// <summary>
        /// 从 hash 中移除指定字段
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashFields"></param>
        /// <returns></returns>
        public long HashDelete(string redisKey, IEnumerable<string> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            var fields = hashFields.Select(x => (RedisValue)x);

            return _db.HashDelete(redisKey, fields.ToArray());
        }

        /// <summary>
        /// 在 hash 设定值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HashSet(string redisKey, string hashField, string value)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashSet(redisKey, hashField, value);
        }

        /// <summary>
        /// 在 hash 中设定值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashFields"></param>
        public void HashSet(string redisKey, IEnumerable<KeyValuePair<string, string>> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            var entries = hashFields.Select(x => new HashEntry(x.Key, x.Value));

            _db.HashSet(redisKey, entries.ToArray());
        }

        /// <summary>
        /// 在 hash 中获取值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public string HashGet(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.HashGet(redisKey, hashField);
        }

        /// <summary>
        /// 在 hash 中获取值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashFields"></param>
        /// <returns></returns>
        public IEnumerable<string> HashGet(string redisKey, IEnumerable<string> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            var fields = hashFields.Select(x => (RedisValue)x);

            return ConvertStrings(_db.HashGet(redisKey, fields.ToArray()));
        }

        /// <summary>
        /// 从 hash 返回所有的字段值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public IEnumerable<string> HashKeys(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(_db.HashKeys(redisKey));
        }

        /// <summary>
        /// 返回 hash 中的所有值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public IEnumerable<string> HashValues(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(_db.HashValues(redisKey));
        }

        /// <summary>
        /// 在 hash 设定值（序列化）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public bool HashSet<T>(string redisKey, string hashField, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            var json = Serialize(redisValue);

            return _db.HashSet(redisKey, hashField, json);
        }

        /// <summary>
        /// 在 hash 中获取值（反序列化）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public T HashGet<T>(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);

            return Deserialize<T>(_db.HashGet(redisKey, hashField));
        }

        #region async

        /// <summary>
        /// 判断该字段是否存在 hash 中
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashExistsAsync(redisKey, hashField);
        }

        /// <summary>
        /// 从 hash 中移除指定字段
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashDeleteAsync(redisKey, hashField);
        }

        /// <summary>
        /// 从 hash 中移除指定字段
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashFields"></param>
        /// <returns></returns>
        public async Task<long> HashDeleteAsync(string redisKey, IEnumerable<string> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            var fields = hashFields.Select(x => (RedisValue)x);

            return await _db.HashDeleteAsync(redisKey, fields.ToArray());
        }

        /// <summary>
        /// 在 hash 设定值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync(string redisKey, string hashField, string value)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashSetAsync(redisKey, hashField, value);
        }

        /// <summary>
        /// 在 hash 中设定值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashFields"></param>
        public async Task HashSetAsync(string redisKey, IEnumerable<KeyValuePair<string, string>> hashFields)
        {
            redisKey = AddKeyPrefix(redisKey);
            var entries = hashFields.Select(x => new HashEntry(AddKeyPrefix(x.Key), x.Value));
            await _db.HashSetAsync(redisKey, entries.ToArray());
        }

        /// <summary>
        /// 在 hash 中获取值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public async Task<string> HashGetAsync(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.HashGetAsync(redisKey, hashField);
        }

        /// <summary>
        /// 在 hash 中获取值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashFields"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> HashGetAsync(string redisKey, IEnumerable<string> hashFields,
            string value)
        {
            redisKey = AddKeyPrefix(redisKey);
            var fields = hashFields.Select(x => (RedisValue)x);

            return ConvertStrings(await _db.HashGetAsync(redisKey, fields.ToArray()));
        }

        /// <summary>
        /// 从 hash 返回所有的字段值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> HashKeysAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(await _db.HashKeysAsync(redisKey));
        }

        /// <summary>
        /// 返回 hash 中的所有值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> HashValuesAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(await _db.HashValuesAsync(redisKey));
        }

        /// <summary>
        /// 在 hash 设定值（序列化）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> HashSetAsync<T>(string redisKey, string hashField, T value)
        {
            redisKey = AddKeyPrefix(redisKey);
            var json = Serialize(value);
            return await _db.HashSetAsync(redisKey, hashField, json);
        }

        /// <summary>
        /// 在 hash 中获取值（反序列化）
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public async Task<T> HashGetAsync<T>(string redisKey, string hashField)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(await _db.HashGetAsync(redisKey, hashField));
        }

        #endregion async

        #endregion Hash 操作

        #region List 操作

        /// <summary>
        /// 从索引值获取元素
        /// </summary>
        /// <param name="redisKey">Key</param>
        /// <param name="index">索引值</param>
        /// <param name="flags">命令标识</param>
        /// <returns></returns>
        public string ListGetByIndex(string redisKey, int index, RedisCommandFlags flags = RedisCommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListGetByIndex(redisKey, index, (CommandFlags)(int)flags);
        }

        /// <summary>
        /// 从索引值更新元素
        /// </summary>
        /// <param name="redisKey">Key</param>
        /// <param name="index">索引值</param>
        /// <param name="flags">命令标识</param>
        /// <returns></returns>
        public void ListSetByIndex(string redisKey, int index, string redisValue, RedisCommandFlags flags = RedisCommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            _db.ListSetByIndex(redisKey, index, redisValue, (CommandFlags)(int)flags);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public string ListLeftPop(string redisKey)
        {

            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLeftPop(redisKey);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public string ListRightPop(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRightPop(redisKey);
        }

        /// <summary>
        /// 移除列表指定键上与该值相同的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListRemove(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRemove(redisKey, redisValue);
        }

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListRightPush(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRightPush(redisKey, redisValue);
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListLeftPush(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLeftPush(redisKey, redisValue);
        }

        /// <summary>
        /// 返回列表上该键的长度，如果不存在，返回 0
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public long ListLength(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLength(redisKey);
        }

        /// <summary>
        /// 返回在该列表上键所对应的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public IEnumerable<string> ListRange(string redisKey, long start = 0L, long stop = -1L)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(_db.ListRange(redisKey, start, stop));
        }

        /// <summary>
        /// 根据索引值获取对象的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="index"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public T ListGetIndex<T>(string redisKey, int index, RedisCommandFlags flags = RedisCommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(_db.ListGetByIndex(redisKey, index, (CommandFlags)(int)flags));
        }


        /// <summary>
        /// 根据索引值更新对象的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="index"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public void ListGetIndex<T>(string redisKey, int index, T obj, RedisCommandFlags flags = RedisCommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            var s = Serialize(obj);
            _db.ListSetByIndex(redisKey, index, s, (CommandFlags)(int)flags);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(_db.ListLeftPop(redisKey));
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(_db.ListRightPop(redisKey));
        }

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListRightPush<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListRightPush(redisKey, Serialize(redisValue));
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListLeftPush<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListLeftPush(redisKey, Serialize(redisValue));
        }


        /// <summary>
        /// 插入到指定元素之后的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="source"></param>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long ListInsertAfter<T>(string redisKey, T source, T obj, RedisCommandFlags flags = RedisCommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListInsertAfter(redisKey, Serialize(source), Serialize(obj), (CommandFlags)(int)flags);
        }


        /// <summary>
        /// 插入到指定元素之前的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="source"></param>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long ListInsertBefore<T>(string redisKey, T source, T obj, RedisCommandFlags flags = RedisCommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.ListInsertBefore(redisKey, Serialize(source), Serialize(obj), (CommandFlags)(int)flags);
        }




        #region List-async

        /// <summary>
        /// 从索引获取缓存的异步方法
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="index"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<string> ListGetByIndexAsync(string redisKey, int index, CommandFlags flags = CommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListGetByIndexAsync(redisKey, index, flags);
        }


        /// <summary>
        /// 从索引获取缓存的异步方法
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="index"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task ListSetByIndexAsync(string redisKey, int index, string redisValue, CommandFlags flags = CommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            await _db.ListSetByIndexAsync(redisKey, index, redisValue, flags);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<string> ListLeftPopAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLeftPopAsync(redisKey);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<string> ListRightPopAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRightPopAsync(redisKey);
        }

        /// <summary>
        /// 移除列表指定键上与该值相同的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListRemoveAsync(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRemoveAsync(redisKey, redisValue);
        }

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRightPushAsync(redisKey, redisValue);
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync(string redisKey, string redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLeftPushAsync(redisKey, redisValue);
        }

        /// <summary>
        /// 返回列表上该键的长度，如果不存在，返回 0
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLengthAsync(redisKey);
        }

        /// <summary>
        /// 返回在该列表上键所对应的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> ListRangeAsync(string redisKey, long start = 0L, long stop = -1L)
        {
            redisKey = AddKeyPrefix(redisKey);
            var query = await _db.ListRangeAsync(redisKey, start, stop);
            return query.Select(x => x.ToString());
        }

        /// <summary>
        /// 根据索引值获取缓存对象的异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="index"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<T> ListGetByIndexAsync<T>(string redisKey, int index, CommandFlags flags = CommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(await _db.ListGetByIndexAsync(redisKey, index, flags));
        }

        /// <summary>
        /// 更新指定索引处的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task ListSetByIndexAsync<T>(string redisKey, int index, T obj, CommandFlags flags = CommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            var s = Serialize(obj);
            await _db.ListSetByIndexAsync(redisKey, index, s, flags);
        }


        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<T> ListLeftPopAsync<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(await _db.ListLeftPopAsync(redisKey));
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<T> ListRightPopAsync<T>(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return Deserialize<T>(await _db.ListRightPopAsync(redisKey));
        }

        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListRightPushAsync(redisKey, Serialize(redisValue));
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public async Task<long> ListLeftPushAsync<T>(string redisKey, T redisValue)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListLeftPushAsync(redisKey, Serialize(redisValue));
        }

        /// <summary>
        /// 插入到指定元素之后的异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="source"></param>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> ListInsertAfterAsync<T>(string redisKey, T source, T obj, int index, CommandFlags flags = CommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListInsertAfterAsync(redisKey, Serialize(source), Serialize(obj), flags);

        }

        /// <summary>
        /// 插入到指定元素之前的异步方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="source"></param>
        /// <param name="obj"></param>
        /// <param name="index"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> ListInsertBeforeAsync<T>(string redisKey, T source, T obj, int index, CommandFlags flags = CommandFlags.None)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.ListInsertBeforeAsync(redisKey, Serialize(source), Serialize(obj), flags);
        }

        #endregion List-async

        #endregion List 操作

        #region SortedSet 操作

        public void Sort(string redisKey, long skip = 0, long take = -1, OrderType orderType = OrderType.Ascending, SortByType sortByType = SortByType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, RedisCommandFlags flags = RedisCommandFlags.None)
        {

            redisKey = AddKeyPrefix(redisKey);
            _db.Sort(redisKey, skip, take, (Order)(int)orderType, (SortType)(int)sortByType, by, get, (CommandFlags)(int)flags);
        }


        /// <summary>
        /// SortedSet 新增
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool SortedSetAdd(string redisKey, string member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);

            return _db.SortedSetAdd(redisKey, member, score);
        }

        /// <summary>
        /// 在有序集合中返回指定范围的元素，默认情况下从低到高。
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public IEnumerable<string> SortedSetRangeByRank(string redisKey, long start = 0L, long stop = -1L,
            OrderType order = OrderType.Ascending)
        {
            redisKey = AddKeyPrefix(redisKey);

            return _db.SortedSetRangeByRank(redisKey, start, stop, (Order)order).Select(x => x.ToString());
        }



        /// <summary>
        /// 返回有序集合的元素个数
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public long SortedSetLength(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetLength(redisKey);
        }

        /// <summary>
        /// 返回有序集合的元素个数
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="memebr"></param>
        /// <returns></returns>
        public bool SortedSetLength(string redisKey, string memebr)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetRemove(redisKey, memebr);
        }

        /// <summary>
        /// SortedSet 新增
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool SortedSetAdd<T>(string redisKey, T member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);
            var json = Serialize(member);

            return _db.SortedSetAdd(redisKey, json, score);
        }

        /// <summary>
        /// 增量的得分排序的集合中的成员存储键值键按增量
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double SortedSetIncrement(string redisKey, string member, double value = 1)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetIncrement(redisKey, member, value);
        }

        #region SortedSet-Async

        /// <summary>
        /// SortedSet 新增
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddAsync(string redisKey, string member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.SortedSetAddAsync(redisKey, member, score);
        }

        /// <summary>
        /// 在有序集合中返回指定范围的元素，默认情况下从低到高。
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> SortedSetRangeByRankAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return ConvertStrings(await _db.SortedSetRangeByRankAsync(redisKey));
        }

        /// <summary>
        /// 返回有序集合的元素个数
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<long> SortedSetLengthAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.SortedSetLengthAsync(redisKey);
        }

        /// <summary>
        /// 返回有序集合的元素个数
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="memebr"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetRemoveAsync(string redisKey, string memebr)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.SortedSetRemoveAsync(redisKey, memebr);
        }

        /// <summary>
        /// SortedSet 新增
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="member"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddAsync<T>(string redisKey, T member, double score)
        {
            redisKey = AddKeyPrefix(redisKey);
            var json = Serialize(member);

            return await _db.SortedSetAddAsync(redisKey, json, score);
        }



        /// <summary>
        /// 增量的得分排序的集合中的成员存储键值键按增量
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Task<double> SortedSetIncrementAsync(string redisKey, string member, double value = 1)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.SortedSetIncrementAsync(redisKey, member, value);
        }

        /// <summary>
        /// 查找成员的索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="menber"></param>
        /// <returns></returns>
        public Task<long?> SortedSetByRankAsync<T>(string redisKey, T menber)
        {
            redisKey = AddKeyPrefix(redisKey);

            return _db.SortedSetRankAsync(redisKey, Serialize(menber));
        }

        /// <summary>
        /// 根据索引值获取缓存对象的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public async Task<T> SortedSetByIndexAsync<T>(string redisKey, long index)
        {
            redisKey = AddKeyPrefix(redisKey);
            var objs = await _db.SortedSetRangeByRankAsync(redisKey, index, index);
            if (objs.Count() == 1)
            {
                T t = Deserialize<T>(objs[0]);
                return t;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// 根据编号查询缓存对象的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<T> SortedSetByIDAsync<T>(string redisKey, int id) where T : BaseEntity
        {
            redisKey = AddKeyPrefix(redisKey);
            var strs = await _db.SortedSetRangeByRankAsync(redisKey, (long)0, _db.SortedSetLength(redisKey));
            T result = strs.Select(x => Deserialize<T>(x)).Where(s => s.ID == id).SingleOrDefault();
            return result;
        }

        /// <summary>
        /// 根据分值查找缓存对象的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<T> SortedSetByScoreAsync<T>(string redisKey, long score)
        {
            redisKey = AddKeyPrefix(redisKey);
            var strs = await _db.SortedSetRangeByScoreAsync(redisKey, score, score);
            if (strs.Count() == 1)
            {
                T result = Deserialize<T>(strs[0]);
                return result;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }


        /// <summary>
        /// 根据分值更新缓存对象的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetUpdateByScoreAsync<T>(string redisKey, long score, T newObject) where T:BaseEntity
        {
            redisKey = AddKeyPrefix(redisKey);
            var count = await _db.SortedSetRemoveRangeByScoreAsync(redisKey, score, score);

            var result = Serialize(newObject);
            return await _db.SortedSetAddAsync(redisKey, result, newObject.ID);

        }

        /// <summary>
        /// 根据分值查找缓存对象的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="score_begin"></param>
        /// <param name="score_end"></param>
        /// <returns></returns>
        public async Task<List<T>> SortedSetByScoreAsync<T>(string redisKey, long score_begin, long score_end)
        {
            redisKey = AddKeyPrefix(redisKey);
            var strs = await _db.SortedSetRangeByScoreAsync(redisKey, score_begin, score_end);
            if (strs.Count() > 0)
            {
                List<T> result = strs.Select(x => JsonConvert.DeserializeObject<T>(x)).ToList();
                return result;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        #endregion SortedSet-Async

        #endregion SortedSet 操作

        #region key 操作

        /// <summary>
        /// 移除指定 Key
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public bool KeyDelete(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyDelete(redisKey);
        }

        /// <summary>
        /// 移除指定 Key
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        public long KeyDelete(IEnumerable<string> redisKeys)
        {
            var keys = redisKeys.Select(x => (RedisKey)AddKeyPrefix(x));
            return _db.KeyDelete(keys.ToArray());
        }

        /// <summary>
        /// 校验 Key 是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public bool KeyExists(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyExists(redisKey);
        }

        /// <summary>
        /// 重命名 Key
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisNewKey"></param>
        /// <returns></returns>
        public bool KeyRename(string redisKey, string redisNewKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyRename(redisKey, redisNewKey);
        }

        /// <summary>
        /// 设置 Key 的时间
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool KeyExpire(string redisKey, TimeSpan? expiry)
        {
            redisKey = AddKeyPrefix(redisKey);
            return _db.KeyExpire(redisKey, expiry);
        }

        #region key-async

        /// <summary>
        /// 移除指定 Key
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<bool> KeyDeleteAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyDeleteAsync(redisKey);
        }

        /// <summary>
        /// 移除指定 Key
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        public async Task<long> KeyDeleteAsync(IEnumerable<string> redisKeys)
        {
            var keys = redisKeys.Select(x => (RedisKey)AddKeyPrefix(x));
            return await _db.KeyDeleteAsync(keys.ToArray());
        }

        /// <summary>
        /// 校验 Key 是否存在
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public async Task<bool> KeyExistsAsync(string redisKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyExistsAsync(redisKey);
        }

        /// <summary>
        /// 重命名 Key
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisNewKey"></param>
        /// <returns></returns>
        public async Task<bool> KeyRenameAsync(string redisKey, string redisNewKey)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyRenameAsync(redisKey, redisNewKey);
        }

        /// <summary>
        /// 设置 Key 的时间
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string redisKey, TimeSpan? expiry)
        {
            redisKey = AddKeyPrefix(redisKey);
            return await _db.KeyExpireAsync(redisKey, expiry);
        }

        #endregion key-async

        #endregion key 操作


        #region 发布订阅

        #region 同步

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            sub.Subscribe(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long Publish<T>(string channel, T msg)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            return sub.Publish(channel, ConvertJson(msg));
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        /// <summary>
        /// 取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            sub.UnsubscribeAll();
        }

        #endregion 发布订阅

        #region 异步

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="subChannel"></param>
        /// <param name="handler"></param>
        public async Task SubscribeAsync(string subChannel, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            await sub.SubscribeAsync(subChannel, (channel, message) =>
            {
                if (handler == null)
                {
                    Console.WriteLine(subChannel + " 订阅收到消息：" + message);
                }
                else
                {
                    handler(channel, message);
                }
            });
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<long> PublishAsync<T>(string channel, T msg)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            return await sub.PublishAsync(channel, ConvertJson(msg));
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public async Task UnsubscribeAsync(string channel)
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            await sub.UnsubscribeAsync(channel);
        }

        /// <summary>
        /// 取消全部订阅
        /// </summary>
        public async Task UnsubscribeAllAsync()
        {
            ISubscriber sub = _connMultiplexer.GetSubscriber();
            await sub.UnsubscribeAllAsync();
        }

        #endregion 发布订阅

        #endregion


        #region private method

        /// <summary>
        /// 添加 Key 的前缀
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string AddKeyPrefix(string key)
        {
            return DefaultKey + ":" + key;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private static IEnumerable<string> ConvertStrings<T>(IEnumerable<T> list) where T : struct
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            return list.Select(x => x.ToString());
        }

        /// <summary>
        /// json转换的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        /// <summary>
        /// 反序列化的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private T ConvertObj<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        #region 注册事件

        /// <summary>
        /// 添加注册事件
        /// </summary>
        private static void AddRegisterEvent()
        {
            _connMultiplexer.ConnectionRestored += ConnMultiplexer_ConnectionRestored;
            _connMultiplexer.ConnectionFailed += ConnMultiplexer_ConnectionFailed;
            _connMultiplexer.ErrorMessage += ConnMultiplexer_ErrorMessage;
            _connMultiplexer.ConfigurationChanged += ConnMultiplexer_ConfigurationChanged;
            _connMultiplexer.HashSlotMoved += ConnMultiplexer_HashSlotMoved;
            _connMultiplexer.InternalError += ConnMultiplexer_InternalError;
            _connMultiplexer.ConfigurationChangedBroadcast += ConnMultiplexer_ConfigurationChangedBroadcast;
        }

        /// <summary>
        /// 重新配置广播时（通常意味着主从同步更改）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnMultiplexer_ConfigurationChangedBroadcast(object sender, EndPointEventArgs e)
        {
            Console.WriteLine($"{nameof(ConnMultiplexer_ConfigurationChangedBroadcast)}: {e.EndPoint}");
        }

        /// <summary>
        /// 发生内部错误时（主要用于调试）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnMultiplexer_InternalError(object sender, InternalErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(ConnMultiplexer_InternalError)}: {e.Exception}");
        }

        /// <summary>
        /// 更改集群时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnMultiplexer_HashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.WriteLine(
                $"{nameof(ConnMultiplexer_HashSlotMoved)}: {nameof(e.OldEndPoint)}-{e.OldEndPoint} To {nameof(e.NewEndPoint)}-{e.NewEndPoint}, ");
        }

        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnMultiplexer_ConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.WriteLine($"{nameof(ConnMultiplexer_ConfigurationChanged)}: {e.EndPoint}");
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnMultiplexer_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(ConnMultiplexer_ErrorMessage)}: {e.Message}");
        }

        /// <summary>
        /// 物理连接失败时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnMultiplexer_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($"{nameof(ConnMultiplexer_ConnectionFailed)}: {e.Exception}");
        }

        /// <summary>
        /// 建立物理连接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ConnMultiplexer_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine($"{nameof(ConnMultiplexer_ConnectionRestored)}: {e.Exception}");
        }

        #endregion 注册事件

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                var data = memoryStream.ToArray();
                return data;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static T Deserialize<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(data))
            {
                var result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }

        #endregion private method
    }
}