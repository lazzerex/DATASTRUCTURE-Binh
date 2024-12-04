using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace linkedlist_quanly
{
    public class PostManager
    {
        private const string POSTS_FILE = "posts.txt";
        private string filePath;

        public PostManager()
        {
            string debugPath = Path.GetDirectoryName(Application.ExecutablePath);
            filePath = Path.Combine(debugPath, POSTS_FILE);

            // Create the file if it doesn't exist
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public void SavePost(Post post)
        {
            string postLine = $"{post.Author}|{post.Content}|{post.MediaReference}|{post.PostTime:yyyy-MM-dd HH:mm:ss}\n";
            File.AppendAllText(filePath, postLine);
        }

        public List<PostData> LoadAllPosts()
        {
            var posts = new List<PostData>();

            if (!File.Exists(filePath))
                return posts;

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length >= 4)
                {
                    posts.Add(new PostData
                    {
                        Author = parts[0],
                        Content = parts[1],
                        MediaReference = parts[2] == "null" ? null : parts[2],
                        PostTime = DateTime.Parse(parts[3])
                    });
                }
            }

            return posts;
        }

        public void InitializeDefaultPosts(string username)
        {
            var posts = LoadAllPosts();
            if (!posts.Any(p => p.Author == username))
            {
                var defaultPosts = new List<Post>
                {
                    new Post("Hello! This is my first post!", null, username),
                    new Post("Just joined this amazing platform!", null, username)
                };

                foreach (var post in defaultPosts)
                {
                    SavePost(post);
                }
            }
        }
    }

    public class PostData
    {
        public string Content { get; set; }
        public string MediaReference { get; set; }
        public DateTime PostTime { get; set; }
        public string Author { get; set; }
    }
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AvatarPath { get; set; } // Thêm đường dẫn ảnh đại diện
    }
    /*-------------------------------------------------------------------------Test thu hash password-----------------------------------*/
    /*public class UserManager
    {
        private const string USER_FILE = "users.txt";
        private string filePath;*/

    /* public UserManager()
     {
         // Get the debug folder path
         string debugPath = Path.GetDirectoryName(Application.ExecutablePath);
         filePath = Path.Combine(debugPath, USER_FILE);
     }

     *//*private string HashPassword(string password)  //neu muon ma hoa mat khau de bao mat thi dung
     {
         using (SHA256 sha256 = SHA256.Create())
         {
             byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
             StringBuilder builder = new StringBuilder();
             for (int i = 0; i < bytes.Length; i++)
             {
                 builder.Append(bytes[i].ToString("x2"));
             }
             return builder.ToString();
         }
     }*//*

     public bool RegisterUser(string username, string password)
     {
         try
         {
             // Check if username already exists
             if (File.Exists(filePath))
             {
                 var users = File.ReadAllLines(filePath);
                 if (users.Any(u => u.Split('|')[0] == username))
                 {
                     return false;
                 }
             }

             // Hash password and save user
             *//*string hashedPassword = HashPassword(password);*/
    /* File.AppendAllText(filePath, $"{username}|{hashedPassword}\n");*//*
     File.AppendAllText(filePath, $"{username}|{password}\n");
     return true;
 }
 catch (Exception)
 {
     return false;
 }
}*/
    /*-------------------------------------------------------------------------Test thu hash password-----------------------------------*/
    public class UserManager
    {
        private const string USER_FILE = "users.txt";
        private string filePath;

        public UserManager()
        {
            string debugPath = Path.GetDirectoryName(Application.ExecutablePath);
            filePath = Path.Combine(debugPath, USER_FILE);

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        public bool RegisterUser(string username, string password)
        {
            try
            {
                // Check if username already exists
                if (File.Exists(filePath))
                {
                    var users = File.ReadAllLines(filePath);
                    if (users.Any(u => u.Split('|')[0] == username))
                    {
                        return false;
                    }
                }

                // Save user with default avatar path
                string defaultAvatarPath = "Resources/default-avatar.png";
                File.AppendAllText(filePath, $"{username}|{password}|{defaultAvatarPath}\n");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                var users = File.ReadAllLines(filePath);
                return users.Any(u =>
                {
                    var parts = u.Split('|');
                    return parts[0] == username && parts[1] == password;
                });
            }
            catch (Exception)
            {
                return false;
            }
        }
        public string GetUserAvatar(string username)
        {
            try
            {
                if (!File.Exists(filePath)) return null;

                var users = File.ReadAllLines(filePath);
                var userLine = users.FirstOrDefault(u => u.Split('|')[0] == username);
                if (userLine != null)
                {
                    var parts = userLine.Split('|');
                    return parts.Length >= 3 ? parts[2] : "Resources/default-avatar.png";
                }
            }
            catch (Exception) { }

            return "Resources/default-avatar.png";
        }

        public bool UpdateUserAvatar(string username, string newAvatarPath)
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                var users = File.ReadAllLines(filePath).ToList();
                for (int i = 0; i < users.Count; i++)
                {
                    var parts = users[i].Split('|');
                    if (parts[0] == username)
                    {
                        // Update avatar path while keeping username and password
                        users[i] = $"{parts[0]}|{parts[1]}|{newAvatarPath}";
                        File.WriteAllLines(filePath, users);
                        return true;
                    }
                }
            }
            catch (Exception) { }

            return false;
        }
    }
    public class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private PictureBox picAvatar;
        private UserManager userManager;

        public string LoggedInUser { get; private set; }

        public LoginForm()
        {
            userManager = new UserManager();
            InitializeComponents();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "Đăng nhập";
        }

        private void InitializeComponents()
        {
            this.Size = new Size(300, 200);

            picAvatar = new PictureBox
            {
                Size = new Size(80, 80),
                Location = new Point(110, 10),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = Image.FromFile("Resources/default-avatar.png"),
                BorderStyle = BorderStyle.FixedSingle
            };

            System.Windows.Forms.Label lblUsername = new System.Windows.Forms.Label
            {
                Text = "Tên đăng nhập:",
                Location = new Point(20, 20),
                Size = new Size(100, 20)
            };

            txtUsername = new TextBox
            {
                Location = new Point(120, 20),
                Size = new Size(150, 20)
            };

            System.Windows.Forms.Label lblPassword = new System.Windows.Forms.Label
            {
                Text = "Mật khẩu:",
                Location = new Point(20, 50),
                Size = new Size(100, 20)
            };

            txtPassword = new TextBox
            {
                Location = new Point(120, 50),
                Size = new Size(150, 20),
                PasswordChar = '•'
            };

            btnLogin = new Button
            {
                Text = "Đăng nhập",
                Location = new Point(120, 90),
                Size = new Size(100, 30)
            };
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "Đăng ký",
                Location = new Point(120, 130),
                Size = new Size(100, 30)
            };
            btnRegister.Click += BtnRegister_Click;

            this.Controls.AddRange(new Control[] {
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnLogin, btnRegister
            });
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (userManager.ValidateUser(txtUsername.Text, txtPassword.Text))
            {
                LoggedInUser = txtUsername.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!");
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterForm())
            {
                registerForm.ShowDialog();
            }
        }

        private void TxtUsername_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                string avatarPath = userManager.GetUserAvatar(txtUsername.Text);
                try
                {
                    picAvatar.Image = Image.FromFile(avatarPath);
                }
                catch
                {
                    picAvatar.Image = Image.FromFile("Resources/default-avatar.png");
                }
            }
        }
    }

    public class RegisterForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private UserManager userManager;

        public RegisterForm()
        {
            userManager = new UserManager();
            InitializeComponents();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.Text = "Đăng ký";
        }

        private void InitializeComponents()
        {
            this.Size = new Size(300, 250);

            System.Windows.Forms.Label lblUsername = new System.Windows.Forms.Label
            {
                Text = "Tên đăng nhập:",
                Location = new Point(20, 20),
                Size = new Size(100, 20)
            };

            txtUsername = new TextBox
            {
                Location = new Point(120, 20),
                Size = new Size(150, 20)
            };

            System.Windows.Forms.Label lblPassword = new System.Windows.Forms.Label
            {
                Text = "Mật khẩu:",
                Location = new Point(20, 50),
                Size = new Size(100, 20)
            };

            txtPassword = new TextBox
            {
                Location = new Point(120, 50),
                Size = new Size(150, 20),
                PasswordChar = '•'
            };

            System.Windows.Forms.Label lblConfirmPassword = new System.Windows.Forms.Label
            {
                Text = "Xác nhận MK:",
                Location = new Point(20, 80),
                Size = new Size(100, 20)
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(120, 80),
                Size = new Size(150, 20),
                PasswordChar = '•'
            };

            btnRegister = new Button
            {
                Text = "Đăng ký",
                Location = new Point(120, 120),
                Size = new Size(100, 30)
            };
            btnRegister.Click += BtnRegister_Click;

            this.Controls.AddRange(new Control[] {
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                lblConfirmPassword, txtConfirmPassword,
                btnRegister
            });
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtConfirmPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 8 ký tự!");
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!");
                return;
            }

            if (userManager.RegisterUser(txtUsername.Text, txtPassword.Text))
            {
                MessageBox.Show("Đăng ký thành công!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập đã tồn tại!");
            }
        }
    }
    public class Post
    {
        public string Content { get; set; }
        public string MediaReference { get; set; }
        public DateTime PostTime { get; set; }
        public string Author { get; set; }
        public Post Next { get; set; }

        public Post(string content, string mediaReference, string author)
        {
            Content = content;
            MediaReference = mediaReference;
            PostTime = DateTime.Now;
            Author = author;
            Next = null;
        }


        public Post(string content, string mediaReference, string author, DateTime postTime)
        {
            Content = content;
            MediaReference = mediaReference;
            PostTime = postTime;
            Author = author;
            Next = null;
        }
    }

    public class SocialMediaLinkedList
    {
        private Post Head;
        private PostManager postManager;

        public SocialMediaLinkedList()
        {
            postManager = new PostManager();
            LoadPostsFromStorage();
        }

        private void LoadPostsFromStorage()
        {
            var posts = postManager.LoadAllPosts();
            Head = null;

            // Convert PostData objects to linked list
            foreach (var post in posts.OrderByDescending(p => p.PostTime))
            {
                AddPost(post.Content, post.MediaReference, post.Author, post.PostTime);
            }
        }

        public void AddPost(string content, string mediaReference, string author)
        {
            Post newPost = new Post(content, mediaReference, author);
            AddPostToList(newPost);
            postManager.SavePost(newPost);
        }

        public void AddPost(string content, string mediaReference, string author, DateTime postTime)
        {
            Post newPost = new Post(content, mediaReference, author, postTime);
            AddPostToList(newPost);
        }

        private void AddPostToList(Post newPost)
        {
            if (Head == null)
            {
                Head = newPost;
                return;
            }

            if (newPost.PostTime >= Head.PostTime)
            {
                newPost.Next = Head;
                Head = newPost;
                return;
            }

            Post current = Head;
            while (current.Next != null && current.Next.PostTime > newPost.PostTime)
            {
                current = current.Next;
            }
            newPost.Next = current.Next;
            current.Next = newPost;
        }

        public List<Post> GetAllPosts()
        {
            List<Post> posts = new List<Post>();
            Post current = Head;
            while (current != null)
            {
                posts.Add(current);
                current = current.Next;
            }
            return posts;
        }
        public void ShufflePosts()
        {
            if (Head == null || Head.Next == null)
            {
                return; // Danh sách rỗng hoặc chỉ có một phần tử
            }

            // Chuyển danh sách liên kết thành danh sách mảng
            List<Post> posts = GetAllPosts();

            // Trộn ngẫu nhiên danh sách mảng
            Random random = new Random();
            for (int i = posts.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (posts[i], posts[j]) = (posts[j], posts[i]); // Hoán đổi
            }

            // Tái tạo danh sách liên kết
            Head = posts[0];
            Post current = Head;
            for (int i = 1; i < posts.Count; i++)
            {
                current.Next = posts[i];
                current = current.Next;
            }
            current.Next = null; // Đảm bảo nút cuối cùng không trỏ tới đâu
        }


        public List<Post> GetUserPosts(string author)
        {
            List<Post> userPosts = new List<Post>();
            Post current = Head;

            while (current != null)
            {
                if (current.Author == author)
                {
                    userPosts.Add(current);
                }
                current = current.Next;
            }

            // Sắp xếp danh sách theo thời gian tăng dần
            userPosts.Sort((p1, p2) => p2.PostTime.CompareTo(p1.PostTime));

            return userPosts;
        }

        public void DeletePost(DateTime postTime)
        {
            Post current = Head;
            Post previous = null;

            while (current != null && current.PostTime != postTime)
            {
                previous = current;
                current = current.Next;
            }

            if (current != null)
            {
                if (previous == null)
                {
                    Head = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }
            }
        }
    
    }
}
