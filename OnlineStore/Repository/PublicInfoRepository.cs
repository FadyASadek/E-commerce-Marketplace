using Microsoft.EntityFrameworkCore;
using OnlineStore.Models;

namespace OnlineStore.Repository
{
    public class PublicInfoRepository : IPublicInfoRepository
    {
        private readonly myContext context;

        public PublicInfoRepository(myContext context)
        {
            this.context = context;
        }

        public void AddPublicInfo(PublicInfo publicInfo)
        {
            try
            {
                context.publicInfos.Add(publicInfo);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the PublicInfo.", ex);
            }
        }

        public void DeletePublicInfo(int id)
        {
            throw new NotImplementedException();
        }

        public void EditPublicInfo(int id,PublicInfo publicInfo)
        {
            PublicInfo oldPublicInfo = GetPublicInfoById(id);
            oldPublicInfo.Title = publicInfo.Title;
            oldPublicInfo.Description = publicInfo.Description;
            oldPublicInfo.Email = publicInfo.Email;
            oldPublicInfo.Logo = publicInfo.Logo;
            context.SaveChanges();
        }

        public IEnumerable<PublicInfo> GetPublicInfo()
        {
           return context.publicInfos.AsNoTracking().ToList();  
        }

        public PublicInfo GetPublicInfoById(int id)
        {
            return context.publicInfos.FirstOrDefault(c => c.Id == id);
        }
    }
}
