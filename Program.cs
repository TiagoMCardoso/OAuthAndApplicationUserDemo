using Microsoft.Crm.Sdk.Messages;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.WebServiceClient;
using System;

namespace TMC.OAuthAndApplicationUserDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceUrl = "https://tiagomvpdev.crm6.dynamics.com";
            string clientAppId = "APPID";
            string clientSecretId = "SECRETID";
            string tenantId = "TENANTID";
            string authority = $"https://login.microsoftonline.com/{tenantId}";

            try
            {
                AuthenticationContext authContext = new AuthenticationContext(authority);
                ClientCredential credential = new ClientCredential(clientAppId, clientSecretId);

                AuthenticationResult result = authContext.AcquireTokenAsync(serviceUrl, credential).Result;

                using (var proxy = new OrganizationWebProxyClient(GetServiceUrl(serviceUrl), false))
                {
                    proxy.HeaderToken = result.AccessToken;

                    OrganizationRequest request = new OrganizationRequest()
                    {
                        RequestName = "WhoAmI"
                    };

                    WhoAmIResponse response = proxy.Execute(new WhoAmIRequest()) as WhoAmIResponse;
                    Console.WriteLine(response.UserId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Code: {ex.InnerException.HResult}\nMessage: {ex.InnerException.Message}\nStackTrace: {ex.InnerException.StackTrace}");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        static private Uri GetServiceUrl(string organizationUrl)
        {
            return new Uri(organizationUrl + @"/xrmservices/2011/organization.svc/web?SdkClientVersion=9.1");
        }
    }
}
