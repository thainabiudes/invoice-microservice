using Invoice.API.Data.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Invoice.API.Repository
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<InvoiceVO>> FindAll();
        Task<InvoiceVO> FindById(long id);
        Task<InvoiceVO> Create(InvoiceVO vo);
        Task<InvoiceVO> Update(long id, InvoiceVO vo);
        Task<bool> Delete(long id);
    }
}
