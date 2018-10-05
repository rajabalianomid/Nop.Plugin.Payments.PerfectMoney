using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PerfectMoney.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Services.Orders;
using Nop.Services.Customers;
using Nop.Services.Security;

namespace Nop.Plugin.Payments.PerfectMoney.Controllers
{
    public class PerfectMoneyController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IPermissionService _permissionService;
        public PerfectMoneyController(IOrderService orderService, ICustomerService customerService, ISettingService settingService, IWorkContext workContext, IStoreService storeService, IPermissionService permissionService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _settingService = settingService;
            _workContext = workContext;
            _storeService = storeService;
            _permissionService = permissionService;
        }
        public ActionResult Index(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var perfectMoneyPaymentSettings = _settingService.LoadSetting<PerfectMoneyPaymentSettings>(storeScope);

            var order = _orderService.GetOrderById(id);
            if (order != null)
            {
                var foundAccount = perfectMoneyPaymentSettings.PayeeAccounts[order.CustomerCurrencyCode];
                var url = new PerfectMoney().Pay(foundAccount, perfectMoneyPaymentSettings.PayeeName, (order.OrderTotal * order.CurrencyRate).ToString(), $"http://{HttpContext.Request.Url.Authority}/PerfectMoney/paymentUrl/{order.Id}", $"http://{HttpContext.Request.Url.Authority}/PerfectMoney/notpaymentUrl/{order.Id}", order.Id.ToString(), order.CustomerId.ToString(), order.CustomerCurrencyCode);
                return Content(url, "text/html");
            }
            return Content("Redirect...");
        }
        public ActionResult PaymentUrl(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var order = _orderService.GetOrderById(id);
            if (order != null)
            {
                order.PaymentStatus = Core.Domain.Payments.PaymentStatus.Paid;
            }
            _orderService.UpdateOrder(order);
            return RedirectToRoute("CheckoutCompleted", new { orderId = id });
        }
        public ActionResult NotPaymentUrl(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return RedirectToRoute("HomePage");

            var order = _orderService.GetOrderById(id);
            _orderService.UpdateOrder(order);
            return RedirectToRoute("CheckoutCompleted", new { orderId = id });
        }
    }
}