using linkedlist_quanly.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;



namespace linkedlist_quanly
{
 
    public partial class MainForm : Form
    {
        private SocialMediaLinkedList postList;
        private string currentUser;
        private bool isProfileView = false;
        private FlowLayoutPanel postsPanel;
        private Random random = new Random();


        private void RoundedForm_Load(object sender, EventArgs e)
        {
            int radius = 45; // Bán kính bo tròn
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90); // Góc trên bên trái
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90); // Góc trên bên phải
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90); // Góc dưới bên phải
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90); // Góc dưới bên trái
            path.CloseFigure();
            this.Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Call the base method
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // or SmoothingMode.None


            // Đường kẻ 
            using (Pen pen = new Pen(Color.FromArgb(255, 227, 229, 228), 3)) 
            {
                e.Graphics.DrawLine(pen, new Point(0, 75), new Point(320, 75));
            }
            // Chọn cửa sổ
            using (Pen pen = new Pen(Color.FromArgb(255, 1, 95, 105), 3)) 
            {
                e.Graphics.DrawLine(pen, new Point(5, 72), new Point(105, 72));
            }
            // Đường kẻ 
            using (Pen pen = new Pen(Color.FromArgb(255, 227, 229, 228), 6))
            {
                e.Graphics.DrawLine(pen, new Point(0, 127), new Point(320, 127));
            }
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1065, 450);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }

        public MainForm()
        {
            // Show login form first
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
                currentUser = loginForm.LoggedInUser;
            }

            InitializeComponent();
            postList = new SocialMediaLinkedList();

            // Initialize default posts for new users
            new PostManager().InitializeDefaultPosts(currentUser);

            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += new EventHandler(RoundedForm_Load);
            InitializeUI();

            // Remove AddSamplePosts() since we're now loading from storage

            // Di chuyển form
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
            this.DoubleBuffered = true;
        }

        private void AddSamplePosts()
        {

            postList.AddPost(
                "Hello, world!",
                "https://example.com/hello.jpg",
                currentUser,
                new DateTime(2024, 11, 20, 14, 30, 0) // 20/11/2024 14:30:00
            );

            postList.AddPost(
                "My first video",
                "https://example.com/video.mp4",
                "OtherUser",
                new DateTime(2024, 11, 21, 9, 15, 0) // 21/11/2024 09:15:00
            );

            postList.AddPost(
                "Funny GIF",
                "https://example.com/funny.gif",
                currentUser,
                new DateTime(2024, 11, 21, 16, 45, 0) // 21/11/2024 16:45:00
            );

            postList.AddPost(
                "Beautiful sunset today!",
                "https://example.com/sunset.jpg",
                "OtherUser",
                new DateTime(2024, 11, 22, 10, 20, 0) // 22/11/2024 10:20:00
            );

            postList.AddPost(
                "Just finished my project!",
                null,
                currentUser,
                new DateTime(2024, 11, 22, 11, 30, 0) // 22/11/2024 11:30:00
            );
            postList.AddPost(
                 "I am handsome!",
                null,
                "OthertUser",
                new DateTime(2024, 11, 21, 11, 30, 0) // 22/11/2024 11:30:00
                );
            postList.AddPost(
                 "I get into Havard!",
                null,
                "OthertUser",
                new DateTime(2024, 11, 20, 11, 30, 0) // 22/11/2024 11:30:00
                );
            postList.ShufflePosts(); ;
        }

        /* private DateTime GetRandomTime(int daysBack)
         {
             DateTime now = DateTime.Now;
             int maxMinutesBack = daysBack * 24 * 60;
             int randomMinutes = random.Next(0, maxMinutesBack);
             return now.AddMinutes(-randomMinutes);
         }*/

        private string FormatTimeAgo(DateTime postTime)
        {
            TimeSpan timeDiff = DateTime.Now - postTime;

            if (timeDiff.TotalSeconds < 60)
                return $"{Math.Floor(timeDiff.TotalSeconds)} giây trước";
            if (timeDiff.TotalMinutes < 60)
                return $"{Math.Floor(timeDiff.TotalMinutes)} phút trước";
            if (timeDiff.TotalHours < 24)
                return $"{Math.Floor(timeDiff.TotalHours)} giờ trước";
            if (timeDiff.TotalDays < 7)
                return $"{Math.Floor(timeDiff.TotalDays)} ngày trước";

            return postTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void InitializeUI()
        {
            this.Size = new Size(320, 650);
            //Icon Social+ 
            System.Windows.Forms.Label app_name = new System.Windows.Forms.Label
            {
                Text = "facebook",
                ForeColor = Color.FromArgb(255, 1, 95, 105),
                Location = new Point(3, 5),
                Size = new Size(140, 40), // Kích thước cố định
                Font = new Font("Klavika1", 20, FontStyle.Bold),
            };

            Panel navigationPanel = new Panel
            {
                Size = new Size(320, 40),
                Location = new Point(0, 35),
                BackColor = Color.Transparent
            };

            RoundedPictureBox homeButton = new RoundedPictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(100, 30),
                Image = ResizeImage(Image.FromFile("Resources/home.png"), 30, 30),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent

            };

            // Add mouse event handlers for click behavior
            homeButton.MouseEnter += (s, e) => homeButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            homeButton.MouseLeave += (s, e) => homeButton.BackColor = Color.Transparent; // Reset hover effect*/

            RoundedPictureBox profileButton = new RoundedPictureBox
            {
                Location = new Point(110, 5),
                Size = new Size(100, 30), 
                Image = ResizeImage(Image.FromFile("Resources/profile.png"), 30, 30),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };
            // Add mouse event handlers for click behavior
            profileButton.MouseEnter += (s, e) => profileButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            profileButton.MouseLeave += (s, e) => profileButton.BackColor = Color.Transparent; // Reset hover effect*/

            RoundedPictureBox notificationButton = new RoundedPictureBox
            {
                Location = new Point(220, 5),
                Size = new Size(100, 30),
                Image = ResizeImage(Image.FromFile("Resources/bell.png"), 30, 30),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };
            notificationButton.MouseEnter += (s, e) => notificationButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            notificationButton.MouseLeave += (s, e) => notificationButton.BackColor = Color.Transparent; // Reset hover effect*/

            navigationPanel.Controls.AddRange(new Control[] { homeButton, profileButton, notificationButton });

            Panel uploadPanel  = new Panel
            {
                Size = new Size(320, 40),
                Location = new Point(0, 85)
            };

            RoundedPictureBox uploadButton = new RoundedPictureBox
            {
                CornerRadius = 30,
                Location = new Point(55, 0),
                Size = new Size(225, 30),
                DisplayText = $"{currentUser} ơi, bạn đang nghĩ gì?", // Updated text with username
                TextColor = Color.Black,
                TextStartX = 11,
                TextFont = new Font("Arial", 10, FontStyle.Regular),
                BackColor = Color.Transparent,
                ShowBorder = true,
                BorderColor = Color.FromArgb(255, 227, 229, 228),
                BorderThickness = 4
            };
            uploadButton.MouseEnter += (s, e) => uploadButton.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            uploadButton.MouseLeave += (s, e) => uploadButton.BackColor = Color.Transparent; // Reset hover effect*/

            RoundedPictureBox uploadPicture = new RoundedPictureBox
            {
                Location = new Point(283, 0),
                Size = new Size(30, 30),
                Image = ResizeImage(Image.FromFile("Resources/photos.png"), 30, 30),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };
            uploadPicture.MouseEnter += (s, e) => uploadPicture.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            uploadPicture.MouseLeave += (s, e) => uploadPicture.BackColor = Color.Transparent; // Reset hover effect*/

            RoundedPictureBox uehLogo = new RoundedPictureBox
            {
                Location = new Point(9, 2),
                Size = new Size(40, 25),
                Image = ResizeImage(Image.FromFile("Resources/logo.png"), 40, 25),
                SizeMode = PictureBoxSizeMode.CenterImage, // Adjust image to fit
                BackColor = Color.Transparent // Optional: make background transparent
            };
            uehLogo.MouseEnter += (s, e) => uehLogo.BackColor = Color.FromArgb(255, 227, 229, 228); // Optional: hover effect
            uehLogo.MouseLeave += (s, e) => uehLogo.BackColor = Color.Transparent; // Reset hover effect*/

            uploadPanel.Controls.AddRange(new Control[] { uploadButton,uploadPicture, uehLogo });





            TextBox contentBox = new TextBox
            {
                Multiline = true,
                Size = new Size(400, 20),
                Location = new Point(20, 50)
            };

            Button postBtn = new Button
            {
                Text = "Đăng bài",
                Location = new Point(120, 180)
            };

            postsPanel = new FlowLayoutPanel
            {
                Size = new Size(960, 430),
                Location = new Point(20, 220),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            homeButton.Click += (s, e) =>
            {
                isProfileView = false;
                RefreshPosts();
            };

            profileButton.Click += (s, e) =>
            {
                isProfileView = true;
                RefreshPosts();
            };

            string selectedMediaPath = "";
            uploadPicture.Click += (s, e) =>
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Media files (*.jpg, *.gif, *.mp4)|*.jpg;*.gif;*.mp4";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        selectedMediaPath = ofd.FileName;
                    }
                }
            };

            postBtn.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(contentBox.Text))
                {
                    postList.AddPost(contentBox.Text, selectedMediaPath, currentUser);
                    RefreshPosts();
                    contentBox.Clear();
                    selectedMediaPath = "";
                }
            };

            this.Controls.AddRange(new Control[] {
                //contentBox,
                navigationPanel,
                uploadPanel,
                //postBtn,
                postsPanel,
                app_name
            });

            RefreshPosts();
        }

        private void RefreshPosts()
        {
            postsPanel.Controls.Clear();

            // Lấy danh sách bài viết tùy theo chế độ xem
            var posts = isProfileView
                ? postList.GetUserPosts(currentUser)
                : postList.GetAllPosts().Where(post => post.Author != currentUser).ToList();

            foreach (var post in posts)
            {
                Panel postPanel = new Panel
                {
                    Size = new Size(920, 350),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(0, 0, 0, 10)
                };

                // Thông tin tác giả và thời gian
                Panel headerPanel = new Panel
                {
                    Size = new Size(900, 30),
                    Location = new Point(10, 10)
                };

                System.Windows.Forms.Label authorLabel = new System.Windows.Forms.Label
                {
                    Text = post.Author,
                    Font = new Font(this.Font, FontStyle.Bold),
                    Location = new Point(0, 5),
                    AutoSize = true
                };

                System.Windows.Forms.Label timeLabel = new System.Windows.Forms.Label
                {
                    Text = $"• {FormatTimeAgo(post.PostTime)}",
                    ForeColor = Color.Gray,
                    Location = new Point(authorLabel.Right + 10, 5),
                    AutoSize = true
                };

                // Tooltip cho thời gian chính xác
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(timeLabel, post.PostTime.ToString("dd/MM/yyyy HH:mm:ss"));

                headerPanel.Controls.AddRange(new Control[] { authorLabel, timeLabel });

                System.Windows.Forms.Label contentLabel = new System.Windows.Forms.Label
                {
                    Text = post.Content,
                    Size = new Size(900, 60),
                    Location = new Point(10, 45)
                };

                if (!string.IsNullOrEmpty(post.MediaReference))
                {
                    string extension = Path.GetExtension(post.MediaReference).ToLower();
                    if (extension == ".jpg" || extension == ".gif")
                    {
                        PictureBox pictureBox = new PictureBox
                        {
                            ImageLocation = post.MediaReference,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Size = new Size(500, 300),
                            Location = new Point(10, 110)
                        };
                        postPanel.Controls.Add(pictureBox);
                    }
                    else if (extension == ".mp4")
                    {
                        System.Windows.Forms.Label mediaLabel = new System.Windows.Forms.Label
                        {
                            Text = "Selected video: " + Path.GetFileName(post.MediaReference),
                            Location = new Point(10, 110),
                            Size = new Size(300, 20)
                        };
                        postPanel.Controls.Add(mediaLabel);
                    }
                }

                postPanel.Controls.AddRange(new Control[] {
            headerPanel,
            contentLabel
        });

                postsPanel.Controls.Add(postPanel);
            }
        }


        //Handle button's icon
        private void SetButtonImage(Button button, string imagePath)
        {
            Image originalImage = Image.FromFile(imagePath);
            button.Image = ResizeImage(originalImage, button.Width, button.Height);
        }

        /*        private Image ResizeImage(Image img, int width, int height)
                {
                    // Calculate the aspect ratio
                    float ratioX = (float)width / img.Width;
                    float ratioY = (float)height / img.Height;
                    float ratio = Math.Min(ratioX, ratioY);

                    // Calculate the new dimensions
                    int newWidth = (int)(img.Width * ratio);
                    int newHeight = (int)(img.Height * ratio);

                    // Create a new bitmap with the new dimensions
                    Bitmap newImage = new Bitmap(newWidth, newHeight);
                    using (Graphics g = Graphics.FromImage(newImage))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(img, 0, 0, newWidth, newHeight);
                    }
                    return newImage;
                }*/
        private Image ResizeImage(Image img, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic; // Set high-quality interpolation
                g.DrawImage(img, 0, 0, width, height);
            }
            return resizedImage;
        }


        //Xử lý phần di chuyển 
        // Mouse down event to start dragging the form
        // Import necessary functions from user32.dll
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 0x0002;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        // Optionally, you can handle the MouseMove event if you want to do something while dragging
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // You can add any code here if you want to do something while dragging
        }

        // Optionally, you can handle the MouseUp event if you want to do something after dragging
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // You can add any code here if you want to do something after dragging
        }

        // Make sure to subscribe to the MouseDown event in the designer or constructor
        private void Form1_Load(object sender, EventArgs e)
        {
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
        }

    }
}

