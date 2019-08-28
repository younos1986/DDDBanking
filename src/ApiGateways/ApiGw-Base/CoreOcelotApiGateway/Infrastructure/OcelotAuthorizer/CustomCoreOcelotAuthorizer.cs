using Core.Ocelot.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreOcelotApiGateway.Infrastructure.Authorizer
{
    public class CustomCoreOcelotAuthorizer : BaseCoreOcelotAuthorizer
    {

        private static ConcurrentDictionary<int, IEnumerable<int>> CachedData = new ConcurrentDictionary<int, IEnumerable<int>>();

        const string DynamicPermissionKey = "DynamicPermissionKey";
        readonly IConfiguration _configuration;
        private IMemoryCache _cache;
        public CustomCoreOcelotAuthorizer(
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _cache = memoryCache;
        }


        public override bool Authorize(HttpContext context)
        {
            var user = context.User;
            var extractedPath = ExtractPath(context.Request.Path);
            var hasAccess = PerformAuthorize(user.Claims, extractedPath.area, extractedPath.controller, extractedPath.action);

            if (hasAccess == false)
                throw new UnauthorizedAccessException("You do not have sufficent access");
            else
                return true;
        }

        public bool PerformAuthorize(IEnumerable<Claim> claims, string area, string controller, string action)
        {
            // for educational purpose
            return true;

            if (!string.IsNullOrEmpty(controller) || !string.IsNullOrEmpty(action))
            {
                var currentClaimValue = $"{area}:{controller}:{action}";
                var hashedCurrentClaimValue = GetSha256Hash(currentClaimValue);

                var intHashCode = GetStableHashCode(hashedCurrentClaimValue);
                var dynamicPermissionKey = CheckIfDynamicPermissionKeyIsReady(claims);

                if (CachedData.TryGetValue(GetStableHashCode(dynamicPermissionKey), out IEnumerable<int> permissions))
                    return permissions.Contains(intHashCode);
            }

            return false;
        }

        private string CheckIfDynamicPermissionKeyIsReady(IEnumerable<Claim> claims)
        {
            var dynamicPermissionKey = claims.FirstOrDefault(q => q.Type == DynamicPermissionKey);
            if (dynamicPermissionKey == null)
                throw new Exception("Login please");

            return dynamicPermissionKey.Value;
        }


        public int GetStableHashCode(string str)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
