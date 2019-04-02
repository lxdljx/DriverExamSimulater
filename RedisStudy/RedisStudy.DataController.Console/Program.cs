using Common.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RedisStudy.BusinessLogic.Exam;
using RedisStudy.BusinessLogic.Logger;
using RedisStudy.BusinessLogic.Redis;
using RedisStudy.ViewModel.EventObject;
using RedisStudy.ViewModel.Models.Exam;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RedisStudy.DataController
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("订阅的数据操作端启动......");
           

            string connectstr =  new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .Build()["RedisServer:Url"];
            RedisHelper redisHelper = new RedisHelper(connectstr, 1);
            redisHelper.Subscribe("SSS", async (channel, value) =>
            {
                
                    //更新数据库
                    ILog _log = new ConsoleLog();
                    IExamService _service = new ExamService(_log);
                    IRedisService _redis = new RedisService(_log);
                    EventObject evo = JsonConvert.DeserializeObject<EventObject>(value);
                    switch (evo.Type)
                    {
                        case EventType.ExamBegin:
                            {

                                ExamBeginViewModel model = JsonConvert.DeserializeObject<ExamBeginViewModel>(evo.EventModelJson);
                                _log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}收到编号为{model.StudentID}的考生第{model.Order}次考试开始");
                                var result = await  _service.ExamBegin(model);
                                {
                                    
                                    if (result.Successed)
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试开始成功");
                                        //更新缓存

                                        var rs = await _redis.ExamBegin(model);
                                        {
                                            
                                            if (rs.Successed)
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试开始成功");
                                            }
                                            else
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试开始失败，原因是{rs.CurrentExceptionMessage}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试开始失败,错误消息为{result.CurrentExceptionMessage}");
                                    }
                                }
                            }
                            break;
                        case EventType.ExamEnd:
                            {

                                ExamEndViewModel model = JsonConvert.DeserializeObject<ExamEndViewModel>(evo.EventModelJson);
                                _log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}收到编号为{model.StudentID}的考生第{model.Order}次考试结束");
                                var result = await _service.ExamEnd(model);
                                {
                                    
                                    if (result.Successed)
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试结束成功");
                                        //更新缓存
                                        var rs = await _redis.ExamEnd(model);
                                        {
                                            
                                            if (rs.Successed)
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试结束成功");
                                            }
                                            else
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试结束失败，原因是{rs.CurrentExceptionMessage}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试结束失败,错误消息为{result.CurrentExceptionMessage}");
                                    }
                                }
                            }
                            break;
                        case EventType.ProjectBegin:
                            {
                                ExamProjectBeginViewModel model = JsonConvert.DeserializeObject<ExamProjectBeginViewModel>(evo.EventModelJson);
                                _log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}收到编号为{model.StudentID}的考生第{model.Order}次考试项目{model.ProjectName}开始");
                                var result = await _service.ExamProjectBegin(model);
                                {
                                    if (result.Successed)
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目开始成功");
                                        //更新缓存
                                        var rs = await _redis.ExamProjectBegin(model);
                                        {
                                            
                                            if (rs.Successed)
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目开始成功");
                                            }
                                            else
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目开始失败，原因是{rs.CurrentExceptionMessage}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目开始失败,错误消息为{result.CurrentExceptionMessage}");
                                    }
                                }
                            }
                            break;
                        case EventType.projectEnd:
                            {
                                ExamProjectEndViewModel model = JsonConvert.DeserializeObject<ExamProjectEndViewModel>(evo.EventModelJson);
                                _log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}收到编号为{model.StudentID}的考生第{model.Order}次考试项目{model.ProjectName}结束");
                                var result = await _service.ExamProjectEnd(model);
                                {
                                    
                                    if (result.Successed)
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目结束成功");
                                        //更新缓存
                                        var rs = await _redis.ExamProjectEnd(model);
                                        {
                                            
                                            if (rs.Successed)
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目结束成功");
                                            }
                                            else
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目结束失败，原因是{rs.CurrentExceptionMessage}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试{model.ProjectName}项目结束失败,错误消息为{result.CurrentExceptionMessage}");
                                    }
                                }
                            }
                            break;
                        case EventType.LostPoint:
                            {
                                ExamLostPointViewModel model = JsonConvert.DeserializeObject<ExamLostPointViewModel>(evo.EventModelJson);
                                _log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}收到编号为{model.StudentID}的考生扣分{model.Point},原因是{model.Reason}");
                                var result = await _service.ExamLostPoint(model);
                                {
                                   
                                    if (result.Successed)
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试扣分成功");
                                        //更新缓存
                                        var rs = await _redis.ExamLostPoint(model);
                                        {
                                            
                                            if (rs.Successed)
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试扣分成功");
                                            }
                                            else
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}第{model.Order}次考试扣分失败，原因是{rs.CurrentExceptionMessage}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}第{model.Order}次考试扣分失败,错误消息为{result.CurrentExceptionMessage}");
                                    }
                                }
                            }
                            break;

                        case EventType.AllEnd:
                            {
                                ExamAllEndViewModel model = JsonConvert.DeserializeObject<ExamAllEndViewModel>(evo.EventModelJson);
                                _log.Info($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff")}收到编号为{model.StudentID}的考生考试全部结束");
                                var result = await _service.ExamAllEnd(model);
                                {
                                   
                                    if (result.Successed)
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}考试完成成功");
                                        //更新缓存
                                        var rs = await _redis.ExamAllEnd(model);
                                        {
                                            
                                            if (rs.Successed)
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}考试全部结束成功");
                                            }
                                            else
                                            {
                                                _log.Info($"缓存{DateTime.Now}考生{model.StudentID}考试全部结束失败，原因是{rs.CurrentExceptionMessage}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Info($"{DateTime.Now}考生{model.StudentID}考试完成失败,错误消息为{result.CurrentExceptionMessage}");
                                    }
                                }
                            }
                            break;
                        default:
                            _log.Info($"{DateTime.Now}未知的事件类型");
                            break;
                    }

                
            });
            Console.ReadLine();
        }
    }
}
