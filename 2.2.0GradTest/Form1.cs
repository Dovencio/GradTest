namespace GradTest
{
    public partial class Form1 : Form
    {
        UserControl1 UserControl1;

        public Form1()
        {
            InitializeComponent();
            UserControl1 = new UserControl1()
            {
                Dock = DockStyle.Fill,
            };
            Controls.Add(UserControl1);
        }

        private void timer1_Tick(object sender, EventArgs e) => UserControl1.Render();
    }
}