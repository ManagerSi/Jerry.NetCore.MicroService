using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;

namespace Jerry.AuthenticationCenter
{
    public class IdentityConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource>
            {
                new ApiResource("gateway_api","gateway service"),

                new ApiResource("user_api","user_api service"),
                //并且要把contactapi加入到apiResource,并加入到 client的allowedScopes中 
                // new ApiResource("contact_api","contact_api service")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId="pc",
                    //AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, //这里是指定授权的模式，选择密码模式，
                    AllowedGrantTypes = GrantTypes.ClientCredentials, //这里是指定授权的模式
                    ClientSecrets = { new Secret("jerry".Sha256()) },
                    RefreshTokenUsage=TokenUsage.ReUse,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    AllowedScopes=new List<string>
                    {
                        "user_api",
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    }

                }
            };
        }
    }
}