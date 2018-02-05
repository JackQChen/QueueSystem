using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using SystemConfig.Editor;
using SystemConfig.Editor.Controls;

namespace SystemConfig
{
    public partial class FrmEditor : Form
    {
        public object BLL { get; set; }

        public object Model { get; set; }

        public UserControl Content { get; set; }

        public OperateType Operate { get; set; }

        public FrmEditor()
        {
            InitializeComponent();
        }

        Control FindContentControl(string strKey)
        {
            foreach (Control ctl in this.Content.Controls)
            {
                if (ctl.Tag != null)
                    if (ctl.Tag.ToString().ToLower() == strKey.ToLower())
                        return ctl;
            }
            return null;
        }

        private void FrmEditor_Load(object sender, EventArgs e)
        {
            Content.Dock = DockStyle.Fill;
            this.plMain.Controls.Add(Content);
            if (this.Operate == OperateType.Save || this.Operate == OperateType.Edit || this.Operate == OperateType.View)
            {
                var tpModel = this.Model.GetType();
                foreach (Control ctl in this.Content.Controls)
                {
                    if (ctl.Tag == null || ctl.Tag.ToString() == "")
                        continue;
                    if (ctl is ValueTextBox)
                    {
                        if (this.Operate == OperateType.View)
                            ((ValueTextBox)ctl).ReadOnly = true;
                        var obj = tpModel.GetProperty(ctl.Tag.ToString()).GetValue(this.Model, null);
                        if (obj != null && obj != DBNull.Value)
                            ((ValueTextBox)ctl).InitValue(obj);
                    }
                    //包含NumericTextBox
                    else if (ctl is TextBox)
                    {
                        if (this.Operate == OperateType.View)
                            ((TextBox)ctl).ReadOnly = true;
                        var obj = tpModel.GetProperty(ctl.Tag.ToString()).GetValue(this.Model, null);
                        if (obj != null && obj != DBNull.Value)
                            ctl.Text = obj.ToString();
                    }
                    else if (ctl is ComboBox)
                    {
                        var obj = tpModel.GetProperty(ctl.Tag.ToString()).GetValue(this.Model, null);
                        if (obj != null && obj != DBNull.Value)
                            ((ComboBox)ctl).SelectedValue = obj;
                    }
                    else if (ctl is CheckBox)
                    {
                        var obj = tpModel.GetProperty(ctl.Tag.ToString()).GetValue(this.Model, null);
                        if (obj != null && obj != DBNull.Value)
                            ((CheckBox)ctl).Checked = Convert.ToBoolean(obj);
                    }
                    else if (ctl is DateTimePicker)
                    {
                        var obj = tpModel.GetProperty(ctl.Tag.ToString()).GetValue(this.Model, null);
                        if (obj != null && obj != DBNull.Value)
                            ((DateTimePicker)ctl).Value = (DateTime)obj;
                    }
                    else if (ctl is PictureBox)
                    {
                        var obj = tpModel.GetProperty(ctl.Tag.ToString()).GetValue(this.Model, null);
                        if (obj != null && obj != DBNull.Value)
                        {
                            using (var ms = new MemoryStream((byte[])obj))
                            {
                                ((PictureBox)ctl).Image = Image.FromStream(ms);
                                ms.Close();
                            }
                        }
                    }
                }
            }
            switch (this.Operate)
            {
                case OperateType.New:
                    this.Text = this.Content.Tag + "-新增";
                    this.FindContentControl("id").Text = "新增";
                    break;
                case OperateType.Edit:
                    this.Text = this.Content.Tag + "-编辑";
                    break;
                case OperateType.Save:
                    this.Text = this.Content.Tag + "-维护";
                    break;
                case OperateType.View:
                    this.Text = this.Content.Tag + "-查看";
                    this.btnOK.Visible = false;
                    this.btnOK.Location = this.btnCancel.Location;
                    break;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var check = this.Content as ICheckData;
            if (check != null)
                if (!check.CheckData())
                    return;
            var tpModel = this.Model.GetType();
            foreach (Control ctl in this.Content.Controls)
            {
                if (ctl.Tag == null || ctl.Tag.ToString() == "")
                    continue;
                if (ctl is ValueTextBox)
                    tpModel.GetProperty(ctl.Tag.ToString()).SetValue(this.Model, ((ValueTextBox)ctl).Value, null);
                else if (ctl is NumericTextBox)
                    tpModel.GetProperty(ctl.Tag.ToString()).SetValue(this.Model, ((NumericTextBox)ctl).Value, null);
                else if (ctl is TextBox)
                    tpModel.GetProperty(ctl.Tag.ToString()).SetValue(this.Model, ctl.Text, null);
                else if (ctl is ComboBox)
                    tpModel.GetProperty(ctl.Tag.ToString()).SetValue(this.Model, ((ComboBox)ctl).SelectedValue, null);
                else if (ctl is CheckBox)
                    tpModel.GetProperty(ctl.Tag.ToString()).SetValue(this.Model, ((CheckBox)ctl).Checked, null);
                else if (ctl is DateTimePicker)
                    tpModel.GetProperty(ctl.Tag.ToString()).SetValue(this.Model, ((DateTimePicker)ctl).Value, null);
                else if (ctl is PictureBox)
                {
                    byte[] data = null;
                    var img = ((PictureBox)ctl).Image;
                    if (img != null)
                    {
                        using (var bmp = new Bitmap(img.Width, img.Height))
                        {
                            using (var ms = new MemoryStream())
                            {
                                Graphics.FromImage(bmp).DrawImage(img, Point.Empty);
                                bmp.Save(ms, ImageFormat.Jpeg);
                                data = ms.ToArray();
                                ms.Close();
                            }
                        }
                    }
                    tpModel.GetProperty(ctl.Tag.ToString()).SetValue(this.Model, data, null);
                }
            }
            switch (this.Operate)
            {
                case OperateType.New:
                    this.BLL.GetType().GetMethod("Insert").Invoke(this.BLL, new object[] { this.Model });
                    break;
                case OperateType.Edit:
                    this.BLL.GetType().GetMethod("Update").Invoke(this.BLL, new object[] { this.Model });
                    break;
                case OperateType.Save:
                    if (this.FindContentControl("id").Text == "0")
                        this.BLL.GetType().GetMethod("Insert").Invoke(this.BLL, new object[] { this.Model });
                    else
                        this.BLL.GetType().GetMethod("Update").Invoke(this.BLL, new object[] { this.Model });
                    break;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }

    public enum OperateType { New, Edit, Save, View }
}
