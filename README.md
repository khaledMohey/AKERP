# AKERP

نظام ERP عربي حديث للإيجار والبيع النهائي — كود واحد، واجهة بشكل ويب، وتشغيل ديسكتوب.

## اقرأ أولًا (مهم للفريق و Cursor)

- **دليل النظام PDF:** [`docs/AKERP-SYSTEM-GUIDE.pdf`](docs/AKERP-SYSTEM-GUIDE.pdf)
- **مرجع Agents:** [`AGENTS.md`](AGENTS.md)
- **HTML قابل للطباعة:** [`docs/AKERP-SYSTEM-GUIDE.html`](docs/AKERP-SYSTEM-GUIDE.html)
- تجربة LAN: [`deploy/lan/README.txt`](deploy/lan/README.txt)

## التقنيات

| الطبقة | التقنية |
|--------|---------|
| واجهة ERP | **Blazor Server + MudBlazor** (شكل ويب حديث + RTL) |
| ديسكتوب | **WPF + WebView2** يغلف نفس الواجهة |
| API / لوحة التحكم | **ASP.NET Core** (`AKERP.Api`) |
| الدومين | **AKERP.Domain** |
| التطبيق | **AKERP.Application** |
| البيانات | **EF Core + SQLite (تطوير) / SQL Server (إنتاج)** |

## التشغيل الآن

```bash
# تطوير
dotnet run --project src/AKERP.Web --launch-profile https

# شبكة محلية لتجربة إيجار مع لاب آخر
dotnet run --project src/AKERP.Web --launch-profile lan
```

Login تجريبي: `admin` / `admin123`

## حالة مختصرة

- **منجز:** مرحلة 0 + مرحلة 1 (Login، حسابات العملاء، رخص إيجار/بيع، Users، Features/Settings، LAN desktop)
- **التالي:** مرحلة 2 (Partners → Inventory → Sales → Purchase)

التفاصيل الكاملة في PDF الدليل.
