# AKERP — System Guide for Humans & Cursor

> هذا الملف مرجع أساسي للمشروع. أي مساهم (إنسان أو Cursor Agent) لازم يقرأه قبل التعديل.
> نسخة PDF: [`docs/AKERP-SYSTEM-GUIDE.pdf`](docs/AKERP-SYSTEM-GUIDE.pdf)

---

## 1) ما هو AKERP؟

**AKERP** نظام ERP عربي قابل للبيع، يستهدف:

- مخزون Inventories
- مبيعات Sales
- مشتريات Purchase
- عملاء وموردين Partners
- مناديب Sales Reps
- حسابات Accounting
- تقارير Reports
- Dashboard

### نموذج المنتج (مهم جدًا)

**كود واحد** للجميع، مع وضعين رخصة:

| الوضع | الاسم التقني | المعنى |
|------|---------------|--------|
| إيجار | `LicenseType.Subscription` | اشتراك، تحكم، تحديثات، يمكن إيقافه |
| بيع نهائي | `LicenseType.Perpetual` | ملكية، غالبًا أوفلاين، بدون تاريخ انتهاء |

**ممنوع** عمل فرع كود منفصل لكل عميل أو نسختين Sale/Rent.

التخصيص يتم عبر:

- `company_id`
- `CompanyFeature` (feature flags)
- `CompanySetting` (سلوك/إعدادات)
- لاحقًا: Custom Fields / Print Templates / Plugins

---

## 2) التقنيات (Tech Stack)

| الطبقة | التقنية |
|--------|---------|
| UI | Blazor Server + MudBlazor (واجهة ويب حديثة + RTL) |
| Desktop | WPF + WebView2 يغلف نفس الواجهة |
| API (لاحقًا للإيجار السحابي) | ASP.NET Core (`AKERP.Api`) |
| Domain | `AKERP.Domain` |
| Application contracts | `AKERP.Application` |
| Data access | `AKERP.Infrastructure` + EF Core |
| DB تطوير | SQLite (`akerp.dev.db`) |
| DB إنتاج متوقع | SQL Server |
| Target framework | .NET 8 |

---

## 3) هيكل الحل

```text
AKERP.sln
├── src/
│   ├── AKERP.Domain          # Entities + Enums + Feature catalog
│   ├── AKERP.Application     # Interfaces / DTOs (لا يعتمد على UI)
│   ├── AKERP.Infrastructure  # EF Core, services, hashing, seed
│   ├── AKERP.Web             # Blazor UI (المصدر الأساسي للواجهة)
│   ├── AKERP.Desktop         # غلاف ديسكتوب + server.json
│   └── AKERP.Api             # جاهز للتوسعة (رخص سحابية / أدمن)
├── deploy/lan/               # نشر تجربة إيجار على شبكة محلية
├── docs/                     # الدليل + PDF
├── AGENTS.md                 # تعليمات لـ Cursor (هذا الملف)
└── README.md
```

### قواعد المعمارية

1. منطق الأعمال في Domain/Application/Infrastructure — ليس داخل Razor إلا للعرض.
2. كل بيانات تشغيل مربوطة بـ `CompanyId` حيث ينطبق.
3. Modular Monolith — لا Microservices الآن.
4. واجهة واحدة للكل؛ Desktop مجرد host.

---

## 4) الكيانات الأساسية (مرحلة 1)

- `Company` — العميل/الشركة
- `License` — رخصة الشركة (`Subscription` / `Perpetual`, Status, Expiry, MaxUsers, Modules)
- `AppUser` — مستخدم داخل شركة (اسم المستخدم فريد على مستوى النظام حاليًا لتسهيل Login)
- `CompanyFeature` — تفعيل/إيقاف خصائص لكل شركة
- `CompanySetting` — مفاتيح إعداد مثل `currency`, `tax_percent`, `commission_method`

Feature catalog موجود في `AKERP.Domain/FeatureCatalog.cs`.

---

## 5) التدفقات المهمة الحالية

### إنشاء حساب عميل
المسار: `/companies` → **إنشاء حساب عميل**

ينشئ مرة واحدة:

1. شركة
2. رخصة (إيجار بمدة أو بيع بدون انتهاء)
3. مستخدم Admin للعميل
4. إعدادات افتراضية

بعد الحفظ تظهر شاشة ملخص (License Key + User).

### إدارة الرخصة
المسار: `/license`

- اختَر العميل من القائمة (ليس فقط الشركة الحالية للجلسة)
- إيجار → يظهر تاريخ انتهاء ويُحفظ في DB
- بيع → `ExpiresAtUtc = null`
- لازم ضغط **حفظ** لتثبيت التغيير في الداتا

### تسجيل الدخول
- تجريبي seed: `admin` / `admin123` (شركة تجريبية)
- أو يوزر أي عميل أنشأته من شاشة العملاء
- الجلسة في `ProtectedSessionStorage` عبر `BrowserCurrentSession`

### تجربة LAN (إيجار على لاب تاني)
- سيرفر: `dotnet run --project src/AKERP.Web --launch-profile lan` → `http://0.0.0.0:5088`
- عميل ديسكتوب: مجلد `deploy/lan/DesktopClient` + تعديل `server.json` لـ IP السيرفر
- التفاصيل: `deploy/lan/README.txt`

---

## 6) حالة المراحل

### تم إنجازه

#### مرحلة 0 — التأسيس
- Solution + المشاريع
- MudBlazor shell عربي RTL
- Desktop WebView2
- README أولي

#### مرحلة 1 — الأساس التشغيلي للمنتج
- Login / Logout / Session
- Companies = حسابات العملاء
- Users داخل الشركة
- Licenses (إيجار/بيع) مع فرق بيانات حقيقي
- Features & Settings لكل company
- Seed بيانات تجريبية
- نشر LAN للتجربة بين جهازين

### Placeholder موجود في الواجهة (لم يُبنَ منطقها بعد)
- `/inventory` المخزون
- `/sales` المبيعات
- `/purchase` المشتريات
- `/partners` عملاء وموردين
- `/reps` المناديب
- `/accounting` الحسابات
- `/reports` التقارير
- Dashboard KPIs ما زالت أصفارًا تجميلية

### المراحل القادمة (بالترتيب الإلزامي تقريبًا)

#### مرحلة 2 — التشغيل اليومي
1. Partners (Customers/Suppliers) + أرصدة افتتاحية
2. Inventory: أصناف، وحدات، مخازن، حركات، تحويل، جرد، حد أدنى
3. Sales: عرض → أمر → فاتورة → مرتجع → تحصيل
4. Purchase: طلب → أمر → استلام → فاتورة → مرتجع
5. ربط الحركات بالمخزون تلقائيًا

#### مرحلة 3 — الميدان والمالية
1. Sales Reps: مناطق، أهداف، عمولات (flat/tiers عبر settings)
2. Accounting: دليل حسابات، قيود يومية (يدوي + آلي من الفواتير)، صندوق/بنك، ذمم
3. Reports أساسية + PDF printing
4. Dashboard أرقام حقيقية

#### مرحلة 4 — المنتج التجاري (إيجار/بيع كمنتج)
1. تفعيل `AKERP.Api` للرخص السحابية
2. Admin Panel للمالك (إيقاف اشتراك، باقات)
3. Auto-update للديسكتوب
4. ملف رخصة موقّع لعملاء البيع الأوفلاين
5. Multi-user على LAN/Cloud بشكل منتج

#### مرحلة 5 — Hybrid Offline للإيجار
1. Local cache / sync
2. Grace period بدون نت
3. حل تعارضات المزامنة

#### مرحلة 6 — تخصيص متقدم
1. Custom fields
2. Print templates لكل شركة
3. Behavior variants أوسع
4. Plugins للطلبات الكبيرة فقط

---

## 7) قواعد ذهبية للمطور / Cursor

1. **لا تفصل كود البيع عن الإيجار** — استخدم `LicenseType` و feature flags.
2. **لا تكتب `if (companyName == "...")`** — استخدم `company_id` + features/settings.
3. أي شاشة تشغيل جديدة لازم تحترم صلاحيات لاحقًا و Company scope.
4. عند إضافة خاصية لعميل واحد: أضفها في الكود العام مطفية، وفعّلها في `CompanyFeature`.
5. عند تغيير سلوك موجود: أضف variant/setting ولا تكسر الافتراضي.
6. UI: حافظ على اتجاه RTL وطابع MudBlazor الحالي (Primary `#0F766E`).
7. كلمات المرور: `PasswordHasher` (SHA256 حاليًا — يمكن ترقية لاحقًا).
8. لا ترفع أسرار أو ملفات `.db` حساسة؛ SQLite للتطوير محلي.
9. قبل إضافة Microservices / Cloud معقد: أكمل مرحلة 2 و 3.
10. حدّث هذا الدليل عند إنهاء مرحلة أو تغيير قرار معماري مهم.

---

## 8) أوامر تشغيل سريعة

```bash
# واجهة تطوير عادية
dotnet run --project src/AKERP.Web --launch-profile https

# سيرفر شبكة محلية لتجربة الإيجار
dotnet run --project src/AKERP.Web --launch-profile lan

# نشر ديسكتوب للاب آخر
powershell -ExecutionPolicy Bypass -File deploy/lan/publish-desktop.ps1 -ServerIp "YOUR_LAN_IP"
```

Login تجريبي: `admin` / `admin123`

---

## 9) ماذا يعمل المساهم الآن؟

الأولوية المقترحة بعد فهم الدليل:

1. مرحلة 2 — ابدأ بـ Partners ثم Inventory ثم Sales/Purchase
2. حافظ على ربط كل شيء بـ `CompanyId`
3. أي خيار خاص بعميل → Feature/Setting
4. حدّث قسم "حالة المراحل" في هذا الملف + PDF عند الإنتهاء من جزء كبير

---

## 10) مصطلحات سريعة

| عربي | إنجليزي في الكود |
|------|-------------------|
| إيجار | Subscription |
| بيع نهائي | Perpetual |
| شركة/عميل | Company |
| رخصة | License |
| تخصيص | Features / Settings |
| مندوب | Rep |
| غلاف ديسكتوب | AKERP.Desktop + WebView2 |
