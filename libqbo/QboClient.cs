using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
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
        serviceContext.IppConfiguration.MinorVersion.Qbo = "23";
        serviceContext.IppConfiguration.BaseUrl.Qbo = _configuration.UseSandbox ? "https://sandbox-quickbooks.api.intuit.com/" : "https://quickbooks.api.intuit.com/";

        return serviceContext;
    }

    public async Task<Report> GetReport(QuickBooksReport report, Action<ReportService> configure)
    {
        var service = new ReportService(await GetServiceContext());
        var completionSource = new TaskCompletionSource<Report>();
        configure(service);

        service.OnExecuteReportAsyncCompleted = (sender, args) =>
        {
            if (args.Error != null)
            {
                completionSource.SetException(args.Error);
            }
            else
            {
                completionSource.SetResult(args.Report);
            }
        };

        service.ExecuteReportAsync(report.ToString());
        return await completionSource.Task;
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
}


/// <summary>
/// Enum representing QuickBooks Reports API Endpoints.
/// </summary>
public enum QuickBooksReport
{
    /// <summary>
    /// Balance Sheet Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/balancesheet
    /// </summary>
    BalanceSheet,

    /// <summary>
    /// Profit and Loss Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/profitandloss
    /// </summary>
    ProfitAndLoss,

    /// <summary>
    /// Profit and Loss Detail Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/profitandlossdetail
    /// </summary>
    ProfitAndLossDetail,

    /// <summary>
    /// Trial Balance Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/trialbalance
    /// </summary>
    TrialBalance,

    /// <summary>
    /// Statement of Cash Flows Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/cashflow
    /// </summary>
    CashFlow,

    /// <summary>
    /// Inventory Valuation Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/inventoryvaluationsummary
    /// </summary>
    InventoryValuationSummary,

    /// <summary>
    /// Inventory Valuation Detail Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/inventoryvaluationdetail
    /// </summary>
    InventoryValuationDetail,

    /// <summary>
    /// Sales by Customer Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/customersales
    /// </summary>
    CustomerSales,

    /// <summary>
    /// Sales by Product/Service Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/itemsales
    /// </summary>
    ItemSales,

    /// <summary>
    /// Sales by Department Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/departmentsales
    /// </summary>
    DepartmentSales,

    /// <summary>
    /// Sales by Class Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/classsales
    /// </summary>
    ClassSales,

    /// <summary>
    /// Income by Customer Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/customerincome
    /// </summary>
    CustomerIncome,

    /// <summary>
    /// Customer Balance Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/customerbalance
    /// </summary>
    CustomerBalance,

    /// <summary>
    /// Customer Balance Detail Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/customerbalancedetail
    /// </summary>
    CustomerBalanceDetail,

    /// <summary>
    /// A/R Aging Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/agedreceivables
    /// </summary>
    AgedReceivables,

    /// <summary>
    /// A/R Aging Detail Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/agedreceivabledetail
    /// </summary>
    AgedReceivableDetail,

    /// <summary>
    /// Vendor Balance Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/vendorbalance
    /// </summary>
    VendorBalance,

    /// <summary>
    /// Vendor Balance Detail Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/vendorbalancedetail
    /// </summary>
    VendorBalanceDetail,

    /// <summary>
    /// A/P Aging Summary Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/agedpayables
    /// </summary>
    AgedPayables,

    /// <summary>
    /// A/P Aging Detail Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/agedpayabledetail
    /// </summary>
    AgedPayableDetail,

    /// <summary>
    /// Expenses by Vendor Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/vendorexpenses
    /// </summary>
    VendorExpenses,

    /// <summary>
    /// Account List Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/accountlistdetail
    /// </summary>
    AccountListDetail,

    /// <summary>
    /// General Ledger Report
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/generalledgerdetail
    /// </summary>
    GeneralLedgerDetail,

    /// <summary>
    /// Tax Summary Report (France only)
    /// Docs: https://developer.intuit.com/app/developer/qbo/docs/api/accounting/all-entities/taxsummary
    /// </summary>
    TaxSummary
}
