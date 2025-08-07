# Windows Bildirim Projesi (C#)

## Proje Hakkında

Bu proje, C# Windows Forms kullanarak Firebase Realtime Database’deki bildirim verilerini okuyup, Windows sistem bildirimleri (toast balloon notifications) şeklinde gösteren basit bir uygulamadır.  

Yıllar önce yapılmış bir örnek uygulamadır ve Firebase veritabanındaki güncel verileri belirli aralıklarla kontrol ederek, "Okunmadı" durumundaki bildirimleri kullanıcıya bildirir.

---

## Özellikler

- Firebase Realtime Database’e bağlanarak veri okuma ve yazma işlemleri  
- Gelen bildirimleri Windows sistem tepsisinde balon ipucu (Balloon Tip) olarak gösterme  
- Kullanıcı bildirim balonuna tıkladığında, bildirimin "Okundu" olarak işaretlenmesi  
- Bildirimler datagridview üzerinde listelenir  
- Yeni bildirim ekleme imkanı  
- Timer ile Firebase veritabanını düzenli olarak kontrol etme  

---

## Kullanılan Teknolojiler

- C# Windows Forms  
- [FireSharp](https://github.com/ziyasal/FireSharp) — Firebase Realtime Database istemcisi  
- Newtonsoft.Json — JSON serileştirme/deserileştirme için  
- Firebase Realtime Database  

---

## Çalışma Prensibi

1. Uygulama açıldığında Firebase veritabanına bağlanır ve `Bildirimtbl` düğümündeki tüm bildirimleri getirir.  
2. Veriler DataGridView’da gösterilir.  
3. Timer periyodik olarak Firebase’i sorgular. Eğer "Okunmadı" durumunda bildirim varsa, bunlar Windows bildirim balonu olarak gösterilir.  
4. Kullanıcı balon bildirime tıkladığında, ilgili bildirim "Okundu" olarak işaretlenir ve DataGridView güncellenir.  
5. Yeni bildirim eklemek için form üzerindeki ilgili alanlar doldurularak Firebase’e kayıt yapılır.  

---

## Kullanım

1. Projeyi Visual Studio’da açın.  
2. Firebase projenize ait `AuthSecret` ve `BasePath` değerlerini `FirebaseConfig` kısmına girin.  
3. Uygulamayı çalıştırın.  
4. Bildirim ekleyebilir, var olanları görebilir ve okundu durumunu takip edebilirsiniz.  

---

## Önemli Notlar

- Proje basit bir örnek olup, güvenlik ve performans iyileştirmeleri yapılmamıştır.  
- Firebase `AuthSecret` gizli tutulmalı ve asla açık paylaşılmamalıdır.  
- Bildirim sisteminde çok sayıda veri varsa, performans sorunları yaşanabilir.  
- Bildirim balonları Windows sistem bildirim mekanizmasını kullanır, bazı Windows sürümlerinde farklı davranabilir.  

---

## Lisans

Bu proje MIT lisansı ile lisanslanmıştır.

---

## İletişim

Sorularınız ve katkılarınız için açabilirsiniz.

---

