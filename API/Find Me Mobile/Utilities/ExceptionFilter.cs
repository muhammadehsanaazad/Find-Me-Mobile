using Find_Me_Mobile.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;

namespace Find_Me_Mobile.Utilities
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (!Directory.Exists("c:/log"))
                Directory.CreateDirectory("c:/log");
            // Log Exception Message
            File.AppendAllText("c:/log/log.txt", Environment.NewLine + "Utc => " + DateTime.UtcNow + Environment.NewLine + "Local => " + DateTime.Now + Environment.NewLine + "Error => " + context.Exception.GetBaseException().Message + Environment.NewLine);

            var result = new ApplicationResult() { IsSuccess = false, Message = context.Exception.Message };
            context.Result = new JsonResult(result);

            base.OnException(context);
        }
    }
}
