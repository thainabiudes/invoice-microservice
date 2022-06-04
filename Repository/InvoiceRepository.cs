using AutoMapper;
using Invoice.API.Data.ValueObjects;
using Invoice.API.Model.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Invoice.API.Model;

namespace Invoice.API.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly MySQLContext _context;
        private IMapper _mapper;

        public InvoiceRepository(MySQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InvoiceVO>> FindAll()
        {
            List<InvoiceEntity> Invoices = await _context.Invoice.ToListAsync();
            return _mapper.Map<List<InvoiceVO>>(Invoices);
        }

        public async Task<InvoiceVO> FindById(long id)
        {
            InvoiceEntity invoice =
                await _context.Invoice.Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            return _mapper.Map<InvoiceVO>(invoice);
        }

        public async Task<InvoiceVO> Create(InvoiceVO vo)
        {
            InvoiceEntity invoice = _mapper.Map<InvoiceEntity>(vo);
            invoice.Id = null;
            _context.Invoice.Add(invoice);
            await _context.SaveChangesAsync();
            return _mapper.Map<InvoiceVO>(invoice);
        }
        public async Task<InvoiceVO> Update(long id, InvoiceVO vo)
        {
            try
            {
                InvoiceEntity invoice = _mapper.Map<InvoiceEntity>(vo);
                invoice.Id = id;
                _context.Invoice.Update(invoice);
                await _context.SaveChangesAsync();
                return _mapper.Map<InvoiceVO>(invoice);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                InvoiceEntity invoice =
                await _context.Invoice.Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                if (invoice == null) return false;
                _context.Invoice.Remove(invoice);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
