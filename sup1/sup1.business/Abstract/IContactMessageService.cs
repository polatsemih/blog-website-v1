using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.entity;

namespace sup1.business.Abstract
{
    public interface IContactMessageService
    {
        List<ContactMessage> GetContactMessages(int page, int pageSize);
        int GetContactMessagesCount();
        Task<List<ContactMessage>> GetAll();
        Task<ContactMessage> GetById(int id);
        void Create(ContactMessage entity);
        void Update(ContactMessage entity);
        void Delete(ContactMessage entity);
    }
}