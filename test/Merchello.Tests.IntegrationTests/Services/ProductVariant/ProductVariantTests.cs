﻿using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.ProductVariant
{
    [TestFixture]
    [Category("Service Integration")]
    public class ProductVariantTests : ServiceIntegrationTestBase
    {
        private IProductService _productService;
        private IProductVariantService _productVariantService;
        private IProduct _product;

        [SetUp]
        public void Init()
        {
            PreTestDataWorker.DeleteAllProducts();
            _productService = PreTestDataWorker.ProductService;
            _productVariantService = PreTestDataWorker.ProductVariantService;

            _product = PreTestDataWorker.MakeExistingProduct();
            _product.ProductOptions.Add(new ProductOption("Color"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Black", "Blk"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Blue", "Blu"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Red", "Red"));
            _product.ProductOptions["Color"].Choices.Add(new ProductAttribute("Green", "Gre"));
            _product.ProductOptions.Add(new ProductOption("Size"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Small", "Sm"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Medium", "M"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("Large", "Lg"));
            _product.ProductOptions["Size"].Choices.Add(new ProductAttribute("X-Large", "XL"));
            _product.Height = 20;
            _product.Weight = 20;
            _product.Length = 20;
            _product.Width = 20;
            _product.Shippable = true;
            _productService.Save(_product);

        }

        /// <summary>
        /// Test verifies that a product variant can be created
        /// </summary>
        [Test]
        public void Can_Create_A_ProductVariant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };

            //// Act
            var variant = _productVariantService.CreateVariantWithKey(_product, attributes);

            //// Assert
            Assert.IsTrue(variant.HasIdentity);
            
        }

        /// <summary>
        /// Test verifies that a product variant cannot be created twice
        /// </summary>
        [Test]
        public void Can_Not_Create_A_Duplicate_ProductVariant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };
            var variant = _productVariantService.CreateVariantWithKey(_product, attributes);

            //// Act 
            
 
            //// Assert
            Assert.Throws<ArgumentException>(() => _productVariantService.CreateVariantWithKey(_product, attributes));
        }

        /// <summary>
        /// Test verifies that a variant can be retrieved by it's key
        /// </summary>
        [Test]
        public void Can_Retrieve_A_ProductVariant_by_Its_Key()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };
            var expected = _productVariantService.CreateVariantWithKey(_product, attributes);
            var key = expected.Key;
            Assert.AreNotEqual(key.ToString(), Guid.Empty.ToString());

            //// Act
            var retrieved = _productVariantService.GetByKey(key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(key == retrieved.Key);
        }

        /// <summary>
        /// Test verifies that product variants can be retrieved for a particular product
        /// </summary>
        [Test]
        public void Can_Retrieve_All_Variants_For_A_Product()
        {
            //// Arrange
            var attributes1 = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };
            var attributes2 = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["XL"]
            };
            var variant1 = _productVariantService.CreateVariantWithKey(_product, attributes1);
            var variant2 = _productVariantService.CreateVariantWithKey(_product, attributes2);

            Assert.IsTrue(_product.ProductVariants.Count == 2);

            //// Act
            var variants = _productVariantService.GetByProductKey(_product.Key);

            //// Assert
           Assert.IsTrue(variants.Any());
            //Assert.IsTrue(2 == variants.Count());

        }

        /// <summary>
        /// Test verifies that a variant can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_A_Variant()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };
            var variant = _productVariantService.CreateVariantWithKey(_product, attributes);
            var key = variant.Key;
            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _productVariantService.Delete(variant);

            //// Assert
            var retrieved = _productVariantService.GetByKey(key);

            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Test verifies deleting an option also deletes its corresponding variants
        /// </summary>
        [Test]
        public void Can_Verify_That_ProductVariant_Is_Deleted_When_An_Option_Is_Deleted()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };
            var variant = _productVariantService.CreateVariantWithKey(_product, attributes);
            
            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _product.ProductOptions.Remove("Size");
            _productService.Save(_product);

            //// Assert
            Assert.IsFalse(_product.ProductVariants.Any());
        }

        /// <summary>
        /// Test verifies deleting an attribute also deletes variants that are assoicated with the attribute
        /// </summary>
        [Test]
        public void Can_Verify_Removing_An_Attribute_Deletes_Variants_That_Have_That_Attribute()
        {
            //// Arrange
            var attributes = new ProductAttributeCollection
            {
                _product.ProductOptions["Color"].Choices["Blk"],
                _product.ProductOptions["Size"].Choices["Lg"]
            };
            var variant = _productVariantService.CreateVariantWithKey(_product, attributes);

            Assert.IsTrue(_product.ProductVariants.Any());

            //// Act
            _product.ProductOptions["Size"].Choices.Remove("Lg");
            _productService.Save(_product);

            //// Assert
            Assert.IsTrue(_product.ProductVariants.Any());
        }
    }
}