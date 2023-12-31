﻿using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Test;

// resources in this file have been copied from the template generated by IdentityServer
namespace Ids
{
    public static class Config
    {
        // users of our application
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "One Hacker Way",
                    locality = "Heidelberg",
                    postal_code = 69118,
                    country = "Germany"
                };

                return new List<TestUser>
                {
                  new TestUser
                  {
                    SubjectId = "818727",
                    Username = "alice",
                    Password = "alice",
                    Claims =
                    {
                      new Claim(JwtClaimTypes.Name, "Alice Smith"),
                      new Claim(JwtClaimTypes.GivenName, "Alice"),
                      new Claim(JwtClaimTypes.FamilyName, "Smith"),
                      new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                      new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                      new Claim(JwtClaimTypes.Role, "admin"),
                      new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                      new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                        IdentityServerConstants.ClaimValueTypes.Json)
                    }
                  },
                  new TestUser
                  {
                    SubjectId = "88421113",
                    Username = "bob",
                    Password = "bob",
                    Claims =
                    {
                      new Claim(JwtClaimTypes.Name, "Bob Smith"),
                      new Claim(JwtClaimTypes.GivenName, "Bob"),
                      new Claim(JwtClaimTypes.FamilyName, "Smith"),
                      new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                      new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                      new Claim(JwtClaimTypes.Role, "user"),
                      new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                      new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address),
                        IdentityServerConstants.ClaimValueTypes.Json)
                    }
                  }
                };
            }
        }

        // a resource is something that we want to protect - here we have an identity resource
        public static IEnumerable<IdentityResource> IdentityResources =>
          new[]
          {
              // these first two resources, OpenId and Profile are standard OpenId Connect scopes we want IdentityServer to support
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
              Name = "role",
              UserClaims = new List<string> {"role"}
            }
          };

        // Configuration associated with clients
        public static IEnumerable<ApiScope> ApiScopes =>
          new[]
          {
            new ApiScope("weatherapi.read"),
            new ApiScope("weatherapi.write"),
          };

        // a resource is something that we want to protect - here, we have an API resource
        public static IEnumerable<ApiResource> ApiResources => new[]
        {
            // it's associated with two scopes - something that can read information from and update information using this api
            new ApiResource("weatherapi")
            {
                Scopes = new List<string> {"weatherapi.read", "weatherapi.write"},
                ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };

        // client applications taht will be talking to IdentityServer to be asked to be allwoed to user our application
        public static IEnumerable<Client> Clients =>
          new[]
          {
            // m2m client credentials flow client
            // Machine to Machine or API client
            new Client
            {
              ClientId = "m2m.client",
              ClientName = "Client Credentials Client",

              AllowedGrantTypes = GrantTypes.ClientCredentials,
              // the .Sha256() method hashes the secret string
              ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

              AllowedScopes = {"weatherapi.read", "weatherapi.write"}
            },

            // interactive client using code flow + pkce
            // Interactive Client of Web client
            new Client
            {
              ClientId = "interactive",
              ClientSecrets = {new Secret("SuperSecretPassword".Sha256())},

              AllowedGrantTypes = GrantTypes.Code,

              RedirectUris = {"https://localhost:5444/signin-oidc"},
              FrontChannelLogoutUri = "https://localhost:5444/signout-oidc",
              PostLogoutRedirectUris = {"https://localhost:5444/signout-callback-oidc"},

              AllowOfflineAccess = true,
              AllowedScopes = {"openid", "profile", "weatherapi.read"},
              RequirePkce = true,
              RequireConsent = true,
              AllowPlainTextPkce = false
            },
          };
    }
}