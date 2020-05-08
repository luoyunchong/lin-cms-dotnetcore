// using System.Threading.Tasks;
// using Castle.Core.Logging;
// using FreeSql;
// using LinCms;
// using LinCms.Web;
// using LinCms.Web.Middleware;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;
//
// namespace LinCms.Web.Middleware
// {
//     /// <summary>
//     /// 处理FreeSql中同一个请求处于一个事务，并一同提交
//     /// </summary>
//     public class UnitOfWorkMiddleware : IMiddleware
//     {
//         private readonly UnitOfWorkManager _unitOfWorkManager;
//         private readonly ILogger<UnitOfWorkMiddleware> _logger;
//         public UnitOfWorkMiddleware(UnitOfWorkManager unitOfWorkManager, ILogger<UnitOfWorkMiddleware> logger)
//         {
//             this._unitOfWorkManager = unitOfWorkManager;
//             this._logger = logger;
//         }
//
//         public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//         {
//             _logger.LogInformation($"事务开启中...");
//             using (IUnitOfWork unitOfWork = _unitOfWorkManager.Begin(Propagation.Nested))
//             {
//                 try
//                 {
//                     await next(context);
//                     unitOfWork.Commit();
//                     _logger.LogInformation($"事务提交成功...");
//                 }
//                 catch
//                 {
//                     unitOfWork.Rollback();
//                     _logger.LogInformation($"事务回滚了...");
//
//                     throw;
//                 }
//             }
//           
//         }
//     }
// }
