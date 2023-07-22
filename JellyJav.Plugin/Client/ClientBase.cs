using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;

namespace JellyJav.Plugin.Client
{
    public class ClientBase
    {
        protected readonly IBrowsingContext context;
        protected readonly HttpClient httpClient;

        public ClientBase()
        {
            httpClient = new();
            context = BrowsingContext.New();
        }

        protected string ReverseName(in string name)
        {
            return string.Join(" ", name.Split(' ').Reverse());
        }

        public HttpClient GetClient()
        {
            return httpClient;
        }
    }
}
