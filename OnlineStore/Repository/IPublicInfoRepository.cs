using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public interface IPublicInfoRepository
    {
         IEnumerable<PublicInfo> GetPublicInfo();  
         PublicInfo GetPublicInfoById(int id);
         void AddPublicInfo(PublicInfo publicInfo);
        void EditPublicInfo(int id,PublicInfo publicInfo);
        void DeletePublicInfo(int id);
    }
}
