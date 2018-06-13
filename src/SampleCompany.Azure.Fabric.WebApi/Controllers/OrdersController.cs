using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SampleCompany.Azure.Fabric.Contracts.Data.Dto.Purchase;
using SampleCompany.Azure.Fabric.Contracts.Data.Response.Purchase;

namespace SampleCompany.Azure.Fabric.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        public ActionResult<PurchaseOrderResponse> GetOrders()
        {
            return new PurchaseOrderResponse
            {
                OrderDetails =
                    new List<OrderDetailsDto>
                    {
                        new OrderDetailsDto
                        {
                            ItemId = Guid.NewGuid(),
                            OrderId = Guid.NewGuid()
                        }
                    }
            };
        }

        [HttpPost]
        public ActionResult<PurchaseOrderResponse> PlaceOrder(IList<OrderDto> orders)
        {
            return new PurchaseOrderResponse
            {
                OrderDetails =
                    new List<OrderDetailsDto>
                    {
                        new OrderDetailsDto
                        {
                            ItemId = Guid.NewGuid(),
                            OrderId = Guid.NewGuid()
                        }
                    }
            };
        }
    }
}
