using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Exception;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.ReportService;
using Intuit.Ipp.Security;
using libqbo.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;

namespace libqbo;

/// <summary>
/// Wraps the Quickbooks Online API.
/// </summary>
public class QboClient
{
    private readonly QboAuthenticationHandler _authHandler;
    private readonly QboConfiguration _configuration;

    public QboClient(QboAuthenticationHandler authHandler, IOptions<QboConfiguration> configuration)
    {
        _authHandler = authHandler;
        _configuration = configuration.Value;
    }

    /// <summary>
    /// Get the URL to redirect the user to for authentication.
    /// </summary>
    public string GetAuthorizationUrl(params OidcScopes[] scopes)
    {
        return _authHandler.GetAuthorizationURL(scopes);
    }

    /// <summary>
    /// Call this method with the query string received from QuickBooks after user authorization.
    /// </summary>
    public async Task<bool> HandleAuthenticationCallback(string queryString, bool suppressErrors = true)
    {
        return await _authHandler.CheckQueryParamsAndSet(queryString, suppressErrors);
    }

    private async Task<ServiceContext> GetServiceContext()
    {
        var token = await _authHandler.GetValidToken();

        var oauthValidator = new OAuth2RequestValidator(token.AccessToken);

        var serviceContext = new ServiceContext(token.RealmId, IntuitServicesType.QBO, oauthValidator);
        serviceContext.IppConfiguration.MinorVersion.Qbo = "75";
        serviceContext.IppConfiguration.Message.Response.SerializationFormat = Intuit.Ipp.Core.Configuration.SerializationFormat.Json;
        serviceContext.IppConfiguration.Message.Request.SerializationFormat = Intuit.Ipp.Core.Configuration.SerializationFormat.Json;
        serviceContext.IppConfiguration.BaseUrl.Qbo = _configuration.UseSandbox ? "https://sandbox-quickbooks.api.intuit.com/" : "https://quickbooks.api.intuit.com/";

        return serviceContext;
    }

    public async Task<QboWrappedReport> GetReport(QuickBooksReportType report, Action<ReportService> configure)
    {
        var service = new ReportService(await GetServiceContext());
        configure(service);

        var result = await System.Threading.Tasks.Task.Run(() =>
        {
            return service.ExecuteReport(report.ToString());
        });

        return new QboWrappedReport(result);
    }

    /// <summary>
    /// Wraps the <see cref="QueryService{T}"/> to run a query against QuickBooks Online.
    /// </summary>
    /// <example>
    /// var result = await qboClient.RunQuery<Customer>("SELECT * FROM Customer");
    /// </example>
    public async Task<ReadOnlyCollection<T>> RunQuery<T>(string query)
    {
        var service = new QueryService<T>(await GetServiceContext());

        var result = await System.Threading.Tasks.Task.Run(() =>
        {
            return service.ExecuteIdsQuery(query);
        });

        return result;
    }

    public async IAsyncEnumerable<T> RunListQuery<T>(string query)
    {
        var service = new QueryService<T>(await GetServiceContext());
        var pageSize = 100;
        var startPosition = 1;
        ReadOnlyCollection<T> result;
        do
        {
            var pagedQuery = $"{query} STARTPOSITION {startPosition} MAXRESULTS {pageSize}";
            result = await System.Threading.Tasks.Task.Run(() =>
            {
                return service.ExecuteIdsQuery(pagedQuery);
            });

            foreach (var item in result)
            {
                yield return item;
            }

            startPosition += pageSize;
        } while (result.Count == pageSize);
    }

    public async Task<Item?> GetItem(string id)
    {
        var query = await RunQuery<Item>($"SELECT * FROM Item WHERE Id = '{id}'");
        return query.FirstOrDefault();
    }

    public Task<Item?> GetItem(ReferenceType reference) => GetItem(reference.Value);

    /// <summary>
    /// The ID of the account. 
    /// Note: This is different than the GL account number.
    /// </summary>
    public async Task<Account?> GetAccount(string id)
    {
        var d = new DataService(await GetServiceContext());
        var result = d.FindById<Account>(new Account() { Id = id });
        return result;

        var query = await RunQuery<Account>($"SELECT * FROM Account WHERE Id = '{id}'");
        return query.FirstOrDefault();
    }

    public Task<Account?> GetAccount(ReferenceType reference) => GetAccount(reference.Value);
}
