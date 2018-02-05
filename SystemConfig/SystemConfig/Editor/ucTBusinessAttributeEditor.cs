using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using Model;

namespace SystemConfig.Editor
{
    public partial class ucTBusinessAttributeEditor : UserControl
    {
        public ucTBusinessAttributeEditor()
        {
            InitializeComponent();
        }

        List<TUnitModel> unitList = new List<TUnitModel>();
        List<TBusinessModel> busiList = new List<TBusinessModel>();

        private void ucTBusinessAttributeEditor_Load(object sender, EventArgs e)
        {
            this.txtUnit.ValueInit += TxtUnit_ValueChanged;
            this.txtBusi.ValueInit += TxtBusi_ValueChanged; 
            unitList = new TUnitBLL().GetModelList();
            busiList = new TBusinessBLL().GetModelList();
        } 

        private void TxtUnit_ValueChanged(object obj)
        {
            this.txtUnit.Text = this.unitList.Where(p => p.unitSeq == obj.ToString()).FirstOrDefault().unitName;
        }

        private void TxtBusi_ValueChanged(object obj)
        {
            this.txtBusi.Text = this.busiList.Where(p => p.busiSeq == obj.ToString()).FirstOrDefault().busiName;
        }
    }
}
