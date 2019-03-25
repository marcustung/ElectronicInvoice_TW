﻿using ElectronicInvoice.Core.Facade;
using ElectronicInvoice.Produce.Mapping;
using System;
using System.Collections.Generic;
using ElectronicInvoice.Models.ViewModel;
using AutoMapper;
using ElectronicInvoice.Produce.Factroy;
using ElectronicInvoice.Produce.InvoiceResult;
using ElectronicInvoice.Produce.Base;

namespace ElectronicInvoice.Service
{
    public class InvoiceService : IInvoiceService
    {
        private IMapper _mapper;
        private InvoiceApiFactroy _invoiceApiFactory;
        private IConfig _config;

        public InvoiceService(IMapper mapper,
            InvoiceApiFactroy invoiceApiFactory,
            IConfig config)
        {
            _mapper = mapper;
            _invoiceApiFactory = invoiceApiFactory;
            _config = config;
        }

        //private static IMapper mapper = MapperSetting.Current.Setting;
        //private static IContainer autofac = AutofacConfig.Register();
        /// <summary>
        /// 取得中獎號碼
        /// </summary>
        /// <param name="invTerm">查詢的期別</param>
        /// <returns></returns>
        public QryWinningListResultViewModel GetWinningList(QryWinningListViewModel viewModel)
        {
            var model = _mapper.Map<QryWinningListModel>(viewModel);
            var resultJson = ExecuteApi(model);
            return JsonConvertFacde.DeserializeObject<QryWinningListResultViewModel>(resultJson);
        }

        public List<InvoiceViewModel> GetInvoice(CarrierTilteViewModel viewModel)
        {
            var carrierTitleModel = _mapper.Map<CarrierTilteModel>(viewModel);
            string result = ExecuteApi(carrierTitleModel);
            var title = JsonConvertFacde.DeserializeObject<CarrierTitleResult>(result);
            List<InvoiceViewModel> InvoiceList = null;
            //查詢成功再加入List中
            if (title.code == "200")
            {
                InvoiceList = _mapper.Map<List<InvoiceViewModel>>(title.details);
            }
            return InvoiceList ?? new List<InvoiceViewModel>();
        }

        private CarrierDetailModel GetDetail(CarrierTilteModel carrierTitle, TitleDetail detail)
        {
            return new CarrierDetailModel()
            {
                invNum = detail.invNum,
                cardNo = detail.cardNo,
                cardEncrypt = carrierTitle.cardEncrypt,
                cardType = "3J0002",
                invDate = $"{Convert.ToInt32(detail.invDate.year) + 1911}/{detail.invDate.month}/{detail.invDate.date}"
            };
        }

        /// <summary>
        /// 執行API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="carrierTitleModel"></param>
        /// <returns></returns>
        private string ExecuteApi<T>(T carrierTitleModel) where T :
            class, new()
        {
            var api = _invoiceApiFactory.GetProxyInstace(carrierTitleModel);
            return api.ExcuteApi(carrierTitleModel);
        }
    }
}