# Yüksel Algoritma Blog

![AnaSayfa](http://yukselalgoritma.net/Areas/Blog/Uploads/Anasayfa.PNG)

Web Sayfası [yukselalgoritma.net](http://yukselalgoritma.net)

> Yüksel Algoritma Blog, ***Asp.Net Mvc*** kullanılarak yazılan, içerisinde birçok özellikler buluduran ve bilinen web açıklarına karşı aldığı önemler ile çok amaçlı bir web sitesidir

## Kullanılan Kütüphaneler ve Teklonojiler

**Front-End**
* Bootstrap 4
* Jquery 3.2.1
* Fastclick
* HTML5 Shiv 3.7.2
* JPanelmenu
* Modernizr
* Retina.js
* Jquery.From
* jquery.unobtrusive
* Jquery-validate
* ckeditor

**Back-end**
* Asp.Net Mvc
* Entity Framework
* FakeData
* Newtonsolf.Json
* Asp.Net Web Optimization
* Web Helpers

## Genel Özellikler

* Admin tarfında yapılan ***bütün aksiyonların loglanması***

* Client tarfında, ***kullanıcıdan istenen bütün inputlar için doğrulama*** yapılması

* ***CSRF*** saldırılarına karşı sayfalarda token oluşturma

* ***XSS*** saldırılarına karşı, ***kullanıcıların inputların encode etme***

* ***Brute Force*** saldırılara karşı captcha kullanma

* Sıklıkla kullanılan öğeleri ***cacheleme***

* ***Seo dostu*** Url'ler

* Hassas verilerde, ***SHA256*** şifreleme algoritmasıyla verileri şifreleme

* ***cookie*** kullanılarak otomatik login

## İçerikler

* Haber **ekleme**, **güncelleme**, **silme**, **taslak olarak kaydetme**  ***(ajax ile)***

* Kategori **ekleme**, **güncelleme**, **silme**

* Dosya **yükleme**, **silme** ***(ajax ile)***

* Kullanıcı **kayıt**, **silme**, **login**

* Kullanıcı **profil sayfası**, **bilgilerini güncelleme**

* Kullanıcı **hesap aktifleştirme** ***(token ile)*** , **şifre resetleme** ***(token ile)***

* Sitenin belli özeliklerini **kişileştirebilme(ismi, bilgisi, anasayfa resmi...)**

* Takipci **ekleme**, **çıkarma** ***(token ile)*** , **bildirim gönderme**

* **Arama**,kategorilere göre **filitreleme**

* Comment, Subcomment **ekleme**, **silme**  ***Ajax ile***

* Kullanicilar arası **rollendirme** ***(admin,user)***

* Kullanıcıların **rollerine göre erişim sağlama**

## Kurulum

Öncelikle [Web.Config](yA%20Blog/Web.config) daki bazı ayarları güncellemiz gerekmetedir.

```sh
<add key="recaptcha_sitekey" value="6LcncjwUAAAAAHclYUU7yXyvVaOMZd_FLOCcb0mJ" />
<add key="recaptcha_privatekey" value="6LcncjwUAAAAACSXIynAx_42X0UteOk0VeXkPBVY" />
```
Bu kısımda `recaptcha_sitekey` ve `recaptcha_privatekey` anahtar değer ikililerine, kendi değerlerimizi giriyoruz. Bu bilgiler [reCAPTCHA](https://www.google.com/recaptcha/intro/android.html) adresinden alınabilir

```sh
<add key="mailUser" value="mailadresi@adres.com"/>
<add key="mailPass" value="password"/>
<add key="mailHost" value="host"/>
<add key="mailPort" value="portNo"/>
```
Bu kısımda iligili yerlere hangi mail adersi kullanacaksak, onların bilgilerini giriyoruz.

```sh
<add key="SiteRootUri" value="http://localhost:55556/"/>
```

Uygulamayı ***Deploy*** edeceğimiz ***domain*** adresini giriyoz.

```sh
  <connectionStrings>
    <add name="DatabaseContext" connectionString="Data Source=.;Initial Catalog=deneme;Integrated Security=True" providerName="System.Data.SqlClient" />
  </connectionStrings>
```

Son olarak da, veritabanına bağlanacak connection stringimizi ayarlama kalıyor. Veritabanı ayarlarınıda yaptıktan sonra artık uygulamayı çalıştırmaya hazırız.

Uygulama çalışmaya başlarken ***veritabanı yoksa otomatik oluşturulur*** ve FakeData kullanılarak veriler otomatik Insert edilir.Bu sizin uygulamayı daha iyi anlamanıza yardımcı olacaktır.

Verileri kendiniz oluşturmak istiyorsanız, [DatabaseContext](yA%20Blog/Areas/Blog/Models/Managers/DatabaseContext.cs) dosyasında,

```sh
  public class VeriTabaniOlusturucu : CreateDatabaseIfNotExists <DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
        }
    }
```
***VeriTabaniOlusturucu*** Sınıfının ***Seed*** metodunu özelleştirebilirsiniz. **Diğer kişileştirmeler için** admin olarak giriş yapıp
```sh
domain/Admin/Ayarlar
```
Adresinden yapabiliriniz.

## Ekran Görüntüleri

#### Haber Listeleme Ekranı

![Haberlisteleme](http://yukselalgoritma.net/Areas/Blog/Uploads/Admin.PNG)

#### Dosya Yükleme

![DosyaYukele](http://yukselalgoritma.net/Areas/Blog/Uploads/DosyaYukleme.PNG)

#### Yorumlar - Admin

![Yorumlar](http://yukselalgoritma.net/Areas/Blog/Uploads/yorumlar.PNG)

### Login Captcha

![Captcha](http://yukselalgoritma.net/Areas/Blog/Uploads/gunvelik.PNG)
