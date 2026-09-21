namespace AKERP.Domain;

public static class FeatureKeys
{
    public const string VehicleNumber = "vehicle_number";
    public const string SalesApproval = "sales_approval_flow";
    public const string AdvancedReports = "advanced_reports";
    public const string MultiWarehouse = "multi_warehouse";

    public static IReadOnlyList<(string Key, string TitleAr)> Catalog { get; } =
    [
        (VehicleNumber, "رقم السيارة على الفاتورة"),
        (SalesApproval, "اعتماد المبيعات قبل الترحيل"),
        (AdvancedReports, "تقارير متقدمة"),
        (MultiWarehouse, "تعدد المخازن")
    ];
}

public static class SettingKeys
{
    public const string Currency = "currency";
    public const string TaxPercent = "tax_percent";
    public const string CommissionMethod = "commission_method";
}
