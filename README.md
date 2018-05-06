# Auth2 Token işlemleri - Micro servisler için MongoDb ve RabbitMq Kullanımı

Proje debug sırasında webapi ve  rabbit konsolu açar. Database bu sırada oluşturulur(SqlLocalDb).(migration update işlemi webapi startup da içerdeki son hazır migrasyon .cs ine göre çalışır ve yapıyı oluşturur. 
Database e tablolar oluşturulduktan sonra kullanıcı adı ve şifreyi kendiniz belirlemelisiniz.
Yapıdaki örnek gereği sadece zorunlu alan tarih.(02.02.2018) format
Tüm uygulama Generic ve  Asenkron olarak tasarlanmıştır.
Açılan webapi sayfasına /help yazarak metodlara ulaşabilirsiniz.
Port.Bussines: 
# GenericRepository (Abstrack)
Asenkron olarak Dapper uygulanmıştır ve Sadece Stored Procedure Çağırır.(Post - Get)

## Port.Entites :
###### EntityFrameworks(CodeFirst)

Entites klasörü altında Database tablo modelleri ve DbContext mevcut.
User ve User Detail tabloları oluşturulurken cascade olacak şekilde bağlandı.
Migrasyon klasörü içerisinde önceden yaratılmış migration sınıfı mevcut.
Add- Migration = yeni database yapısı oluşturur.
Database – Update = son yaratılan database sınıfını yaratır.
Database oluşturulurken kullanılacak Stored Porcedure’ ler de oluşturulur.
Tools klasörü altında Singlaton Pattern uygulanmış bağlantı sınıfı mevcut.  
Bussines ve Entities projeleri birbirinden bağımsız geliştirilebilir. 

## Port.RestApi: 
###### Asp.Net WebApi Token Based Authentication(Oauth2) - Owin


Context Burada üretilir. Yani müşteriye gittiniz wep apiyi kurdunuz. İlk token isteği ile database oluşturulur. Denemedim ama böyle 😊
Help sayfası oluşturmak için alltaki Nuget WebApiTestClient paketi kullanıldı ve Custom LaWebApi.Xml oluşturuldu.
Help page xml i güncellemek için rebuild işleminden önce App_Data ve Bin klasörlerinden xml silinmeli.
HelpPage Url(Debug sırasında)= http://localhost:42875/help


Model ve entities nesnelerinin Json formata cevrilebilmesi ve collection property lerine ulaşabilmek için

![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/used_Json.png)
 
## Login İşlemleri ve Bearer Token Üretimi


###### Owin alt yapısı ile kurulan RestApi servislerinin Test arayüzünü oluşturmak

![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/help_page_1.png)

Rebuild sırasında oluşturulacak olan xml için  istenilen test sınıf ve metodlarını tutacak xml in oluşturulması.
Geliştirme sırasında görünürlük kontrolü = [ApiExplorerSettings(IgnoreApi = true)]

![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/help_page_2.png)

###### Help Page 

![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/help_page.png)

Help page ana sayfasından test login linkine tıkanır ve login giriş ekranı alt sağdaki popup butonu ile açılır.
Json Formatında veri gönderilir. 
 
###### Help Page Login 
 
 ![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/login_api.png)
 
###### Result Status Rasponse Code : 200/OK 

 ![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/login_result_api.png)
 

###### Rasponse içeriği : 
access_token":"üretilen_token",
"token_type":"bearer",
"expires_in":86399,
"userName":"a",
"userId":"1",
"userRole":"",
".issued":"Thu, 19 Apr 2018 19:13:29 GMT",
".expires":"Fri, 20 Apr 2018 19:13:29 GMT"


###### Post işlemi ve Token Kullanımı

 ![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/post_api.png)

RabbitPostController sınıfındaki CreateUser metoduna kullanıcı Json olarak post edilir.
Gelen isteği RabbitMq kuyruğuna  declare edecek olan sınıf Generic olarak geliştirildi. 



## Port.RabbitMqListener: 

###### RabbitMq 

RabbitMq kurulumu tamamlandıtan sonra localhostunuzdaki şu adres ve bilgiler ile arayüze giriş yapabilirsiniz.
Queue tabında gönderilmiş istekleri takip edebilirsiniz.
Kullanıcı=quest
Şifre =guest


 ![alt text](https://github.com/okandunay/Auth2_And_RabbitMQ/blob/okandunay/readme_images/rabbitMq_Localhost.png)
 
 
###### RabbitMq Consumer - AspNet Core Console App.
Dinlenecek kuyruk belirlenirken gönderimi yapılırken belirlenen queue adı ile aynı olmalı ve gelen ve karşılayan sınıfların
yapılarıda aynı olmalıdır.
PostRabbitMq.cs = channel.QueueDeclare(queue: data.GetType().Name,
Program.cs = userModelChannel.QueueDeclare(queue: "Port_User",

oluşturulan kuyruk burada dinlenir ve gelen istek yakalanır.
kuyruk  ilk giren ilk çıkar (FIFO) kuralına göre çalışır ve istek alındıktan sonra kuyruktan silinir.
MongoDb işlemlerine geçiş.

###### İşlem Geri Bildirim
Status CPM ‘e update edilir.
Hata oluşursa status ERR olur.
Şimdilik işlemi görmek acısından aynı sınıf içerisinde bulunan ConnetcAndProccessStart metodu ile işlemleri yapılmaktadır.
Geliştirme buradan devam etmekte.
