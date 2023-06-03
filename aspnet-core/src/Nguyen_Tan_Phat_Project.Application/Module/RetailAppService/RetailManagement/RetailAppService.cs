using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Entities;
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


    }
}
