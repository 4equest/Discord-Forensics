namespace CacheViewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void loadCacheDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<MessageData> messageDatas = OpenCacheDir.LoadCacheDir();
            dataGridView1.DataSource = messageDatas;
            //dataGridView1.AutoResizeColumns();
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }
    }
}