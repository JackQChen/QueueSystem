using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class Person
{
    public string idcard;//卡号
    public string name;//姓名
    public string sex;      //性别
    public string birthday;   //出生日期
    public string address;  //地址，在识别护照时导出的是国籍简码
    public string signdate;   //签发日期，在识别护照时导出的是有效期至 
    public string validtermOfStart;  //有效起始日期，在识别护照时为空
    public string validtermOfEnd;  //有效截止日期，在识别护照时为空
}
