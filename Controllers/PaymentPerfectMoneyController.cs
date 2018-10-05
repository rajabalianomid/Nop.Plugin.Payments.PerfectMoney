using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PerfectMoney.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.PerfectMoney.Controllers
{
    public class PaymentPerfectMoneyController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        public PaymentPerfectMoneyController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            ILanguageService languageService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._languageService = languageService;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var perfectMoneyPaymentSettings = _settingService.LoadSetting<PerfectMoneyPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.DescriptionText = perfectMoneyPaymentSettings.DescriptionText;
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DescriptionText = perfectMoneyPaymentSettings.GetLocalizedSetting(x => x.DescriptionText, languageId, 0, false, false);
            });
            model.AdditionalFee = perfectMoneyPaymentSettings.AdditionalFee;

            model.PayeeAccount = perfectMoneyPaymentSettings.PayeeAccount;
            model.PayeeName = perfectMoneyPaymentSettings.PayeeName;


            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = _settingService.SettingExists(perfectMoneyPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(perfectMoneyPaymentSettings, x => x.AdditionalFee, storeScope);
            }

            return View("~/Plugins/Payments.PerfectMoney/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var perfectMoneyPaymentSettings = _settingService.LoadSetting<PerfectMoneyPaymentSettings>(storeScope);

            //save settings
            perfectMoneyPaymentSettings.DescriptionText = model.DescriptionText;
            perfectMoneyPaymentSettings.AdditionalFee = model.AdditionalFee;
            perfectMoneyPaymentSettings.PayeeAccount = model.PayeeAccount;
            perfectMoneyPaymentSettings.PayeeName = model.PayeeName;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(perfectMoneyPaymentSettings, x => x.PayeeAccount, false, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(perfectMoneyPaymentSettings, x => x.PayeeName, false, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(perfectMoneyPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(perfectMoneyPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                perfectMoneyPaymentSettings.SaveLocalizedSetting(x => x.DescriptionText,
                    localized.LanguageId,
                    localized.DescriptionText);
            }

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var perfectMoneyPaymentSettings = _settingService.LoadSetting<PerfectMoneyPaymentSettings>(_storeContext.CurrentStore.Id);

            var model = new PaymentInfoModel
            {
                DescriptionText = perfectMoneyPaymentSettings.GetLocalizedSetting(x => x.DescriptionText, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id),
                PayeeAccount = perfectMoneyPaymentSettings.GetLocalizedSetting(x => x.PayeeAccount, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id),
                PayeeName = perfectMoneyPaymentSettings.GetLocalizedSetting(x => x.PayeeName, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id)
            };

            return View("~/Plugins/Payments.PerfectMoney/Views/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();
            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }
    }
}