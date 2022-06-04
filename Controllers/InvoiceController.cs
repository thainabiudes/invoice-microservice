using Invoice.API.Data.ValueObjects;
using Invoice.API.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using Customers.API.Data.ValueObjects;
using System.Text;
using Invoice.API.RabbitMQSender;
using Invoice.API.Messages;

namespace Invoice.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : ControllerBase
    {
        private const string URL_CUSTOMER_API = "https://localhost:49904";
        private IInvoiceRepository _repository;
        private IRabbitMQMessageSender _rabbitMQMessageSender;

        public InvoiceController(IInvoiceRepository repository,
            IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _repository = repository ?? throw new
                ArgumentNullException(nameof(repository));
            _rabbitMQMessageSender = rabbitMQMessageSender
                ?? throw new ArgumentNullException(nameof(rabbitMQMessageSender));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceVO>>> FindAll()
        {
            IEnumerable<InvoiceVO> invoices = await _repository.FindAll();

            List<long> uniqueIds = new List<long>();

            foreach (long id in invoices.Select(x => x.CustomerId).Distinct())
            {
                uniqueIds.Add(id);
            }

            ListVO vo = new ListVO();

            vo.ids = uniqueIds;

            List<CustomerVO> customers = await FindListCustomer(vo); // retornando objeto de customers com ids


            foreach(InvoiceVO invoice in invoices)
            {
                foreach(CustomerVO customer in customers)
                {
                    if (invoice.CustomerId == customer.Id)
                    {
                        invoice.customer = customer;
                    }
                }
            }

            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceVO>> FindById(long id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest("ID is required");
            }

            InvoiceVO invoice = await _repository.FindById(id);

            if (invoice == null) return NotFound();

            CustomerVO customer = await FindCustomer(invoice.CustomerId);
            invoice.customer = customer;

            if (invoice == null) return BadRequest();

            return Ok(invoice);
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceVO>> Create([FromBody] InvoiceVO vo)
        {
            if (vo == null ||
                string.IsNullOrEmpty(vo.InitialDate.ToString()) ||
                string.IsNullOrEmpty(vo.FinalDate.ToString()) ||
                string.IsNullOrEmpty(vo.Value.ToString()) ||
                string.IsNullOrEmpty(vo.CustomerId.ToString())
                )
            {
                return BadRequest("All request body is required");
            }

            CustomerVO customer = await FindCustomer(vo.CustomerId);

            if (string.IsNullOrEmpty(customer.Age.ToString()) ||
                string.IsNullOrEmpty(customer.Gender) ||
                string.IsNullOrEmpty(customer.LastName) ||
                string.IsNullOrEmpty(customer.Name))
            {
                return BadRequest("The customer's ID doesn't exist.");
            }

            vo.customer = customer;

            InvoiceVO invoice = await _repository.Create(vo);

            invoice.customer = customer;

            await PostMessageRabbitMQ(invoice);

            return Ok(invoice);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InvoiceVO>> Update(long id, [FromBody] InvoiceVO vo)
        {
            if (string.IsNullOrEmpty(vo.InitialDate.ToString()) ||
                string.IsNullOrEmpty(vo.FinalDate.ToString()) ||
                string.IsNullOrEmpty(vo.Value.ToString()) ||
                string.IsNullOrEmpty(id.ToString()) ||
                string.IsNullOrEmpty(vo.CustomerId.ToString())
                )
            {
                return BadRequest("All request body or params are required");
            }

            var invoice = await _repository.Update(id, vo);
            if (invoice == null) return NotFound("ID not found");
            return Ok(invoice);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            if (string.IsNullOrEmpty(id.ToString()))
            {
                return BadRequest("ID is required");
            }

            var status = await _repository.Delete(id);
            if (!status) return NotFound("ID not found");
            return Ok(status);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public Task PostMessageRabbitMQ(InvoiceVO invoice)
        {
            MessageVO message = new MessageVO();

            message.invoice = invoice;
            message.MessageCreated = DateTime.Today;

            _rabbitMQMessageSender.SendMessage(message, "emailqueue");

            return Task.CompletedTask;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<CustomerVO> FindCustomer(long id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL_CUSTOMER_API);

                using (HttpResponseMessage response = await client.GetAsync("/Customer/" + id))
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    CustomerVO customer = JsonConvert.DeserializeObject<CustomerVO>(result);
                    return customer;
                }
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<List<CustomerVO>> FindListCustomer(ListVO ids)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL_CUSTOMER_API);

                string payload = JsonConvert.SerializeObject(ids);

                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                
                using (HttpResponseMessage response = await client.PostAsync("/Customer/list", content))
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    List<CustomerVO> customer = JsonConvert.DeserializeObject<List<CustomerVO>>(result);
                    return customer;
                }
            }
        }
    }
}