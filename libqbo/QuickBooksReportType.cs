namespace libqbo;

/// <summary>
/// Enum representing QuickBooks Reports API Endpoints.
/// </summary>
public enum QuickBooksReportType
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
