using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplechatbot;
using Grpc.Core;

namespace SimpleChatdbotClient
{
    class SimpleChatBotClientImp
    {
        private Simplechatbot.SimpleChatBotServer.SimpleChatBotServerClient client;
        private int id = 0;

        public SimpleChatBotClientImp(SimpleChatBotServer.SimpleChatBotServerClient client)
        {
            this.client = client;
        }

        private void Log(string s, params object[] args)
        {
            Console.WriteLine(string.Format(s, args));
        }

        private void Log(string s)
        {
            Console.WriteLine(s);
        }

        public async Task<string> ChatAsync(string data)
        {
            try
            {
                var req = new Simplechatbot.ChatRequest();
                id++;
                req.Ids = $"id";
                req.Data = data;
                using (var call = client.ChatAsync(req))
                {
                    var responseReaderTask = Task.Run(async () =>
                    {
                        await call.ResponseAsync;
                        var rep = call.ResponseAsync.Result;
                        Log("Recv Rep ids:{0},result:{1}", rep.Ids, rep.Data);
                        return rep.Data;
                    });
                    return await responseReaderTask;
                }
            }
            catch (RpcException e)
            {
                Log(" Chat failed {0}", e);
                throw;
            }

        }


        public string Chat(string data)
        {
            try
            {
                var req = new Simplechatbot.ChatRequest();
                id++;
                req.Ids = $"id";
                req.Data = data;
                var rep = client.Chat(req);
                return rep.Data;
            }
            catch (RpcException e)
            {
                Log(" Chat failed {0}", e);
                throw;
            }
        }
    }

}
