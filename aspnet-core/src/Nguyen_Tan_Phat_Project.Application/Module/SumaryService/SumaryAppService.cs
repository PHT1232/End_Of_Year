using Abp.Domain.Repositories;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.SumaryService.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Abp.Authorization;
using Nguyen_Tan_Phat_Project.Authorization;
using Abp.Extensions;

namespace Nguyen_Tan_Phat_Project.Module.SumaryService
{
    [AbpAllowAnonymous]
    public class SumaryAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Customer, string> _customerRepository;
        private readonly IRepository<Employee, string> _employeeRepository;
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Category, string> _categoryRepository;
        private readonly IRepository<Expenses, string> _expensesRepository;
        private readonly IRepository<ExportImport, string> _exportImportRepository;
        private readonly IRepository<Storage, string> _storageRepository;
        private readonly IRepository<Structure, string> _structureRepository;
        private readonly IRepository<Retail, string> _retailRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;

        private readonly IRepository<ExportImportCustomer> _exportImportCustomerRepository;
        private readonly IRepository<ExportImportProduct> _exportImportProductRepository;
        private readonly IRepository<ProductExpenses> _productExpensesRepository;
        private readonly IRepository<ProductStorage> _productStorageRepository;
        private readonly IRepository<RetailCustomer> _retailCustomerRepository;
        private readonly IRepository<RetailProduct> _retailProductRepository;

        public SumaryAppService(
            IRepository<Customer, string> customerRepository
            , IRepository<Employee, string> employeeRepository
            , IRepository<Product, string> productRepository
            , IRepository<Category, string> categoryRepository
            , IRepository<Expenses, string> expensesRepository
            , IRepository<ExportImport, string> exportImportRepository
            , IRepository<Storage, string> storageRepository
            , IRepository<Structure, string> structureRepository
            , IRepository<Retail, string> retailRepository
            , IRepository<SubCategory> subCategoryRepository
            , IRepository<ExportImportCustomer> exportImportCustomerRepository
            , IRepository<ExportImportProduct> exportImportProductRepository
            , IRepository<ProductExpenses> productExpensesRepository
            , IRepository<ProductStorage> productStorageRepository
            , IRepository<RetailCustomer> retailCustomerRepository
            , IRepository<RetailProduct> retailProductRepository
            )
        {
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _expensesRepository = expensesRepository;
            _exportImportRepository = exportImportRepository;
            _storageRepository = storageRepository;
            _structureRepository = structureRepository;
            _retailRepository = retailRepository;
            _subCategoryRepository = subCategoryRepository;
            _exportImportCustomerRepository = exportImportCustomerRepository;
            _exportImportProductRepository = exportImportProductRepository;
            _productExpensesRepository = productExpensesRepository;
            _productStorageRepository = productStorageRepository;
            _retailCustomerRepository = retailCustomerRepository;
            _retailProductRepository = retailProductRepository;
        }

        public async Task<float> GetAllSales(string date)
        {
            DateTime dateTime = DateTime.Parse(date);
            var exportImportStructureRevenue = _exportImportRepository.GetAll().Where(e => e.CreationTime.Year >= (dateTime.Year - 1) && e.CreationTime.Year <= dateTime.Year).Select(item => item.TotalPrice - (item.TotalPrice * (item.Discount / 100))).Sum();
            var retailStructureRevenue = _retailRepository.GetAll().Where(e => e.CreationTime.Year >= (dateTime.Year - 1) && e.CreationTime.Year <= dateTime.Year).Select(e => e.TotalPrice).Sum();

            //int totalPriceExport = 0;

            //foreach (var item in exportImportStructureRevenue) {
            //    int totalPriceAfterDiscount = (int)(item.TotalPrice - (item.TotalPrice * (item.Discount / 100)));
            //    totalPriceExport += totalPriceAfterDiscount;
            //}

            float totalRevenue = exportImportStructureRevenue + retailStructureRevenue;

            return totalRevenue;
        }

        public async Task<DatasetDtoList> GetRevenueStructure(string date)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(date);
                var structure = await _structureRepository.GetAll().ToListAsync();
                DatasetDto revenues = new DatasetDto();
                revenues.Labels = new List<string>();
                revenues.Datasets = new List<DatasetClass>();

                DatasetClass datasetClass = new DatasetClass();
                datasetClass.Data = new List<int>();
                datasetClass.BackgroundColor = new List<string>();
                datasetClass.HoverBackgroundColor = new List<string>();
                Random _rand = new Random();

                foreach (var item in structure)
                {
                    var exportImportStructureRevenue = _exportImportRepository.GetAll().Where(e => e.StructureId == item.Id && e.CreationTime.Year >= (dateTime.Year - 1) && e.CreationTime.Year <= dateTime.Year).Select(e => e.TotalPrice - (e.TotalPrice * (e.Discount / 100))).Sum();
                    var retailStructureRevenue = _retailRepository.GetAll().Where(e => e.StructureId == item.Id && e.CreationTime.Year >= (dateTime.Year - 1) && e.CreationTime.Year <= dateTime.Year).Select(e => e.TotalPrice).Sum();
                    int totalRevenue = (int)(exportImportStructureRevenue + retailStructureRevenue);

                    revenues.Labels.Add(item.UnitName);
                    datasetClass.Data.Add(totalRevenue);
                    string randomColor = $"rgba({_rand.Next(256)}, {_rand.Next(256)}, {_rand.Next(256)}, {_rand.Next(256)})";
                    datasetClass.BackgroundColor.Add(randomColor);
                    datasetClass.HoverBackgroundColor.Add(randomColor);
                }

                revenues.Datasets.Add(datasetClass);
                DatasetDtoList list = new DatasetDtoList();
                list.items = revenues;

                return list;
            } catch ( Exception ex )
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<DatasetDtoList> GetProductSales(string date)
        {
            DateTime dateTime = DateTime.Parse(date);
            var exportImportStructureRevenue = await _exportImportRepository.GetAll().Where(e => e.CreationTime.Year >= (dateTime.Year - 1) && e.CreationTime.Year <= dateTime.Year).ToListAsync();
            var retailStructureRevenue = await _retailRepository.GetAll().Where(e => e.CreationTime.Year >= (dateTime.Year - 1) && e.CreationTime.Year <= dateTime.Year).ToListAsync();

            DatasetDto revenues = new DatasetDto();
            revenues.Labels = new List<string>();
            revenues.Datasets = new List<DatasetClass>();

            DatasetClass datasetClass = new DatasetClass();
            datasetClass.Data = new List<int>();
            datasetClass.BackgroundColor = new List<string>();
            datasetClass.HoverBackgroundColor = new List<string>();
            Random _rand = new Random();

            int QTime = 0;
            for (int i = 1; i < 13; i += 3)
            {
                var exportImport = exportImportStructureRevenue.Where(e => e.CreationTime.Month >= i && e.CreationTime.Month <= (i + 3)).ToList();
                QTime++;
                revenues.Labels.Add("Q" + QTime);
                var productQuantity = 0;
                foreach (var export in exportImport)
                {
                    productQuantity += _exportImportProductRepository.GetAll().Where(e => e.ExportImportCode == export.Id).Select(e => e.Quantity).Sum();
                }
                datasetClass.Data.Add(productQuantity);
                string randomColor = $"rgba({_rand.Next(256)}, {_rand.Next(256)}, {_rand.Next(256)}, {_rand.Next(256)})";
                datasetClass.BackgroundColor.Add(randomColor);
                datasetClass.HoverBackgroundColor.Add(randomColor);

            }
            datasetClass.Label = "Sản phẩm";
            datasetClass.BorderWidth = 1;
            revenues.Datasets.Add(datasetClass);

            DatasetDtoList list = new DatasetDtoList();
            list.items = revenues;

            return list;
        }
    }
}
