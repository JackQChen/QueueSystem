using System.Windows.Forms;

namespace ReportManager
{
    public partial class FrmPreview : Form
    {
        public int PrintCount { get; set; }

        public PreviewControlEx PreviewControl
        {
            get
            {
                return this.preview;
            }
        }

        public FrmPreview()
        {
            InitializeComponent();
            this.preview.AfterPrint += btnPrint =>
            {
                this.PrintCount++;
            };
        }
    }

}
