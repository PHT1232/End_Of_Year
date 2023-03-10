using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using Nguyen_Tan_Phat_Project.Authorization;
using Nguyen_Tan_Phat_Project.Authorization.Users;
using Nguyen_Tan_Phat_Project.Entities;
using Nguyen_Tan_Phat_Project.Module.CategoryManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nguyen_Tan_Phat_Project.Module.CategoryManagement
{
    [AbpAuthorize(PermissionNames.Page_System_Category_View)]
    public class CategoryAppService : Nguyen_Tan_Phat_ProjectAppServiceBase
    {
        private readonly IRepository<Product, string> _productRepository;
        private readonly IRepository<Category, string> _categoryRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<SubCategory> _subCategoryRepository;
        
        public CategoryAppService(
            IRepository<Product, string> productRepository
            , IRepository<Category, string> categoryRepositor
            , IRepository<User, long> userRepositor
            , IRepository<SubCategory> subCategoryRepositor
            )
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepositor;
            _userRepository = userRepositor;
            _subCategoryRepository = subCategoryRepositor;
        }

        [AbpAuthorize(PermissionNames.Page_System_Category_Add)]
        public async Task AddNewAsync(CategoryInputDto input)
        {
            try
            {
                var categoryDto = await _categoryRepository.FirstOrDefaultAsync(e => e.Id == input.CategoryCode || e.CategoryName == input.CategoryName);
                if (categoryDto != null)
                {
                    throw new UserFriendlyException("Đã tồn tại danh mục này");
                }

                DateTime creationTime = DateTime.Now;
                string categoryId = await _categoryRepository.InsertAndGetIdAsync(new Category
                {
                    Id = input.CategoryCode,
                    CategoryName = input.CategoryName,
                    Description = input.Description,
                    LastModificationTime = creationTime,
                });

                if (!input.SubCategorys.IsNullOrEmpty())
                {
                    foreach (var subcategorydto in input.SubCategorys)
                    {
                        var subcategory = new SubCategory
                        {
                            CategoryId = categoryId,
                            SubCategoryName = subcategorydto
                        };
                        _subCategoryRepository.Insert(subcategory);
                    }
                }
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Category_Delete)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                var checkIfCategoryHaveProduct = await _productRepository.FirstOrDefaultAsync(e => e.CategoryId == id);
                if (checkIfCategoryHaveProduct != null)
                {
                    throw new UserFriendlyException("Không thể xóa danh mục đang có sản phẩm");
                }
                await _categoryRepository.DeleteAsync(id);
                await _subCategoryRepository.DeleteAsync(e => e.CategoryId == id);
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Page_System_Category_Update)]
        public async Task UpdateAsync(CategoryInputDto input)
        {
            try
            {
                var categoryUpdateCheck = _categoryRepository.FirstOrDefaultAsync(x => x.CategoryName == input.CategoryName && x.Id != input.CategoryCode);
                if (categoryUpdateCheck != null)
                    throw new UserFriendlyException("Đã có danh mục với tên này");

                var categoryDto = await _categoryRepository.FirstOrDefaultAsync(e => e.Id == input.CategoryCode);
                if (categoryDto == null)
                    throw new UserFriendlyException($"Không thể tìm thấy danh mục này");

                categoryDto.CategoryName = input.CategoryName;
                categoryDto.Description = input.Description;
                if (input.SubCategorys.IsNullOrEmpty())
                {
                    await _subCategoryRepository.DeleteAsync(e => e.CategoryId == input.CategoryCode);
                } else
                {
                    foreach (var subcategorydto in input.SubCategorys)
                    {
                        var subcategory = new SubCategory
                        {
                            CategoryId = input.CategoryCode,
                            SubCategoryName = subcategorydto
                        };
                        _subCategoryRepository.InsertOrUpdate(subcategory);
                    }
                }
                await _categoryRepository.UpdateAsync(categoryDto);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<CategoryGetAllDto>> GetAllAsync(CategoryPagedResultInput input)
        {
            try
            {
                var query = await _categoryRepository.GetAll()
                    .WhereIf(!string.IsNullOrEmpty(input.KeyWord), e => e.CategoryName.Contains(input.KeyWord) || e.Id.Contains(input.KeyWord))
                    .Select(x => new CategoryGetAllDto
                    {
                        CategoryCode = x.Id,
                        CategoryName = x.CategoryName,
                        CreationTime = x.CreationTime,
                        LastModifiedDate = x.LastModificationTime,
                        Username = _userRepository.GetAll().FirstOrDefault(l => l.Id == x.CreatorUserId || l.Id == x.LastModifierUserId).Name
                    }).OrderByDescending(e => e.CreationTime).PageBy(input).ToListAsync();
                int totalCount = _categoryRepository.Count();

                return new PagedResultDto<CategoryGetAllDto>
                {
                    Items = query,
                    TotalCount = totalCount,
                };
            } catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<CategoryOutputDto> GetAsync(string id)
        {
            var category = await _categoryRepository.FirstOrDefaultAsync(e => e.Id == id);
            if (category == null)
            {
                throw new UserFriendlyException($"Không thể tìm thấy danh mục với mã: {id}");
            }

            //var productList = await _productRepository.GetAll()
            //    .Where(e => e.CategoryId == id).ToListAsync();
            var subCategory = await _subCategoryRepository.GetAll()
                .Where(e => e.CategoryId == id).Select(e => e.SubCategoryName).ToListAsync();

            var categoryOutput = new CategoryOutputDto()
            {
                CategoryCode = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description,
                subCategories = subCategory                
            };

            return categoryOutput;
        }
    }
}
