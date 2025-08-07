
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using static WindowsBildirim.Form1;
namespace WindowsBildirim
{
    public partial class Form1 : Form
    {


        List<Veri> veriKaynagi = new List<Veri>();


        public Form1()
        {
            InitializeComponent();

        }
        IFirebaseConfig fc = new FirebaseConfig()
        {
            // Project Settings => Service Accounts => Database Secrets => Secrets
            AuthSecret = "",
            BasePath = ""
        };

        IFirebaseClient client;

        private void ShowBalloonTip(string title, string content)
        {
            // Bildirim gönderme iþlemleri
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = content;
            //notifyIcon1.Icon = SystemIcons.Information; bu da yapýlabilir
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;//icon olmazsa çalýþmaz
            notifyIcon1.ShowBalloonTip(0); // 5000 milisaniye (5 saniye) süreyle göster
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(fc);
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("Gönderici", "Gönderen");
                dataGridView1.Columns.Add("Ýçeriði", "Ýçerik");
                dataGridView1.Columns.Add("Okunmadurumu", "Okundu mu?");
                FirebaseResponse verial = client.Get(@"Bildirimtbl");
                Dictionary<string, Veri> verieþleme = JsonConvert.DeserializeObject<Dictionary<string, Veri>>(verial.Body.ToString());
                if (verieþleme != null)
                {
                    foreach (var item in verieþleme)
                    {
                        dataGridView1.Rows.Add(item.Value.Gönderen, item.Value.Ýçerik, item.Value.Okundu);

                    }
                }
                timer1.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Veri Tabanýna Baðlantý Saðlanamadý!");
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {


            if (textBox1.Text != null && textBox2.Text != null && textBox1.Text != "" && textBox2.Text != "")
            {
                Veri veri = new Veri()
                {
                    Gönderen = textBox1.Text,
                    Ýçerik = textBox2.Text,
                    Okundu = "Okunmadý"
                };
                //veriKaynagi.Add(new Veri { Gönderen = textBox1.Text, Ýçerik = textBox2.Text, Okundu = "Okunmadý" });

                var setet = client.Set("Bildirimtbl/" + textBox1.Text, veri);
                MessageBox.Show("Veriler Eklendi");
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("Gönderici", "Gönderen");
                dataGridView1.Columns.Add("Ýçeriði", "Ýçerik");
                dataGridView1.Columns.Add("Okunmadurumu", "Okundu mu?");
                FirebaseResponse verial = client.Get(@"Bildirimtbl");
                Dictionary<string, Veri> verieþleme = JsonConvert.DeserializeObject<Dictionary<string, Veri>>(verial.Body.ToString());
                foreach (var item in verieþleme)
                {
                    dataGridView1.Rows.Add(item.Value.Gönderen, item.Value.Ýçerik, item.Value.Okundu);

                }

                ShowBalloonTip(textBox1.Text, textBox2.Text);
            }
            else
            {
                MessageBox.Show("Gönderici Veya Ýçerik Boþ Býrakýlamaz!", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            if (e != null)
            {
                string balloonTitle = notifyIcon1.BalloonTipTitle;

                // Güvenli bir veri yolu oluþturmak için geçerli bir veri yolu yapýsý oluþturun
                string veriYolu = "Bildirimtbl/" + balloonTitle.Replace(".", "_").Replace("#", "_").Replace("$", "_").Replace("[", "_").Replace("]", "_");

                FirebaseResponse veriAl = client.Get(veriYolu);

                // Veriyi sadece bulunduðunda iþlem yapýn
                if (veriAl.Body != "null")
                {
                    Veri secilenVeri = veriAl.ResultAs<Veri>();
                    secilenVeri.Okundu = "Okundu";
                    var setet = client.Update(veriYolu, secilenVeri);

                    if (setet.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        dataGridView1.Columns.Add("Gönderici", "Gönderen");
                        dataGridView1.Columns.Add("Ýçeriði", "Ýçerik");
                        dataGridView1.Columns.Add("Okunmadurumu", "Okundu mu?");
                        FirebaseResponse verial = client.Get(@"Bildirimtbl");
                        Dictionary<string, Veri> verieþleme = JsonConvert.DeserializeObject<Dictionary<string, Veri>>(verial.Body.ToString());
                        foreach (var item in verieþleme)
                        {
                            dataGridView1.Rows.Add(item.Value.Gönderen, item.Value.Ýçerik, item.Value.Okundu);

                        }

                    }
                    else
                    {
                        MessageBox.Show("Bildirim Okundu olarak iþaretlenirken bir hata oluþtu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bildirim bulunamadý veya zaten okunmuþ.", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            FirebaseResponse verial = client.Get(@"Bildirimtbl");
            Dictionary<string, Veri> veritimer = JsonConvert.DeserializeObject<Dictionary<string, Veri>>(verial.Body.ToString());
            if (verial.Body != "null")
            {
                foreach (var item in veritimer)
                {
                    if (item.Value.Okundu == "Okunmadý")
                    {
                        ShowBalloonTip(item.Value.Gönderen.ToString(),item.Value.Ýçerik.ToString());
                        await Task.Delay(10000);
                    }
                }
            }
        }
    }
    public class Veri
    {
        public string Gönderen { get; set; }
        public string Ýçerik { get; set; }

        public string Okundu { get; set; }
    }
}
