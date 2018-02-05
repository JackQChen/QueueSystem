
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
namespace SystemConfig
{
    public enum AsyncType { Work = 1, Loading = 2 }

    [ToolboxItem(false)]
    public class ucProcess : UserControl
    {

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.labCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.ForeColor = System.Drawing.SystemColors.Control;
            this.progressBar.Location = new System.Drawing.Point(0, 18);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(300, 22);
            this.progressBar.TabIndex = 1;
            // 
            // labCaption
            // 
            this.labCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.labCaption.Location = new System.Drawing.Point(0, 0);
            this.labCaption.Name = "labCaption";
            this.labCaption.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labCaption.Size = new System.Drawing.Size(300, 18);
            this.labCaption.TabIndex = 2;
            this.labCaption.Text = "正在加载...";
            this.labCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ucProcess
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.labCaption);
            this.Name = "ucProcess";
            this.Size = new System.Drawing.Size(300, 40);
            this.ResumeLayout(false);

        }

        #endregion

        private Label labCaption;

        private System.Windows.Forms.ProgressBar progressBar;
        /// <summary>
        /// 进度
        /// </summary>
        public int Process
        {
            get
            {
                return progressBar.Value;
            }
            set
            {
                progressBar.Value = value;
            }
        }

        /// <summary>
        /// 设置标题
        /// </summary>
        public string Caption
        {
            get
            {
                return this.labCaption.Text;
            }
            set
            {
                this.labCaption.Text = value;
            }
        }

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

        public ucProcess(AsyncType type)
        {
            InitializeComponent();
            if (type == AsyncType.Loading)
                this.progressBar.Style = ProgressBarStyle.Marquee;
        }

        public void SetPostion()
        {
            this.Width = this.Parent.Width * 2 / 3;
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
            _work.ProgressChanged += new ProgressChangedEventHandler(_work_ProgressChanged);
            _work.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_work_RunWorkerCompleted);
        }

        private void WorkClear()
        {
            if (_work != null)
            {
                _work.DoWork -= new DoWorkEventHandler(_work_DoWork);
                _work.ProgressChanged -= new ProgressChangedEventHandler(_work_ProgressChanged);
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

        void _work_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this._asyncType == AsyncType.Work)
                _processPanel.Process = e.ProgressPercentage;
            _processPanel.Caption = e.UserState.ToString();
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
            foreach (Control ctl in _control.Controls)
            {
                if (string.IsNullOrEmpty(ctl.Name))
                    continue;
                dicControls.Add(ctl.Name, ctl.Enabled);
                ctl.Enabled = false;
            }
            _processPanel = new ucProcess(this._asyncType);
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
            foreach (Control ctl in _control.Controls)
            {
                if (string.IsNullOrEmpty(ctl.Name))
                    continue;
                ctl.Enabled = dicControls[ctl.Name];
                dicControls.Remove(ctl.Name);
            }
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
