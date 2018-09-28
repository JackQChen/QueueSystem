using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public class ucReadCardContainer : VirtualControlContainer
    {
        public Action InputCard;
        ucReadCard card;
        public ucReadCardContainer()
        {
            ucReadCard read = new ucReadCard();
            this.Height = 1080;
            this.Width = 1920;
            read.Image1Rectangle.Location = new Point(568, 169);
            read.Image1Rectangle.Size = new Size(830, 477);
            read.Image2Rectangle.Location = new Point(568, 706);
            read.Image2Rectangle.Size = new Size(830, 90);
            read.Image3Rectangle.Location = new Point(678, 513);
            read.Image3Rectangle.Size = new Size(608, 68);
            read.Input += new Action(InputClick);
            this.MouseMove += (s, e) =>
            {
                if (read.Image2Rectangle.Contains(e.Location) || read.Image2Rectangle.Contains(e.Location))
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.Default;
            };
            this.MouseClick += (s, e) =>
            {
                if (read.Image2Rectangle.Contains(e.Location))
                    read.OnButtonClick();
              
            };
            card = read;
            this.AddControl(read);
        }

        public void DrawIdCard(string idCard)
        {
            card.IdCard = idCard;
            card.Refresh();
        }

        void InputClick()
        {
            if (InputCard != null)
                InputCard();
        }
    }

}
