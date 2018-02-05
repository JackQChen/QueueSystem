using System;
using System.Windows.Forms;
using MessageClient.Properties;

namespace MessageClient
{
    public partial class MessageIndicator : UserControl
    {
        Action<StateType, string> actState;

        public MessageIndicator()
        {
            InitializeComponent();
            this.actState = (state, text) =>
            {
                switch (state)
                {
                    case StateType.Success:
                        this.picState.Image = Resources.network;
                        this.timer1.Start();
                        break;
                    case StateType.Error:
                        this.picState.Image = Resources.network_off;
                        this.timer1.Stop();
                        break;
                    case StateType.Loading:
                        this.picState.Image = Resources.loading;
                        this.timer1.Stop();
                        break;
                }
                this.labText.Text = text;
            };
        }

        public void SetState(StateType state, string text)
        {
            if (this.IsHandleCreated)
                this.Invoke(this.actState, state, text);
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            this.labText.Text = "就绪";
        }
    }
    public enum StateType { Success, Error, Loading }
}
