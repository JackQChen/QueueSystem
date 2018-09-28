using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public class ucMainContainer : VirtualControlContainer
    {
        public Action Work;

        public ucMainContainer()
        {
            ucMain main = new ucMain();
            this.Height = 1080;
            this.Width = 1920;
            main.Button1Rectangle.Location = new Point(214, 350);
            main.Button1Rectangle.Size = new Size(490, 190);
            main.Button2Rectangle.Location = new Point(1218, 350);
            main.Button2Rectangle.Size = new Size(490, 190);
            main.ImageRectangle.Location = new Point(592, 93);
            main.ImageRectangle.Size = new Size(725, 71);
            main.Work += new Action(WorkClick);
            this.MouseMove += (s, e) =>
            {
                if (main.Button1Rectangle.Contains(e.Location) || main.Button2Rectangle.Contains(e.Location))
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.Default;
            };
            this.MouseClick += (s, e) =>
            {
                if (main.Button1Rectangle.Contains(e.Location))
                    main.OnButton1Click();
                if (main.Button2Rectangle.Contains(e.Location))
                    main.OnButton2Click();
            };
            this.AddControl(main);
            this.Draw();
        }

        void WorkClick()
        {
            if (Work != null)
                Work();
        }
    }

}
