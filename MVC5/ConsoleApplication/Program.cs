using Crayon.Api.Sdk;
using Crayon.Api.Sdk.Domain.Addresses;
using Crayon.Api.Sdk.Domain.Clients;
using Crayon.Api.Sdk.Domain.CloudProvisioning.CustomerTenants;
using Crayon.Api.Sdk.Domain.CloudProvisioning.Subscriptions;
using Crayon.Api.Sdk.Domain.Common;
using Crayon.Api.Sdk.Domain.InvoiceProfiles;
using Crayon.Api.Sdk.Domain.Products;
using Crayon.Api.Sdk.Domain.MasterData.Publishers;
using Crayon.Api.Sdk.Domain.MasterData.Regions;
using Crayon.Api.Sdk.Domain.Organizations;
using Crayon.Api.Sdk.Domain.Secrets;
using Crayon.Api.Sdk.Domain.Tokens;
using Crayon.Api.Sdk.Domain.Users;
using Crayon.Api.Sdk.Extensions;
using Crayon.Api.Sdk.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using Crayon.Api.Sdk.Exceptions;

namespace ConsoleApplication
{
    public class Program
    {
        public const string ClientId = "";
        public const string ClientSecret = "";
        public const string Username = "";
        public const string Password = "";

        private static readonly CrayonApiClient ApiClient = new CrayonApiClient(CrayonApiClient.ApiUrls.Demo);
        private static Token _myToken;

        private static void Main()
        {
            try
            {
                //Clients
                RunAllFunctionsInSdk();
            }
            catch (ApiHttpException httpException)
            {
                Console.WriteLine("ErrorMessage: " + httpException.Message);
            }

            Console.ReadLine();
        }

        private static void RunAllFunctionsInSdk()
        {
            //Regions
            Console.WriteLine("Regions: ");
            var microsoftRegions = ApiClient.Regions.Get(GetToken(), new RegionFilter { RegionList = RegionList.MicrosoftCsp }).GetData();
            microsoftRegions.Items.ForEach(x => Console.WriteLine(x.Name));

            //Publishers
            var publishers = ApiClient.Publishers.Get(GetToken()).GetData();
            publishers.Items.ForEach(x => Console.WriteLine(x.Name));
            Publisher microsoft = publishers.Items.First(p => p.Name.Equals("Microsoft"));
            microsoft = ApiClient.Publishers.GetById(GetToken(), microsoft.Id).GetData();
            var apiUser = ApiClient.Users.GetByUserName(GetToken(), Username).GetData();

            //Users
            Console.WriteLine("Users: ");
            var newUser = ApiClient.Users.Create(GetToken(), new User { Email = $"myuser_{Guid.NewGuid().ToString().Substring(0, 8)}@mycompany.com", UserName = $"myuser{Guid.NewGuid().ToString().Substring(0, 8)}@mycompany.com" }).GetData();
            newUser.FirstName = "My console user";
            newUser = ApiClient.Users.Update(GetToken(), newUser).GetData();
            var users = ApiClient.Users.Get(GetToken()).GetData();
            users.Items.ForEach(x => Console.WriteLine(x.UserName));
            var resultDeleteUser = ApiClient.Users.Delete(GetToken(), newUser.Id);
            Console.WriteLine("Delete user: " + resultDeleteUser.StatusCode);
            apiUser = ApiClient.Users.GetById(GetToken(), apiUser.Id).GetData();

            //Clients
            Console.WriteLine("Clients: ");
            var newClient = ApiClient.Clients.Create(GetToken(), new Client { ClientName = "My console client" }).GetData();
            newClient = ApiClient.Clients.GetByClientId(GetToken(), newClient.ClientId).GetData();
            newClient.ClientName = "My console client application";
            ApiClient.Clients.Update(GetToken(), newClient).GetData();
            var clients = ApiClient.Clients.Get(GetToken()).GetData();
            clients.Items.ForEach(x => Console.WriteLine(x.ClientName));

            //Secrets
            Console.WriteLine("Secrets: ");
            var secret = ApiClient.Secrets.Create(GetToken(), new Secret { ClientId = newClient.ClientId }).GetData();
            Console.WriteLine("Secret: " + secret.Value);
            ApiClient.Secrets.Delete(GetToken(), secret.Id, newClient.ClientId);

            //Organizations
            Console.WriteLine("Organizations: ");
            var organizations = ApiClient.Organizations.Get(GetToken()).GetData();
            organizations.Items.ForEach(x => Console.WriteLine(x.Name));
            Organization organization = ApiClient.Organizations.GetById(GetToken(), organizations.Items.First().Id).GetData();

            //Access rights
            var access = ApiClient.OrganizationAccess.GetGrant(GetToken(), organization.Id, apiUser.Id).GetData();
            access.Items.ForEach(x => Console.WriteLine(x.Organization.Name));

            //Addresses
            Console.WriteLine("Addresses: ");
            var addresses = ApiClient.Addresses.Get(GetToken(), organization.Id, AddressType.Invoice).GetData().Items;
            addresses.ForEach(x => Console.WriteLine(x.Name));
            var address = ApiClient.Addresses.GetById(GetToken(), organization.Id, addresses.First().Id).GetData();
            Console.WriteLine("Address: " + address.Name);

            //Products
            Console.WriteLine("Products: ");
            var products = ApiClient.AgreementProducts.Get(GetToken(), new AgreementProductFilter { OrganizationId = organization.Id }).GetData();
            products.Items.ForEach(x => Console.WriteLine(x.Name));
            var cspProductsNoAddons = ApiClient.GetMicrosoftCspSeatProducts(GetToken(), new AgreementProductFilter { OrganizationId = organization.Id }, false).GetData();
            cspProductsNoAddons.Items.ForEach(x => Console.WriteLine(x.Name));

            //Billing statements
            Console.WriteLine("Billing statements: ");
            var billingStatements = ApiClient.BillingStatements.Get(GetToken(), new BillingStatementFilter { OrganizationId = organization.Id }).GetData().Items;
            billingStatements.ForEach(x => Console.WriteLine(x.StartDate));
            if (billingStatements.Any())
            {
                var result = ApiClient.BillingStatements.GetAsFile(GetToken(), billingStatements.First().Id);
                Console.WriteLine("Billing statement file: " + result.StatusCode);
            }

            //Agreements
            Console.WriteLine("Agreements: ");
            var agreements = ApiClient.Agreements.Get(GetToken(), new AgreementFilter { OrganizationIds = new List<int> { organization.Id } }).GetData();
            agreements.Items.ForEach(x => Console.WriteLine(x.Name));

            //Blog items
            Console.WriteLine("Blog items: ");
            var blogItems = ApiClient.BlogItems.Get(5).GetData();
            blogItems.Items.ForEach(x => Console.WriteLine(x.Title));

            //Invoice profiles
            Console.WriteLine("Invoice profiles: ");
            var newInvoiceProfile = new InvoiceProfile {
                Organization = new ObjectReference { Id = organization.Id },
                Name = "My console invoice profile",
                InvoiceAddressId = address.Id,
                DeliveryAddress = new AddressData {
                    Name = "My address",
                    City = "My city",
                    CountryCode = "US",
                    ZipCode = "12645",
                    Street = "My street",
                    State = "",
                    County = ""
                }
            };

            var invoiceProfile = ApiClient.InvoiceProfiles.Create(GetToken(), newInvoiceProfile).GetData();
            invoiceProfile.DeliveryAddress.Name = "My console delivery address";
            invoiceProfile = ApiClient.InvoiceProfiles.Update(GetToken(), invoiceProfile).GetData();
            var invoiceProfiles = ApiClient.InvoiceProfiles.Get(GetToken(), new InvoiceProfileFilter { OrganizationId = organization.Id }).GetData();
            invoiceProfiles.Items.ForEach(x => Console.WriteLine(x.Name));
            invoiceProfile = ApiClient.InvoiceProfiles.GetById(GetToken(), invoiceProfile.Id).GetData();

            //Usage records
            Console.WriteLine("Usage records: ");
            var usage = ApiClient.UsageRecords.GetAsGrouped(GetToken(), new UsageRecordGroupedFilter { OrganizationId = organization.Id }).GetData();
            usage.Items.ForEach(x => Console.WriteLine(x.MeterName));

            try
            {
                //Customer tenants
                var newCustomerTenant = new CustomerTenantDetailed {
                    Tenant = new CustomerTenant {
                        Name = "My console customer tenant",
                        CustomerTenantType = CustomerTenantType.T2,
                        DomainPrefix = "mydomainprefix_" + Guid.NewGuid().ToString().Substring(0, 8).Replace("-", ""),
                        Organization = new Organization { Id = organization.Id },
                        Publisher = new ObjectReference { Id = microsoft.Id },
                        InvoiceProfile = new ObjectReference { Id = invoiceProfile.Id }
                    },
                    Profile = new CustomerTenantProfile {
                        Address = new CustomerTenantAddress {
                            FirstName = "Firstname",
                            LastName = "Lastname",
                            AddressLine1 = "Test Address",
                            CountryCode = microsoftRegions.Items.First(r => r.Code == "US").Code,
                            City = "Bellevue",
                            PostalCode = "98005",
                            Region = "WA"
                        },
                        Contact = new CustomerTenantContact {
                            FirstName = "Firstname",
                            LastName = "Lastname",
                            Email = "email@test.se",
                            PhoneNumber = "Test"
                        }
                    }
                };

                newCustomerTenant = ApiClient.CustomerTenants.Create(GetToken(), newCustomerTenant).GetData();
                newCustomerTenant = ApiClient.CustomerTenants.GetDetailedById(GetToken(), newCustomerTenant.Tenant.Id).GetData();
                newCustomerTenant.Tenant.Reference = "My console customer reference";
                newCustomerTenant = ApiClient.CustomerTenants.Update(GetToken(), newCustomerTenant).GetData();
                Console.WriteLine("Customer tenant: " + newCustomerTenant.Tenant.Name);
                var customerTenants = ApiClient.CustomerTenants.Get(GetToken(), new CustomerTenantFilter { OrganizationId = organization.Id, CustomerTenantType = CustomerTenantType.T2 }).GetData();
                customerTenants.Items.ForEach(x => Console.WriteLine(x.Name));

                //Add existing customer tenant
                var existingCustomerTenant = new CustomerTenantDetailed {
                    Tenant = new CustomerTenant {
                        ExternalPublisherCustomerId = newCustomerTenant.Tenant.ExternalPublisherCustomerId,
                        Organization = new Organization { Id = organization.Id }
                    }
                };

                var resultAddExisting = ApiClient.CustomerTenants.CreateExisting(GetToken(), existingCustomerTenant);
                Console.WriteLine("Existing customer tenant: " + resultAddExisting.StatusCode);

                //Subscriptions
                var newSubscription = ApiClient.Subscriptions.Create(GetToken(), new SubscriptionDetailed {
                    Name = "My console subscription",
                    CustomerTenant = new CustomerTenantReference { Id = newCustomerTenant.Tenant.Id },
                    Quantity = 5,
                    Product = new ProductReference {
                        Id = cspProductsNoAddons.Items.First().ProductVariant.Product.Id,
                        PartNumber = cspProductsNoAddons.Items.First().ProductVariant.Product.PartNumber
                    }
                }).GetData();

                newSubscription.Name = "My console csp subscription";
                newSubscription.Status = SubscriptionStatus.CustomerCancellation;
                newSubscription = ApiClient.Subscriptions.Update(GetToken(), newSubscription).GetData();
                newSubscription = ApiClient.Subscriptions.GetById(GetToken(), newSubscription.Id).GetData();
                Console.WriteLine("Subscription: " + newSubscription.Id);
                var subscriptions = ApiClient.Subscriptions.Get(GetToken(), new SubscriptionFilter { OrganizationId = organization.Id }).GetData();
                subscriptions.Items.ForEach(x => Console.WriteLine(x.Name));
            }
            catch (ApiHttpException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string GetToken()
        {
            //Reuse same token until it expires
            if (_myToken != null && _myToken.ExpiresIn > TimeSpan.FromMinutes(5).TotalSeconds)
            {
                return _myToken.AccessToken;
            }

            _myToken = ApiClient.Tokens.GetUserToken(ClientId, ClientSecret, Username, Password).GetData();
            var token = _myToken.AccessToken;

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Unable to get Token." + _myToken?.Error);
            }

            return token;
        }
    }
}