using System.Collections.Generic;
using System.Linq;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.data.Concrete.EfCore
{
    public class EfCoreContactMessageRepository : EfCoreGenericRepository<ContactMessage>, IContactMessageRepository
    {
        public EfCoreContactMessageRepository(SupContext context) : base(context)
        {
            
        }
        private SupContext SupContext
        {
            get { return context as SupContext; }
        }

        public List<ContactMessage> GetContactMessages(int page, int pageSize)
        {
            var contactmessages = SupContext.ContactMessages
                                    .OrderBy(d => d.DateAdded)
                                    .ToList();

            return contactmessages.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetContactMessagesCount()
        {
            var contactmessages = SupContext.ContactMessages
                                    .ToList();

            return contactmessages.Count();
        }
    }
}