
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
namespace QueueClient
{
    public enum AsyncType { Work = 1, Loading = 2 }

    [ToolboxItem(false)]
    public class ucProcess : UserControl
    {
        private PictureBox pictureBox1;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::QueueClient.Properties.Resources.loading361;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1920, 1080);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // ucProcess
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.pictureBox1);
            this.Name = "ucProcess";
            this.Size = new System.Drawing.Size(1920, 1080);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public ucProcess()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        public void SetPostion()
        {
            this.Width = this.Parent.Width * 2 / 3;
            this.Height = this.Parent.Width * 2 / 3;
            this.Left = (this.Parent.Width - this.Width) / 2;
            this.Top = (this.Parent.Height - this.Height) / 2;
        }
    }

    public class AsyncWork
    {
        private static List<Control> workList = new List<Control>();
        private Action<Action<int, string>> _workAction;

        private Control _control;
        private BackgroundWorker _work;
        private ucProcess _processPanel;
        private AsyncType _asyncType;
        private Dictionary<string, bool> dicControls = new Dictionary<string, bool>();

        public AsyncWork(Control ParentControl)
        {
            _control = ParentControl;
            InitWork();
        }

        private void InitWork()
        {
            _work = new BackgroundWorker();
            _work.WorkerReportsProgress = true;
            _work.DoWork += new DoWorkEventHandler(_work_DoWork);
            _work.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_work_RunWorkerCompleted);
        }

        private void WorkClear()
        {
            if (_work != null)
            {
                _work.DoWork -= new DoWorkEventHandler(_work_DoWork);
                _work.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(_work_RunWorkerCompleted);
                _work.Dispose();
                this._workAction = null;
            }
        }

        void _work_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _workAction((process, caption) =>
                {
                    if (_processPanel.InvokeRequired)
                        _work.ReportProgress(process, caption);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "任务错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void _work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RemoveLoadingLayer();
            WorkClear();
        }

        /// <summary>
        /// 添加遮罩层
        /// </summary>
        private void AddLoadingLayer()
        {
            //foreach (Control ctl in _control.Controls)
            //{
            //    if (string.IsNullOrEmpty(ctl.Name))
            //        continue;
            //    dicControls.Add(ctl.Name, ctl.Enabled);
            //    //ctl.Enabled = false;
            //}
            _processPanel = new ucProcess();
            _processPanel.Dock = DockStyle.Fill;
            _control.Controls.Add(_processPanel);
            _processPanel.SetPostion();
            _processPanel.BringToFront();
        }

        /// <summary>
        /// 移除遮罩层
        /// </summary>
        private void RemoveLoadingLayer()
        {
            _control.Controls.Remove(_processPanel);
            //foreach (Control ctl in _control.Controls)
            //{
            //    if (string.IsNullOrEmpty(ctl.Name))
            //        continue;
            //    ctl.Enabled = dicControls[ctl.Name];
            //    dicControls.Remove(ctl.Name);
            //}
            _processPanel.Dispose();
            lock (workList)
            {
                workList.Remove(_control);
            }
        }

        public void Start(Action<Action<int, string>> workAction)
        {
            this.Start(workAction, AsyncType.Work);
        }

        public void Start(Action<Action<int, string>> workAction, AsyncType asyncType)
        {
            lock (workList)
            {
                if (workList.Contains(this._control))
                    return;
                else
                    workList.Add(this._control);
            }
            this._asyncType = asyncType;
            AddLoadingLayer();
            _workAction = workAction;
            _work.RunWorkerAsync();
        }

    }

}
