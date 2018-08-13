using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class FDictionaryBLL : BLLBase<FDictionaryDAL, FDictionaryModel>
    {
        public FDictionaryBLL()
            : base()
        {
        }

        public FDictionaryBLL(string connName)
            : base(connName)
        {
        }

        public FDictionaryBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public List<FDictionaryModel> GetModelListByName(string name)
        {
            return this.dal.GetModelListByName(name);
        }
    }
}
