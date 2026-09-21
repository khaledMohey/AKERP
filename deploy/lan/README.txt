# AKERP - تجربة إيجار على شبكة محلية (لابين)

## على جهازك (السيرفر)

1. اعرف IP جهازك على الواي فاي/الراوتر:
   ipconfig
   مثال: 192.168.1.10

2. اسمح للبورت في الفايروول (مرة واحدة كـ Admin):
   netsh advfirewall firewall add rule name="AKERP LAN 5088" dir=in action=allow protocol=TCP localport=5088

3. شغّل السيرفر على الشبكة:
   cd "H:\AK Tech\AKERP"
   dotnet run --project src/AKERP.Web --launch-profile lan

4. جرّب من نفس الجهاز:
   http://127.0.0.1:5088

5. أنشئ حساب عميل إيجار من: حسابات العملاء
   (يوزر/باسورد خاص بالعميل)

## على اللاب التاني (ديسكتوب إيجار)

1. انسخ مجلد النشر بالكامل:
   H:\AK Tech\AKERP\deploy\lan\DesktopClient\

2. افتح server.json وعدّل IP جهاز السيرفر:
   {
     "ServerUrl": "http://192.168.1.10:5088"
   }

3. شغّل AKERP.Desktop.exe
   (لازم WebView2 Runtime — موجود عادة مع Edge/Windows)

4. ادخل بيوزر العميل اللي عملته (رخصة إيجار)

## ملاحظات

- الجهازين على نفس الشبكة (نفس الراوتر)
- لو الصفحة مش بتفتح: الفايروول أو IP غلط
- دي تجربة إيجار محلية — السيرفر عندك والداتا عندك
