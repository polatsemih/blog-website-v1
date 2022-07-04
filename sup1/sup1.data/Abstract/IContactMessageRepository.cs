using System.Collections.Generic;
using sup1.entity;

namespace sup1.data.Abstract
{
    public interface IContactMessageRepository : IRepository<ContactMessage>
    {
        List<ContactMessage> GetContactMessages(int page, int pageSize);
        int GetContactMessagesCount();
    }
}