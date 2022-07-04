using System.Collections.Generic;
using System.Threading.Tasks;
using sup1.business.Abstract;
using sup1.data.Abstract;
using sup1.entity;

namespace sup1.business.Concrete
{
    public class ContactMessageManager : IContactMessageService
    {
        private readonly IUnitOfWork _unitofwork;
        public ContactMessageManager(IUnitOfWork unitofwork)
        {
            this._unitofwork = unitofwork;
        }

        public List<ContactMessage> GetContactMessages(int page, int pageSize)
        {
            return _unitofwork.ContactMessages.GetContactMessages(page, pageSize);
        }

        public int GetContactMessagesCount()
        {
            return _unitofwork.ContactMessages.GetContactMessagesCount();
        }

        public async Task<List<ContactMessage>> GetAll()
        {
            return await _unitofwork.ContactMessages.GetAll();
        }

        public async Task<ContactMessage> GetById(int id)
        {
            return await _unitofwork.ContactMessages.GetById(id);
        }

        public void Create(ContactMessage entity)
        {
            _unitofwork.ContactMessages.Create(entity);
            _unitofwork.Save();
        }

        public void Update(ContactMessage entity)
        {
            _unitofwork.ContactMessages.Update(entity);
            _unitofwork.Save();
        }

        public void Delete(ContactMessage entity)
        {
            _unitofwork.ContactMessages.Delete(entity);
            _unitofwork.Save();
        }        
    }
}