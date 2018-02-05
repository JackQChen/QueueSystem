using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueueClient
{
    [System.Serializable()]
    public struct CardIdentity
    {
        private static string[] regions = new string[] { null, null, null, null, null, null, null, null, null, null, null, "北京", "天津", "河北", "山西", "内蒙古", null, null, null, null, null, "辽宁", "吉林", "黑龙江", null, null, null, null, null, null, null, "上海", "江苏", "浙江", "安微", "福建", "江西", "山东", null, null, null, "河南", "湖北", "湖南", "广东", "广西", "海南", null, null, null, "重庆", "四川", "贵州", "云南", "西藏", null, null, null, null, null, null, "陕西", "甘肃", "青海", "宁夏", "新疆", null, null, null, null, null, "台湾", null, null, null, null, null, null, null, null, null, "香港", "澳门", null, null, null, null, null, null, null, null, "国外" };

        /// <summary>
        /// 表示空的身份证号。
        /// </summary>
        public static readonly CardIdentity Empty = new CardIdentity((int)0);

        private long id;

        private CardIdentity(int id)
        {
            this.id = (long)id;
        }

        /// <summary>
        /// 使用 Int64 表示的身份证号初始化 CardIdentity 类的新实例。
        /// </summary>
        /// <param name="id">Int64 表示的身份证号。</param>
        public CardIdentity(long id)
        {
            if (id >= 100000000000000000L && id <= 999999999999999999L) // 18 位身份证号
            {
                //InternalCheckID(id, false, false);
                bool isCheck = InternalCheckIDCard18(id.ToString());
                if (!isCheck)
                    throw new ArgumentNullException("无效的身份证号。");

                this.id = 1000000000000000000L + id;
            }
            else
            {
                throw new ArgumentException("无效的身份证号（" + id.ToString("D18") + "）。");
            }
        }

        /// <summary>
        /// 使用 String 表示的身份证号初始化 CardIdentity 类的新实例。
        /// </summary>
        /// <param name="id">String 表示的身份证号。</param>
        public CardIdentity(string id)
        {
            if (id == null)
                throw new ArgumentNullException("无效的身份证号码。");

            long lid = 0;

            if (id.Length == 18)
            {
                if (id[17] == 'x' || id[17] == 'X')
                {
                    try
                    {
                        lid = Int64.Parse(id.Substring(0, 17));
                    }
                    catch (System.Exception exc)
                    {
                        throw new ArgumentException("无效身份证号，每一位必须是介于 0 到 9 的数字，18 位身份证号的最后一位允许为 “x”。", exc);
                    }

                    InternalCheckID(lid, false, true);
                    this.id = 2000000000000000000L + lid * 10;
                }
                else
                {
                    try
                    {
                        lid = Int64.Parse(id);
                    }
                    catch (System.Exception exc)
                    {
                        throw new ArgumentException("无效身份证号，每一位必须是介于 0 到 9 的数字，18 位身份证号的最后一位允许为 “x”。", exc);
                    }

                    InternalCheckID(lid, false, false);
                    this.id = 1000000000000000000L + lid;
                }
            }
            else
            {
                throw new ArgumentException("无效的身份证号长度，必须是 15 或者 18位。");
            }
        }

        private static void InternalCheckID(long id, bool b15, bool x)
        {
            if (id.ToString().Length == 17)
                CheckIdentity18(id.ToString() + "x");
            else
                CheckIdentity18(b15 ? Get18ID(id.ToString()) : id.ToString());

            long tid = 0;
            long year = 0;
            long mon = 0;
            long day = 0;
            if (b15) // xxxxxx yy mm dd nnn
            {
                tid = (long)(id / 1000000000L);
                year = 1900L + (long)(((long)(id % 1000000000L)) / 10000000L);
                mon = (long)(((long)(id % 10000000L)) / 100000L);
                day = (long)(((long)(id % 100000L)) / 1000L);
            }
            else if (x) // xxxxxx yyyy mm dd nnn
            {
                tid = (long)(id / 100000000000L);
                year = (long)(((long)(id % 100000000000L)) / 10000000L);
                mon = (long)(((long)(id % 10000000L)) / 100000L);
                day = (long)(((long)(id % 100000L)) / 1000L);
            }
            else  // xxxxxx yyyy mm dd nnnn
            {
                tid = (long)(id / 1000000000000L);
                year = (long)(((long)(id % 1000000000000L)) / 100000000L);
                mon = (long)(((long)(id % 100000000L)) / 1000000L);
                day = (long)(((long)(id % 1000000L)) / 10000L);
            }

            if (tid < 110000L || tid > 820000L)
            {
                throw new ArgumentException("无效的身份证行政区代码（应该介于 110000 和 820000 之间）。");
            }

            if (year < 1900L || year > 2999L)
                throw new ArgumentException("无效的身份证号码的出生年份，不是一个实际的年份（应该介于 1900 和 2999 之间）。");

            if (mon < 1L || mon > 12L)
                throw new ArgumentException("无效的身份证号码的出生月份（必须介于 1 和 12 之间）。");

            if (mon == 1 || mon == 3 || mon == 5 || mon == 7 || mon == 8 || mon == 10 || mon == 12)
                if (day < 1 || day > 31)
                    throw new ArgumentException("无效的身份证号码的出生日（对于 " + mon + " 月，必须介于 1 和 31 之间）。");

            if (mon == 2)
            {
                if (DateTime.IsLeapYear(((int)year)))
                {
                    if (day < 1 || day > 29)
                        throw new ArgumentException("无效的身份证号码的出生日（对于闰年 2 月，必须介于 1 和 29 之间）。");
                }
                else
                {
                    if (day < 1 || day > 28)
                        throw new ArgumentException("无效的身份证号码的出生日（对于非闰年 2 月，必须介于 1 和 28 之间）。");
                }
            }
            else
            {
                if (day < 1 || day > 30)
                    throw new ArgumentException("无效的身份证号码的出生日（对于 " + mon + " 月，必须介于 1 和 30 之间）。");
            }
        }

        /// <summary>
        /// 新的18位身份证验证方式  2016-04-07
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        private static bool InternalCheckIDCard18(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false || n < Math.Pow(10, 16) ||
long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }

            string address =
"11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";

            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");

            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');

            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;//符合GB11643-1999标准  
        }

        /// <summary>
        /// 获取当前身份证号实例表示的生日。
        /// </summary>
        public DateTime Birthday
        {
            get
            {
                string s = "";
                if (this.id >= 100000000000000L && this.id <= 999999999999999L)
                    s = Get18ID(this.ToString());
                else
                    s = this.ToString();

                return new DateTime(Int32.Parse(s.Substring(6, 4)), Int32.Parse(s.Substring(10, 2)), Int32.Parse(s.Substring(12, 2)));
            }
        }

        /// <summary>
        /// 获取一个值，该值指示当前身份证号表示的是否是女性。
        /// </summary>
        public bool IsFemale
        {
            get
            {
                string s = "";
                if (this.id >= 100000000000000L && this.id <= 999999999999999L)
                    s = Get18ID(this.ToString());
                else
                    s = this.ToString();

                return (Int32.Parse(s.Substring(17, 1)) % 2) == 1;
            }
        }

        /// <summary>
        /// 获取身份证号所属的省份名称。
        /// </summary>
        public string Province
        {
            get
            {
                string s = "";
                if (this.id >= 100000000000000L && this.id <= 999999999999999L)
                    s = Get18ID(this.ToString());
                else
                    s = this.ToString();

                return GetRegionName(Int32.Parse(s.Substring(0, 2)));
            }
        }

        /// <summary>
        /// 获取身份证号的完整的地区代码（6位）。
        /// </summary>
        public int RegionCode
        {
            get
            {
                string s = "";
                if (this.id >= 100000000000000L && this.id <= 999999999999999L)
                    s = Get18ID(this.ToString());
                else
                    s = this.ToString();

                return Int32.Parse(s.Substring(0, 6));
            }
        }

        /// <summary>
        /// 获取身份证号的顺序号（最后一位可能为“x”）。
        /// </summary>
        public string Sequence
        {
            get
            {
                string s = "";
                if (this.id >= 100000000000000L && this.id <= 999999999999999L)
                    s = Get18ID(this.ToString());
                else
                    s = this.ToString();

                return s.Substring(14, 4);
            }
        }

        /// <summary>
        /// 确定指定的对象实例和当前实例是否相等。
        /// </summary>
        /// <param name="obj">要比较的实例。</param>
        /// <returns>如果两个实例表示同一个身份证号，则返回 true，否则返回 false。</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("无效的身份证号。");

            return this == (CardIdentity)obj;
        }

        /// <summary>
        /// 获取身份证号的字符串形式。
        /// </summary>
        /// <returns>字符串表示的身份证号。</returns>
        public override string ToString()
        {
            if (this.id >= 100000000000000L && this.id <= 999999999999999L)
                return this.id.ToString();
            if (this.id < 2000000000000000000L)
                return (this.id - 1000000000000000000L).ToString();
            return (this.id - 2000000000000000000L).ToString() + "x";
        }

        /// <summary>
        /// 获取身份证号的 Int64 表示形式。
        /// </summary>
        /// <returns>返回身份证号的 Int64 表示形式。如果身份证号的最后一位为“x”，则返回值的最高位（十进制的第 19 位）为 2。</returns>
        public long ToInt64()
        {
            if (this.id >= 100000000000000L && this.id <= 999999999999999L)
                return this.id;
            if (this.id < 2000000000000000000L)
                return this.id - 1000000000000000000L;
            return this.id;
        }

        /// <summary>
        /// 获取 CardIdentity 的哈希代码。
        /// </summary>
        /// <returns>返回 CardIdentity 的哈希代码。</returns>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }

        /// <summary>
        /// 判断指定的两个身份证号是否相等。
        /// </summary>
        /// <param name="id1">身份证号1。</param>
        /// <param name="id2">身份证号2。</param>
        /// <returns>如果两个身份证号为同一个证号，则返回 true；否则返回 false。</returns>
        public static bool operator ==(CardIdentity id1, CardIdentity id2)
        {
            return id1.id == id2.id;
        }

        /// <summary>
        /// 判断指定的两个身份证号是否相等。
        /// </summary>
        /// <param name="id1">身份证号1。</param>
        /// <param name="id2">身份证号2。</param>
        /// <returns>如果两个身份证号为同一个证号，则返回 false；否则返回 true。</returns>
        public static bool operator !=(CardIdentity id1, CardIdentity id2)
        {
            return id1.id != id2.id;
        }

        /// <summary>
        /// 获取指定地区的编码。只提供到省份。
        /// </summary>
        /// <param name="region">地区名称（不包括“省”、“自治区”、“直辖市”字样，如果是国外，则提供“国外”）。</param>
        /// <returns>返回指定地区的编码，如果没有指定的地区，则返回 -1。</returns>
        private static int GetRegionCode(string region)
        {
            if (region == null)
                throw new ArgumentNullException("无效的地区名称。");

            return ((System.Collections.IList)regions).IndexOf(region);
        }

        /// <summary>
        /// 返回指定地区编码的名称。只提供到省份。
        /// </summary>
        /// <param name="code">地区编码。</param>
        /// <returns>如果没有提供合适的编号，则返回空引用。</returns>
        private static string GetRegionName(int code)
        {
            if (code < 0 || code > 91)
                throw new ArgumentException("无效编码。");

            return regions[code];
        }

        private static void CheckIdentity18(string cid)
        {
            if (cid.Length == 15)
                cid = Get18ID(cid);
            string[] aCity = regions;
            double iSum = 0;
            System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex(@"^\d{17}(\d|x)$");
            System.Text.RegularExpressions.Match mc = rg.Match(cid);
            if (!mc.Success)
            {
                throw new ArgumentException("无效身份证号。");
            }
            cid = cid.ToLower();
            cid = cid.Replace("x", "a");
            if (aCity[int.Parse(cid.Substring(0, 2))] == null)
            {
                throw new ArgumentException("无效的行政区编码。");
            }
            try
            {
                DateTime.Parse(cid.Substring(6, 4) + "-" + cid.Substring(10, 2) + "-" + cid.Substring(12, 2));
            }
            catch
            {
                throw new ArgumentException("无效的出生日期编码。");
            }
            for (int i = 17; i >= 0; i--)
            {
                iSum += (System.Math.Pow(2, i) % 11) * int.Parse(cid[17 - i].ToString(), System.Globalization.NumberStyles.HexNumber);

            }
            if (iSum % 11 != 1)
                throw new ArgumentException("无效身份证号，无效校验。");
        }

        private static string Get18ID(string id)
        {
            char[] strJiaoYan = { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
            int[] intQuan = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            string strTemp;
            int intTemp = 0;

            strTemp = id.Substring(0, 6) + "19" + id.Substring(6);
            for (int i = 0; i <= strTemp.Length - 1; i++)
            {
                intTemp += int.Parse(strTemp.Substring(i, 1)) * intQuan[i];
            }

            intTemp = intTemp % 11;
            return strTemp + strJiaoYan[intTemp];
        }
    }
}
