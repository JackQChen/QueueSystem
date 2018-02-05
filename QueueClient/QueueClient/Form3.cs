using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using BLL;
using Model;

namespace QueueClient
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        TUserBLL user = new TUserBLL();
        private void button1_Click(object sender, EventArgs e)
        {
            var ee = user.Insert(new TUserModel()
             {
                 Name = "张三",
                 Remark = "测试"
             });
            ee.Name = "张三2";
            var s = user.Update(ee);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var ul = user.GetModelList();
            var u = user.GetModel(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var jsonString = HttpGet("http://45.77.132.47:8080/api/unitList/1/query.v", "");
            System.Web.Script.Serialization.JavaScriptSerializer converter = new System.Web.Script.Serialization.JavaScriptSerializer();
            var obj = converter.DeserializeObject(jsonString) as Dictionary<string, object>;
            var desc = obj["desc"].ToString();
            var dataArr = obj["data"] as object[];
            var data3 = dataArr[3] as Dictionary<string, object>;
            var name = data3["unitName"].ToString();
            var id = data3["unitSeq"].ToString();
        }
        CookieContainer cookie = new CookieContainer();
        private string HttpPost(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
            request.CookieContainer = cookie;
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            response.Cookies = cookie.GetCookies(response.ResponseUri);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
    }
}
