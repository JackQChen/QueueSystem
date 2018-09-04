using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QueueClient
{
    public class VirtualControlContainer : Control
    {
        internal Graphics graphics;
        public VirtualControlContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                       | ControlStyles.OptimizedDoubleBuffer
                       | ControlStyles.SupportsTransparentBackColor
                       , true);
            this.MouseClick += new MouseEventHandler(VirtualControlContainer_MouseClick);
            this.MouseDown += new MouseEventHandler(VirtualControlContainer_MouseDown);
            this.MouseUp += new MouseEventHandler(VirtualControlContainer_MouseUp);
            this.MouseMove += new MouseEventHandler(VirtualControlContainer_MouseMove);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        internal List<VirtualControl> controls = new List<VirtualControl>();

        public Image Image { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Image != null)
                e.Graphics.DrawImage(this.Image, Point.Empty);
        }

        public void AddControl(VirtualControl control)
        {
            control.Parent = this;
            this.controls.Add(control);
        }

        public void RemoveControl(VirtualControl control)
        {
            this.controls.Remove(control);
        }

        public void ClearControl()
        {
            this.controls.Clear();
        }

        public void Draw()
        {
            this.Image = new Bitmap(this.Width, this.Height);
            this.graphics = Graphics.FromImage(this.Image);
            foreach (var ctl in this.controls)
            {
                ctl.Draw(graphics);
            }
            this.Invalidate();
        }

        void VirtualControlContainer_MouseClick(object sender, MouseEventArgs e)
        {
            var ctl = this.controls.Find(m => m.Rectangle.Contains(e.Location));
            if (ctl != null)
                ctl.OnMouseClick(e);
        }

        void VirtualControlContainer_MouseDown(object sender, MouseEventArgs e)
        {
            var ctl = this.controls.Find(m => m.Rectangle.Contains(e.Location));
            if (ctl != null)
                ctl.OnMouseDown(e);
        }

        void VirtualControlContainer_MouseUp(object sender, MouseEventArgs e)
        {
            var ctl = this.controls.Find(m => m.Rectangle.Contains(e.Location));
            if (ctl != null)
                ctl.OnMouseUp(e);
        }

        VirtualControl ctlIn = null;
        void VirtualControlContainer_MouseMove(object sender, MouseEventArgs e)
        {
            var ctl = this.controls.Find(m => m.Rectangle.Contains(e.Location));
            if (ctl == null)
            {
                if (ctlIn != null)
                {
                    ctlIn.OnMouseLeave(e);
                    ctlIn = null;
                }
            }
            else
            {
                if (ctlIn == null)
                {
                    ctlIn = ctl;
                    ctlIn.OnMouseEnter(e);
                }
            }
        }
    }

    public class VirtualControl
    {
        public Rectangle Rectangle;
        public object Tag;
        public string Name;
        public Font Font = new Font("黑体", 22, FontStyle.Bold);
        public VirtualControlContainer Parent;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseLeave;
        public VirtualControl()
        {
        }
        public void OnMouseClick(MouseEventArgs e)
        {
            if (this.MouseClick != null)
                this.MouseClick(this, e);
        }
        public void OnMouseDown(MouseEventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(this, e);
        }
        public void OnMouseUp(MouseEventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(this, e);
        }
        public void OnMouseEnter(MouseEventArgs e)
        {
            if (this.MouseEnter != null)
                this.MouseEnter(this, e);
        }
        public void OnMouseLeave(MouseEventArgs e)
        {
            if (this.MouseLeave != null)
                this.MouseLeave(this, e);
        }
        public virtual void Draw(Graphics g)
        {
        }

        public void Refresh()
        {
            this.Draw(this.Parent.graphics);
            this.Parent.Invalidate(this.Rectangle);
        }
    }
}
