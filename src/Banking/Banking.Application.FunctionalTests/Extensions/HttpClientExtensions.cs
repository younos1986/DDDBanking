using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Banking.Application.FunctionalTests.Extensions
{
    static class HttpClientExtensions
    {
        public static HttpClient CreateIdempotentClient(this TestServer server, Uri uri)
        {
            var client = server.CreateClient();
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());
            return client;
        }
    }
}