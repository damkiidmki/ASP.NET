using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests : SetData
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.Parse("0da65561-cf56-4942-bff2-22f50cf70d43");
            Partner partner = null;
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.Now
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsBadRequest()
        {
            // Arrange
            var partnerId = Guid.Parse("0da65561-cf56-4942-bff2-22f50cf70d43");
            Partner partner = BasePartner();
            partner.IsActive = false;
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.Now
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_ResetsNumberIssuedPromoCodes()
        {
            // Arrange
            Partner partner = BasePartner();
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.Now
            };

            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            partner.NumberIssuedPromoCodes.Should().Be(0);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_SetsCancelDateForPreviousLimits()
        {
            // Arrange
            Partner partner = BasePartner();
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.Now
            };
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            var currentCancelDate = partner.PartnerLimits.FirstOrDefault(x => !x.CancelDate.HasValue)!.CancelDate;
            // Act
            await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            var newCancelDate = partner.PartnerLimits.FirstOrDefault(x => x.CancelDate.HasValue)!.CancelDate;
            // Assert
            Assert.NotEqual(currentCancelDate, newCancelDate);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ZeroLimit_ReturnsBadRequestWithMessage()
        {
            // Arrange
            Partner partner = BasePartner();
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 0,
                EndDate = DateTime.Now
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            result.As<BadRequestObjectResult>().Value.Should().Be("Лимит должен быть больше 0");
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_UpdatesPartnerAndReturnsCreated()
        {
            // Arrange
            Partner partner = BasePartner();
            var setPartnerPromoCodeLimitRequest = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 1000,
                EndDate = DateTime.Now
            };

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, setPartnerPromoCodeLimitRequest);

            // Assert
            _partnersRepositoryMock.Verify(r => r.UpdateAsync(partner), Times.Once);
            partner.PartnerLimits.Should().ContainSingle(x => x.Limit == setPartnerPromoCodeLimitRequest.Limit);
            result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}