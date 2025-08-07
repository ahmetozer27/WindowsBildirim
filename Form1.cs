
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
            AuthSecret = "8B0USPMhmouiXxJM8dKryPphpnZUvIm1PqsxCsg1",
            BasePath = "https://bildirimtest-c7f3b-default-rtdb.firebaseio.com"
        };

        IFirebaseClient client;

        private void ShowBalloonTip(string title, string content)
        {
            // Bildirim g�nderme i�lemleri
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = content;
            //notifyIcon1.Icon = SystemIcons.Information; bu da yap�labilir
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;//icon olmazsa �al��maz
            notifyIcon1.ShowBalloonTip(0); // 5000 milisaniye (5 saniye) s�reyle g�ster
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(fc);
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("G�nderici", "G�nderen");
                dataGridView1.Columns.Add("��eri�i", "��erik");
                dataGridView1.Columns.Add("Okunmadurumu", "Okundu mu?");
                FirebaseResponse verial = client.Get(@"Bildirimtbl");
                Dictionary<string, Veri> verie�leme = JsonConvert.DeserializeObject<Dictionary<string, Veri>>(verial.Body.ToString());
                if (verie�leme != null)
                {
                    foreach (var item in verie�leme)
                    {
                        dataGridView1.Rows.Add(item.Value.G�nderen, item.Value.��erik, item.Value.Okundu);

                    }
                }
                timer1.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Veri Taban�na Ba�lant� Sa�lanamad�!");
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {


            if (textBox1.Text != null && textBox2.Text != null && textBox1.Text != "" && textBox2.Text != "")
            {
                Veri veri = new Veri()
                {
                    G�nderen = textBox1.Text,
                    ��erik = textBox2.Text,
                    Okundu = "Okunmad�"
                };
                //veriKaynagi.Add(new Veri { G�nderen = textBox1.Text, ��erik = textBox2.Text, Okundu = "Okunmad�" });

                var setet = client.Set("Bildirimtbl/" + textBox1.Text, veri);
                MessageBox.Show("Veriler Eklendi");
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("G�nderici", "G�nderen");
                dataGridView1.Columns.Add("��eri�i", "��erik");
                dataGridView1.Columns.Add("Okunmadurumu", "Okundu mu?");
                FirebaseResponse verial = client.Get(@"Bildirimtbl");
                Dictionary<string, Veri> verie�leme = JsonConvert.DeserializeObject<Dictionary<string, Veri>>(verial.Body.ToString());
                foreach (var item in verie�leme)
                {
                    dataGridView1.Rows.Add(item.Value.G�nderen, item.Value.��erik, item.Value.Okundu);

                }

                ShowBalloonTip(textBox1.Text, textBox2.Text);
            }
            else
            {
                MessageBox.Show("G�nderici Veya ��erik Bo� B�rak�lamaz!", "Uyar�", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
            if (e != null)
            {
                string balloonTitle = notifyIcon1.BalloonTipTitle;

                // G�venli bir veri yolu olu�turmak i�in ge�erli bir veri yolu yap�s� olu�turun
                string veriYolu = "Bildirimtbl/" + balloonTitle.Replace(".", "_").Replace("#", "_").Replace("$", "_").Replace("[", "_").Replace("]", "_");

                FirebaseResponse veriAl = client.Get(veriYolu);

                // Veriyi sadece bulundu�unda i�lem yap�n
                if (veriAl.Body != "null")
                {
                    Veri secilenVeri = veriAl.ResultAs<Veri>();
                    secilenVeri.Okundu = "Okundu";
                    var setet = client.Update(veriYolu, secilenVeri);

                    if (setet.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        dataGridView1.Rows.Clear();
                        dataGridView1.Columns.Clear();
                        dataGridView1.Columns.Add("G�nderici", "G�nderen");
                        dataGridView1.Columns.Add("��eri�i", "��erik");
                        dataGridView1.Columns.Add("Okunmadurumu", "Okundu mu?");
                        FirebaseResponse verial = client.Get(@"Bildirimtbl");
                        Dictionary<string, Veri> verie�leme = JsonConvert.DeserializeObject<Dictionary<string, Veri>>(verial.Body.ToString());
                        foreach (var item in verie�leme)
                        {
                            dataGridView1.Rows.Add(item.Value.G�nderen, item.Value.��erik, item.Value.Okundu);

                        }

                    }
                    else
                    {
                        MessageBox.Show("Bildirim Okundu olarak i�aretlenirken bir hata olu�tu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bildirim bulunamad� veya zaten okunmu�.", "Uyar�", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    if (item.Value.Okundu == "Okunmad�")
                    {
                        ShowBalloonTip(item.Value.G�nderen.ToString(),item.Value.��erik.ToString());
                        await Task.Delay(10000);
                    }
                }
            }
        }
    }
    public class Veri
    {
        public string G�nderen { get; set; }
        public string ��erik { get; set; }

        public string Okundu { get; set; }
    }
}
