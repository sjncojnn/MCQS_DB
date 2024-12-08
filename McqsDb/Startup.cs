using Mcq.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace Mcq.Middleware
{   
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();
            Console.WriteLine(path);
            Console.WriteLine(context.Session.GetString("UserId"));
            if ((!(path.StartsWith("/account")||path.StartsWith("/register"))) && string.IsNullOrEmpty(context.Session.GetString("UserId")))
            {
                Console.WriteLine(context.Session.GetString("RoleID"));
                Console.WriteLine(path);
                context.Response.Redirect("/Account/Login");
                return;
            }

            await _next(context);
    }   }

    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var roleId = context.Session.GetString("RoleId");

            if (!string.IsNullOrEmpty(roleId))
            {
                var path = context.Request.Path.ToString().ToLower();
                if (path.StartsWith("/teacher") && roleId != "2")
                {
                    context.Response.Redirect("/Account/AccessDenied");
                    return ;
                }

                if (path.StartsWith("/student") && roleId != "3")
                {
                    context.Response.Redirect("/Account/AccessDenied");
                    return;
                }
            }

            await _next(context);
        }
    }

    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionTimeoutMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Kiểm tra nếu người dùng chưa đăng nhập
            if (string.IsNullOrEmpty(context.Session.GetString("UserId")))
            {
                // Chuyển hướng đến trang login
                context.Response.Redirect("/Account/Login");
                return;
            }

            // Nếu người dùng đã đăng nhập, tiếp tục xử lý
            await _next(context);
        }
    }



}
