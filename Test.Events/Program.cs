﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Test.Events
{
    static class Program
    {
        static void Main()
        {
            Server server = new Server("127.0.0.1", 9000, false, DefaultRoute);
            // server.AccessControl.Mode = AccessControlMode.DefaultDeny;
            // server.AccessControl.Whitelist.Add("127.0.0.1", "255.255.255.255");
            // server.AccessControl.Whitelist.Add("127.0.0.1", "255.255.255.255");

            server.Events.AccessControlDenied = AccessControlDenied;
            server.Events.ConnectionReceived = ConnectionReceived;
            server.Events.RequestorDisconnected = RequestorDisconnected;
            server.Events.ExceptionEncountered = ExceptionEncountered;
            server.Events.RequestReceived = RequestReceived;
            server.Events.ResponseSent = ResponseSent;
            server.Events.ServerDisposed = ServerDisposed;
            server.Events.ServerStopped = ServerStopped;
             
            bool runForever = true;
            while (runForever)
            {
                string userInput = InputString("Command [? for help] >", null, false);
                switch (userInput.ToLower())
                {
                    case "?":
                        Menu();
                        break;

                    case "q":
                        runForever = false;
                        break;

                    case "c":
                    case "cls":
                        Console.Clear();
                        break;

                    case "state":
                        Console.WriteLine("Listening: " + server.IsListening);
                        break;

                    case "dispose":
                        server.Dispose();
                        break;
                }
            }
        }

        static void Menu()
        {
            Console.WriteLine("---");
            Console.WriteLine("  ?        help, this menu");
            Console.WriteLine("  q        quit the application");
            Console.WriteLine("  cls      clear the screen");
            Console.WriteLine("  state    indicate whether or not the server is listening");
            Console.WriteLine("  dispose  dispose the server object");
        }

        static async Task DefaultRoute(HttpContext ctx)
        {
            Console.WriteLine(ctx.Request.ToString());

            if (ctx.Request.Method == HttpMethod.GET)
            {
                if (ctx.Request.RawUrlWithoutQuery.Equals("/delay"))
                {
                    await Task.Delay(10000);
                }
            }

            if ((ctx.Request.Method == HttpMethod.POST
                || ctx.Request.Method == HttpMethod.PUT)
                && ctx.Request.Data != null
                && ctx.Request.ContentLength > 0)
            {
                await ctx.Response.Send(ctx.Request.ContentLength, ctx.Request.Data);
                return;
            }
            else
            {
                ctx.Response.StatusCode = 200;
                await ctx.Response.Send("Watson says hello from the default route!");
                return;
            }
        }

        static string InputString(string question, string defaultAnswer, bool allowNull)
        {
            while (true)
            {
                Console.Write(question);

                if (!String.IsNullOrEmpty(defaultAnswer))
                {
                    Console.Write(" [" + defaultAnswer + "]");
                }

                Console.Write(" ");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    if (!String.IsNullOrEmpty(defaultAnswer)) return defaultAnswer;
                    if (allowNull) return null;
                    else continue;
                }

                return userInput;
            }
        }

        static void AccessControlDenied(string ip, int port, string method, string url)
        {
            Console.WriteLine("AccessControlDenied [" + ip + ":" + port + "] " + method + " " + url); 
        }

        static void RequestorDisconnected(string ip, int port, string method, string url)
        {
            Console.WriteLine("RequestorDisconnected [" + ip + ":" + port + "] " + method + " " + url); 
        }

        static void ConnectionReceived(string ip, int port)
        {
            Console.WriteLine("ConnectionReceived [" + ip + ":" + port + "]"); 
        }

        static void ExceptionEncountered(string ip, int port, Exception e)
        {
            Console.WriteLine("ExceptionEncountered [" + ip + ":" + port + "]: " + Environment.NewLine + e.ToString()); 
        }

        static void RequestReceived(string ip, int port, string method, string url)
        {
            Console.WriteLine("RequestReceived [" + ip + ":" + port + "] " + method + " " + url); 
        }

        static void ResponseSent(string ip, int port, string method, string url, int status, double totalTimeMs)
        {
            Console.WriteLine("ResponseSent [" + ip + ":" + port + "] " + method + " " + url + " status " + status + " " + totalTimeMs + "ms"); 
        }

        static void ServerDisposed()
        {
            Console.WriteLine("ServerDisposed"); 
        }

        static void ServerStopped()
        {
            Console.WriteLine("ServerStopped"); 
        }
    }
}
