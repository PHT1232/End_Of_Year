using Abp.Domain.Repositories;
using Abp.UI;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement.Dtos;
using Nguyen_Tan_Phat_Project.Module.StorageAppService.ExportImportManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.RetailAppService.RetailManagement
{
    public class RetailAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Retail, string> _retailRepository;
        private readonly IRepository<RetailCustomer> _retailCustomerRepository;
        private readonly IRepository<RetailProduct> _retailProductRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Employee, string> _employeeRepository;

        public RetailAppService(IRepository<Storage, string> storageRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<Product, string> productRepository
            , IRepository<Retail, string> retailRepository
            , IRepository<RetailCustomer> retailCustomerRepository
            , IRepository<RetailProduct> retailProductRepository
            )
        {
            _storageRepository = storageRepository;
            _employeeRepository = employeeRepository;
            _productStorageRepository = productStorageRepository;
            _productRepository = productRepository;
            _retailRepository = retailRepository;
            _retailProductRepository = retailProductRepository;
            _retailCustomerRepository = retailCustomerRepository;
        }

        public async Task AddNewAsync(RetailInputDto input)
        {
            try
            {
                var retail = await _retailRepository.FirstOrDefaultAsync(e => e.Id == input.RetailCode);
                if (retail != null)
                {
                    throw new UserFriendlyException("Đơn này đã tồn tại");
                }

                DateTime creationTime = DateTime.Now;
                retail = new Retail
                {
                    Id = input.RetailCode,
                    StructureId = input.StructureId,
                    NameOfReceiver = input.Customer.CustomerName,
                    OrderCreator = input.OrderCreator,
                    DeliveryEmployee = input.DeliveryEmployee,
                    OrderStatus = 1,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                    Discount = input.Discount,
                    TotalPrice = input.TotalPrice,
                };
                string id = await _retailRepository.InsertAndGetIdAsync(retail);

                var customer = new RetailCustomer
                {
                    RetailCode = id,
                    CustomerCode = input.Customer.CustomerCode,
                    ReciveAddress = input.Customer.RevciveAddress,
                    Discount = input.Discount,
                    PhoneToCall = input.Customer.PhoneToCall,
                };

                await _retailCustomerRepository.InsertAsync(customer);

                foreach (var product in input.Products)
                {
                    var exportImportProduct = new RetailProduct
                    {
                        RetailId = id,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        FinalPrice = product.FinalPrice,
                    };
                    _retailProductRepository.Insert(exportImportProduct);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
