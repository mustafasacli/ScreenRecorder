namespace ScreenRecorder
{
    using Source.Helping;
    using Source.Values;
    using Source.Views;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.IO;
    using System.Security.AccessControl;
    using System.Threading;
    using System.Windows.Forms;

    public class Form1 : System.Windows.Forms.Form
    {
        private volatile bool isStop = true;

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItemExit;
        private System.Windows.Forms.MenuItem menuItemPS;
        private System.Windows.Forms.MenuItem menuItemSS;
        private System.Windows.Forms.MenuItem menuItemSetting;

        private System.ComponentModel.IContainer components;
        protected ImageList imgLstAll;

        private Stopwatch sw = new Stopwatch();
        private volatile int waitMargin = 0;

        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            this.components = new System.ComponentModel.Container();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItemPS = new System.Windows.Forms.MenuItem();
            this.menuItemSS = new System.Windows.Forms.MenuItem();
            this.menuItemSetting = new System.Windows.Forms.MenuItem();

            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.imgLstAll = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            //
            // imgLstAll
            //
            this.imgLstAll.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstAll.ImageStream")));
            this.imgLstAll.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLstAll.Images.SetKeyName(0, "Icon");

            // Initialize contextMenu1
            this.contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { menuItemSetting, menuItemSS, this.menuItemPS, this.menuItemExit });

            // Initialize menuItem1
            this.menuItemExit.Index = 3;
            this.menuItemExit.Text = "E&xit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItem1_Click);

            this.menuItemPS.Index = 2;
            this.menuItemPS.Text = "P&rint Screen";
            this.menuItemPS.Click += delegate
            {
                WaitForIt();
                PrintScreenNew();
            };

            this.menuItemSS.Index = 1;
            this.menuItemSS.Text = "S&tart Print Screen";
            this.menuItemSS.Click += new System.EventHandler(this.menuItemSS_Click);

            this.menuItemSetting.Index = 0;
            this.menuItemSetting.Text = "O&pen Menu";
            this.menuItemSetting.Click += delegate
            {
                OpenSettingsForm();
            };

            // Set up how the form should be displayed.
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Text = "Print Screen Example";
            // Create the NotifyIcon.
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);

            // Handle the DoubleClick event to activate the form.
            notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // The Icon property sets the icon that will appear
            // in the systray for this application.

            //Image im = imgLstAll.Images["Icon"];
            //MemoryStream memoryStream = new MemoryStream();
            //im.Save(memoryStream, ImageFormat.MemoryBmp);

            //notifyIcon1.Icon = new Icon("1484938805_Android.ico");
            notifyIcon1.Icon = Icon.FromHandle(((Bitmap)imgLstAll.Images["Icon"]).GetHicon());
            //memoryStream);
            //"1484938805_Android.ico");

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            notifyIcon1.ContextMenu = this.contextMenu1;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            notifyIcon1.Text = "Print Screen Saver";
            notifyIcon1.Visible = true;

            //this.Load += Form1_Load;
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
            Opacity = 0;
            base.SetVisibleCore(false);
            base.OnLoad(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Hide();
            //this.Visible = false;
            //base.SetVisibleCore(false);
            this.BeginInvoke(new MethodInvoker(delegate
            {
                this.Hide();
            }));
        }

        protected override void Dispose(bool disposing)
        {
            // Clean up any components being used.
            if (disposing)
                if (components != null)
                    components.Dispose();

            base.Dispose(disposing);
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            // Show the form when the user double clicks on the notify icon.

            // Set the WindowState to normal if the form is minimized.
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            // Activate the form.
            this.Activate();
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            // Close the form, which closes the application.
            if (isStop)
                this.Close();
        }

        private void WaitForIt(int milisec = 0)
        {
            Thread.Sleep(
                milisec < AppValues.MinDelayInMSec ? AppValues.MinDelayInMSec : milisec > AppValues.MaxDelayInMSec ? AppValues.MaxDelayInMSec : milisec
                );
        }

        private void PrintScreen()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            Graphics graphics = Graphics.FromImage(printscreen as Image);
            if (AppSettings.IsHQ)
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
            }
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);

            int state = imSave(printscreen);
            //printscreen.Save(AppSettings.SaveFileName);
            //string.Format(string.Concat(AppSettings.SaveFolderPath, "ps_{0:yyyy-MM-dd_HH-mm-ss_FFFF}.jpg"), DateTime.Now), AppSettings.ImgFormat);
        }

        private void PrintScreenNew()
        {
            using (Image printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            using (Graphics graphics = Graphics.FromImage(printscreen))
            {
                if (AppSettings.IsHQ)
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                }
                graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
                int state = imSave(printscreen);
                //printscreen.Save(AppSettings.SaveFileName);
                //string.Format("D:/ScreenRecords/ps_{0:yyyy-MM-dd_HH-mm-ss_FFFF}.jpg", DateTime.Now), AppSettings.ImgFormat);
            }
        }

        private int imSave(Image im)
        {
            int res = 0;
            try
            {
                if (!Directory.Exists(AppSettings.SaveFolderPath))
                    Directory.CreateDirectory(AppSettings.SaveFolderPath, new DirectorySecurity());

                im.Save(AppSettings.SaveFileName);
                res = 1;
            }
            catch (Exception)
            {
                res = -1;
            }

            return res;
        }

        private void menuItemSS_Click(object Sender, EventArgs e)
        {
            isStop ^= true;
            menuItemSS.Text = isStop ? "Start Print Screen" : "Stop Print Screen";

            this.menuItemPS.SetEnableAndVisibility(isStop);
            this.menuItemExit.SetEnableAndVisibility(isStop);
            this.menuItemSetting.SetEnableAndVisibility(isStop);

            //this.menuItemPS.Enabled = isStop;
            //this.menuItemPS.Visible = isStop;
            //this.menuItemExit.Enabled = isStop;
            //this.menuItemExit.Visible = isStop;
            //this.menuItemSetting.Enabled = isStop;
            //this.menuItemSetting.Visible = isStop;

            WaitForIt();

            if (!isStop)
            {
                Thread thrd = new Thread(new ThreadStart(this.PrintScreenThread));
                thrd.Start();
            }
        }

        private void PrintScreenThread()
        {
            do
            {
                sw.Start();
                PrintScreenNew();
                sw.Stop();
                waitMargin = (int)sw.ElapsedMilliseconds;
                sw.Reset();
                if ((AppSettings.ImageDelayTime - waitMargin) > 0)
                    WaitForIt(AppSettings.ImageDelayTime - waitMargin);
            } while (!isStop);
        }

        private void OpenSettingsForm()
        {
            if (isStop)
            {
                FrmSetting frmst = new FrmSetting();
                frmst.Show();
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.imgLstAll = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            //
            // imgLstAll
            //
            this.imgLstAll.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgLstAll.ImageStream")));
            this.imgLstAll.TransparentColor = System.Drawing.Color.Transparent;
            this.imgLstAll.Images.SetKeyName(0, "Icon");
            //
            // Form1
            //
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Form1";
            this.ResumeLayout(false);
        }
    }
}
